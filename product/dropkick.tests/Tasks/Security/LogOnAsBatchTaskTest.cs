using System.Security.Principal;

namespace dropkick.tests.Tasks.Security
{
    using System;
    using dropkick.DeploymentModel;
    using dropkick.Tasks.Security.LocalPolicy;
    using NUnit.Framework;

    [TestFixture]
    [Category("Integration")]
    public class LogOnAsBatchTaskTest
    {

        [Test]
        public void Verify()
        {
            var t = new LogOnAsBatchTask(new DeploymentServer(Environment.MachineName),  WindowsIdentity.GetCurrent().Name);
            var r = t.VerifyCanRun();
        }

        [Test]
        public void Execute()
        {
            var t = new LogOnAsBatchTask(new DeploymentServer(Environment.MachineName), WindowsIdentity.GetCurrent().Name);
            var r = t.Execute();
            r.LogToConsole();
            Assert.AreEqual(false, r.ContainsError());
        }

        [Fact]
        public void should_work_remotely_as_well()
        {
            var t = new LogOnAsBatchTask(new DeploymentServer("127.0.0.1"), WindowsIdentity.GetCurrent().Name);
            var r = t.Execute();
            r.LogToConsole();
            Assert.AreEqual(false, r.ContainsError());
        }



    }
}