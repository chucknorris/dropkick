namespace dropkick.tests.Tasks.Security
{
    using dropkick.Tasks.Security.LocalPolicy;
    using NUnit.Framework;

    [TestFixture]
    public class LogOnAsBatchTaskTest
    {
        [Test]
        public void Verify()
        {
            var t = new LogOnAsBatchTask("sellersd", "test\\reynoldsr");
            var r = t.VerifyCanRun();
        }

        [Test]
        public void Execute()
        {
            var t = new LogOnAsBatchTask("sellersd", "test\\reynoldsr");
            var r = t.Execute();
        }
    }
}