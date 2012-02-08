using System.Configuration;
using NUnit.Framework;
using dropkick.Tasks.Security.Certificate;

namespace dropkick.tests.Tasks.Security.Certificate
{
    [Category("Integration")]
    public class When_getting_certificate_hash_for_a_known_thumbprint : TinySpec
    {
        static readonly byte[] ExpectedHash =
            ConfigurationManager.AppSettings["CertificateStore.LocalCertificateHash"].FromHexToBytes();

        [Fact]
        public void It_should_return_the_correct_hash()
        {
            Assert.AreEqual(ExpectedHash, _certHash);
        }

        public override void Context()
        {
            _store = new CertificateStore();
        }

        public override void Because()
        {
            _certHash = _store.GetCertificateHashForThumbprint(ConfigurationManager.AppSettings["CertificateStore.LocalCertificateThumbprint"]);
        }

        byte[] _certHash;
        CertificateStore _store;
    }

    [Category("Integration")]
    public class When_querying_certificate_store_on_a_remote_machine : TinySpec
    {
        [Fact]
        public void It_should_find_the_certificate()
        {
            Assert.IsNotNull(_certHash);
        }

        public override void Context()
        {
            _store = new CertificateStore(ConfigurationManager.AppSettings["CertificateStore.RemoteMachineName"]);
        }

        public override void Because()
        {
            _certHash = _store.GetCertificateHashForThumbprint(ConfigurationManager.AppSettings["CertificateStore.RemoteCertificateThumbprint"]);
        }

        byte[] _certHash;
        CertificateStore _store;
    }
}
