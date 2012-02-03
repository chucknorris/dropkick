﻿using System;
using System.Security.Cryptography.X509Certificates;
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
        }

        public override void Because()
        {
            _certHash = CertificateStoreUtility.GetCertificateHashForThumbprint("b1 76 8f 34 6c e4 14 1a ca b9 bd d9 84 7e 32 d9 8f 13 73 42");
        }

        byte[] _certHash;
    }
}