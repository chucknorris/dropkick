using System.Configuration;
using NUnit.Framework;
using dropkick.Tasks.Security.Certificate;

namespace dropkick.tests.Tasks.Security.Certificate
{
    [Category("Integration")]
    public class When_querying_certificate_hash_for_a_known_thumbprint : TinySpec
    {
        [Fact]
        public void It_should_find_the_certificate()
        {
            Assert.IsTrue(_store.CertificateExists(ConfigurationManager.AppSettings["CertificateStore.LocalCertificateThumbprint"]));
        }

        public override void Context()
        {
            _store = new CertificateStore();
        }

        public override void Because()
        {            
        }

        CertificateStore _store;
    }

    [Category("Integration")]
    public class When_querying_certificate_store_on_a_remote_machine : TinySpec
    {
        [Fact]
        public void It_should_find_the_certificate()
        {
            Assert.IsTrue(_store.CertificateExists(ConfigurationManager.AppSettings["CertificateStore.RemoteCertificateThumbprint"]));
        }

        public override void Context()
        {
            _store = new CertificateStore(ConfigurationManager.AppSettings["CertificateStore.RemoteMachineName"]);
        }

        public override void Because()
        {
        }

        CertificateStore _store;
    }
}
