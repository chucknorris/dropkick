using System.Security.Cryptography.X509Certificates;

namespace dropkick.Configuration.Dsl.Security.Certificate
{
    public interface GrantReadCertificateOptions
    {
        GrantReadCertificateOptions To(params string[] groupAndOrAccountNames);
        GrantReadCertificateOptions InStoreName(StoreName name);
        GrantReadCertificateOptions InStoreLocation(StoreLocation location);
    }
}