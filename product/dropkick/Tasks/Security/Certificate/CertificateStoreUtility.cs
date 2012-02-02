using System;
using System.Security.Cryptography.X509Certificates;

namespace dropkick.Tasks.Security.Certificate
{
    public static class CertificateStoreUtility
    {
        public static byte[] GetCertificateHashForThumbprint(string certificateThumbprint)
        {
            var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.OpenExistingOnly);
            try
            {
                var certificate = store.Certificates.Find(X509FindType.FindByThumbprint, certificateThumbprint, false);
                if (certificate.Count == 0)
                    throw new ArgumentException(String.Format("No certificate was found with the specified thumbprint '{0}'", certificateThumbprint));

                return certificate[0].GetCertHash();
            }
            finally
            {
                store.Close();
            }
        }
    }
}
