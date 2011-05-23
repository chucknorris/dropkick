using System.Security.Principal;

namespace dropkick.tests.Tasks.Security
{
    using System;
    using dropkick.Tasks.Security.LocalPolicy;
    using NUnit.Framework;

    [TestFixture]
    [Category("Integration")]
    public class LogOnAsBatchTaskTest
    {

        [Test]
        public void Verify()
        {
            var t = new LogOnAsBatchTask(Environment.MachineName,  WindowsIdentity.GetCurrent().Name);
            var r = t.VerifyCanRun();
        }

        [Test]
        public void Execute()
        {
            var t = new LogOnAsBatchTask(Environment.MachineName,  WindowsIdentity.GetCurrent().Name);
            var r = t.Execute();
        }
    }
}