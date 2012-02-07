using NUnit.Framework;
using dropkick.Tasks.Security.Certificate;

namespace dropkick.tests.Tasks.Security.Certificate
{
    [Category("Integration")]
    public class When_getting_certificate_hash_for_a_known_thumbprint : TinySpec
    {
        static readonly byte[] ExpectedHash = new byte[]
                                                  {
                                                      177, 118, 143, 52, 108, 228, 20, 26, 202, 185,
                                                      189, 217, 132, 126, 50, 217, 143, 19, 115, 66
                                                  };
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
            _certHash = _store.GetCertificateHashForThumbprint("b1 76 8f 34 6c e4 14 1a ca b9 bd d9 84 7e 32 d9 8f 13 73 42");
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
            _store = new CertificateStore("APP01.tron.espares.co.uk");
        }

        public override void Because()
        {
            _certHash = _store.GetCertificateHashForThumbprint("e833d41450b1f4bbf69467fd9906d373a699d239");
        }

        byte[] _certHash;
        CertificateStore _store;
    }
}
