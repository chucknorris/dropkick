using System;
using System.Security.Cryptography.X509Certificates;

namespace dropkick.Tasks.Security.Certificate
{
    public class CertificateStore
    {
        readonly X509Store _store;

        public CertificateStore() 
        {
            _store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
        }

        public CertificateStore(string machine)
        {
            var storename = String.IsNullOrEmpty(machine) || String.Compare("localhost", machine, StringComparison.OrdinalIgnoreCase) == 0
                ? "MY"
                : @"\\{0}\MY".FormatWith(machine);
            _store = new X509Store(storename.FormatWith(machine), StoreLocation.LocalMachine);
        }

        public byte[] GetCertificateHashForThumbprint(string certificateThumbprint)
        {
            var certificate = getCertificateFromThumbprint(certificateThumbprint);
            if (certificate.Count == 0)
                throw new ArgumentException(String.Format("No certificate was found with the specified thumbprint '{0}'",
                                                          certificateThumbprint));

            return certificate[0].GetCertHash();
        }

        public bool CertificateExists(string thumbprint)
        {
            return getCertificateFromThumbprint(thumbprint).Count > 0;
        }

        X509Certificate2Collection getCertificateFromThumbprint(string certificateThumbprint)
        {
            _store.Open(OpenFlags.OpenExistingOnly);
            try
            {
                var certificate = _store.Certificates.Find(X509FindType.FindByThumbprint, certificateThumbprint, false);
                return certificate;
            }
            finally
            {
                _store.Close();
            }
        }

    }
}
