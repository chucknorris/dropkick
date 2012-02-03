using System.Collections.Generic;
using Magnum;
using dropkick.Tasks.Iis;

namespace dropkick.Configuration.Dsl.Iis
{
    public class IisSiteBindingCollection : List<IisSiteBinding>
    {
        public void Add(string protocol, int port)
        {
            Guard.AgainstEmpty(protocol, "protocol");
            Guard.GreaterThan(1, port, "port");
            Add(new IisSiteBinding{ Protocol = protocol.ToLower(), Port = port });
        }

        public void Add(string protocol, int port, string certificateThumbprint)
        {
            Guard.AgainstEmpty(protocol, "protocol");
            Guard.GreaterThan(1, port, "port");
            Guard.AgainstEmpty(certificateThumbprint, "certificateThumbprint");
            Add(new IisSiteBinding{ Protocol = protocol.ToLower(), Port = port, CertificateThumbPrint = certificateThumbprint});
        }
    }
}