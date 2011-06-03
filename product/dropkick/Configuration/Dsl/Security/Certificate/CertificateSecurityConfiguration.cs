namespace dropkick.Configuration.Dsl.Security.Certificate
{
    public class CertificateSecurityConfiguration : CertificateSecurityConfig 
    {
        private readonly ProtoServer _server;
        private readonly string _thumbprint;

        public CertificateSecurityConfiguration(ProtoServer server, string thumbprint)
        {
            _server = server;
            _thumbprint = thumbprint;
        }

        public GrantReadCertificateOptions GrantReadPrivateKey()
        {
            var proto = new ProtoGrantReadX509CertificatePrivateKeyTask(_thumbprint);
            _server.RegisterProtoTask(proto);

            return proto;
        }
    }
}