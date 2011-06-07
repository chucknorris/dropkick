using System;
using System.Collections.Generic;
using System.IO;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using dropkick.DeploymentModel;
using dropkick.FileSystem;
using dropkick.Tasks.Security;
using dropkick.Tasks.Security.Certificate;
using NUnit.Framework;
using Path = dropkick.FileSystem.Path;

namespace dropkick.tests.Tasks.Security.Certificate
{
    public class SecurityX509CertificateSpecs
    {
        #region Nested type: SecurityX509CertificateSpecsBase

        public abstract class SecurityX509CertificateSpecsBase : TinySpec
        {
            protected DeploymentResult result;

            public override void Context()
            {
                result = new DeploymentResult();
            }
        }

        #endregion

        #region Nested type: when_granting_read_access_to_the_private_key_of_an_X509_certificate

        [ConcernFor("CertificateSecurity"), Category("Integration")]
        public class when_granting_read_access_to_the_private_key_of_an_X509_certificate : SecurityX509CertificateSpecsBase
        {
            private GrantReadCertificatePrivateKeyTask task;
            private IList<string> groups;
            private string thumbprint = "e120c0c2a147f181d0752117db9476db570ebe9f";
            private readonly Path dotNetPath = new DotNetPath();
            private X509Certificate2 certificate;
            private string keyLocation;

            public override void Context()
            {
                base.Context();
                keyLocation = dotNetPath.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Microsoft", "Crypto", "RSA", "MachineKeys");

                groups = new List<string>
                {
                    WellKnownSecurityRoles.Users
                };

                PhysicalServer server =new DeploymentServer("localhost");
                task = new GrantReadCertificatePrivateKeyTask(server,groups, thumbprint, StoreName.My, StoreLocation.LocalMachine, dotNetPath);
                certificate = BaseSecurityCertificatePermissionsTask.FindCertificateBy(thumbprint, StoreName.My, StoreLocation.LocalMachine, server,result);
            }

            public override void Because()
            {
                result = task.Execute();
            }

            [Fact]
            public void should_be_able_to_find_certificate()
            {
                Assert.IsNotNull(certificate);
            }

            [Fact]
            public void should_grant_read_access_to_users()
            {
                bool foundUser = false;

                var rsa = certificate.PrivateKey as RSACryptoServiceProvider;

                if (rsa != null)
                {
                    var file = new FileInfo(dotNetPath.Combine(keyLocation, rsa.CspKeyContainerInfo.UniqueKeyContainerName));
                    var fs = file.GetAccessControl();
                    var rules = fs.GetAccessRules(true, true, typeof(NTAccount));
                    foreach (AuthorizationRule rule in rules)
                    {
                        if (rule.IdentityReference.Value.ToLower() == WellKnownSecurityRoles.Users.ToLower())
                        {
                            foundUser = true;
                        }
                    }
                }

                Assert.AreEqual(true, foundUser);
            }
        }
    }

        #endregion
}