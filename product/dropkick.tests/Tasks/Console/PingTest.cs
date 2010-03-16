namespace dropkick.tests.Tasks.Console
{
    using DeploymentModel;
    using dropkick.DeploymentModel;
    using dropkick.Tasks.CommandLine;
    using NUnit.Framework;

    [TestFixture]
    public class PingTest
    {
        [Test]
        public void Execute()
        {
            var t = new LocalCommandLineTask("ping");
            t.Args = "localhost";
            t.Execute();

        }

        [Test]
        public void Verify()
        {
            var t = new LocalCommandLineTask("ping");
            t.Args = "localhost";
            var r = t.VerifyCanRun();
            var vi = new DeploymentItem(DeploymentItemStatus.Good, "");

            Assert.AreEqual(1, r.Results.Count);
            
            //Assert.Contains(vi, r.Results);
        }
    }
}