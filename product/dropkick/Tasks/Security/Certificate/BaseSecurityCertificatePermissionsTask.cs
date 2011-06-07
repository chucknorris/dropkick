using System;
using System.IO;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using dropkick.DeploymentModel;
using dropkick.FileSystem;
using Path = dropkick.FileSystem.Path;

namespace dropkick.Tasks.Security.Certificate
{
    public abstract class BaseSecurityCertificatePermissionsTask : BaseSecurityTask
    {
        public static X509Certificate2 FindCertificateBy(string thumbprint, StoreName storeName, StoreLocation storeLocation, PhysicalServer server, DeploymentResult result)
        {
            if (string.IsNullOrEmpty(thumbprint)) return null;

            var certstore = new X509Store(storeName, storeLocation);

            try
            {
                certstore.Open(OpenFlags.ReadOnly);

                thumbprint = thumbprint.Trim();
                thumbprint = thumbprint.Replace(" ", "");

                foreach (var cert in certstore.Certificates)
                {
                    if (string.Equals(cert.Thumbprint, thumbprint, StringComparison.OrdinalIgnoreCase) || string.Equals(cert.Thumbprint, thumbprint, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return cert;
                    }
                }

                result.AddError("Could not find a certificate with thumbprint '{0}' on '{1}'".FormatWith(thumbprint, server.Name));
                return null;
            }
            finally
            {
                certstore.Close();
            }
        }

        //Reference http://www.codeproject.com/script/Forums/View.aspx?fid=1649&msg=2062983
        //Reference http://stackoverflow.com/questions/425688/how-to-set-read-permission-on-the-private-key-file-of-x-509-certificate-from-net
        protected static void AddAccessToPrivateKey(X509Certificate2 cert, string group, FileSystemRights rights, StoreLocation storeLocation, Path dotNetPath, PhysicalServer server, DeploymentResult result)
        {
            var rsa = cert.PrivateKey as RSACryptoServiceProvider;

            if (rsa == null)
            {
                result.AddError("Certificate does not contain a private key that is accessible");
                return;
            }

            var keyfilepath = FindKeyLocation(storeLocation, dotNetPath);

            var file = dotNetPath.Combine(keyfilepath, rsa.CspKeyContainerInfo.UniqueKeyContainerName);
            file = PathConverter.Convert(server, file);

            dotNetPath.SetFileSystemRights(file, group, rights, result);
            //var account = new NTAccount(group);
            //var fs = file.GetAccessControl();
            //fs.AddAccessRule(new FileSystemAccessRule(account, rights, AccessControlType.Allow));

            //file.SetAccessControl(fs);
        }

        protected static string FindKeyLocation(StoreLocation storeLocation, Path dotNetPath)
        {
            string keyLocation = string.Empty;

            switch (storeLocation)
            {
                case StoreLocation.LocalMachine:
                    keyLocation = dotNetPath.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Microsoft", "Crypto", "RSA", "MachineKeys");
                    break;
                case StoreLocation.CurrentUser:
                    keyLocation = dotNetPath.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Microsoft", "Crypto", "RSA");
                    break;

            }

            return keyLocation;
        }
    }
}