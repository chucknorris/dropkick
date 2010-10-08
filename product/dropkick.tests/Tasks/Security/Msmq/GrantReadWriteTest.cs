using System;

namespace dropkick.tests.Tasks.Security.Msmq
{
    using System.Messaging;
    using dropkick.DeploymentModel;
    using dropkick.Tasks.Security.Msmq;
    using Dsl.Msmq;
    using NUnit.Framework;

    [TestFixture]
    [Category("Integration")]
    public class GrantReadWriteTest
    {
        QueueAddress _address;

        [TestFixtureSetUp]
        public void Setup()
        {
            var ub = new UriBuilder("msmq", Environment.MachineName) { Path = "dk_test" };
            _address = new QueueAddress(ub.Uri);

            if (MessageQueue.Exists(_address.LocalName))
                MessageQueue.Delete(_address.LocalName);

            MessageQueue.Create(_address.LocalName);
        }

        [Test]
        public void Execute()
        {
            var ps = new DeploymentServer(Environment.MachineName);
            var t = new MsmqGrantReadWriteTask(ps, _address, @"Everyone");
            var r = t.Execute();

            Assert.IsFalse(r.ContainsError(), "Errors occured during permission setting.");


        }

    }
}
