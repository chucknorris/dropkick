namespace dropkick.tests.Tasks.Dsn
{
    using System;
    using dropkick.Tasks.Dsn;
    using NUnit.Framework;

    [TestFixture]
    public class CreatingDsns
    {
        [Test][Explicit]
        public void Execute()
        {
            var t = new DsnTask(Environment.MachineName, "dk_dsn", DsnAction.AddSystemDsn, DsnDriver.Sql(), "Test");
            t.Execute();
        }

        [Test][Explicit]
        public void Verify()
        {
            var t = new DsnTask(Environment.MachineName, "dk_dsn", DsnAction.AddSystemDsn, DsnDriver.Sql(), "Test");
            var r = t.VerifyCanRun();
            Assert.AreEqual(1, r.Results.Count);
        }
    }
}