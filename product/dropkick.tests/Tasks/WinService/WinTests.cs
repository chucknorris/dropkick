namespace dropkick.tests.Tasks.WinService
{
    using System;
    using dropkick.Tasks.WinService;
    using NUnit.Framework;

    [TestFixture]
    public class WinTests
    {
        [Test][Category("Integration")]
        public void Start()
        {
            //TODO: friggin 2008 LUA-must run as admin
            var t = new WinServiceStopTask(Environment.MachineName,"MSMQ");
            var o  = t.VerifyCanRun();
            t.Execute();
            var t2 = new WinServiceStartTask(Environment.MachineName, "MSMQ");
            t.Execute();

        }
    }
}