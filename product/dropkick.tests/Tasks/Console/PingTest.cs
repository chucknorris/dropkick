namespace dropkick.tests.Tasks.Console
{
    using System;
    using dropkick.DeploymentModel;
    using dropkick.Tasks.CommandLine;
    using FileSystem;
    using NUnit.Framework;
    using Path = System.IO.Path;
    using System.Linq;

    [TestFixture]
    public class PingTest
    {
        [Test]
        public void Execute()
        {
            var t = new LocalCommandLineTask(new DotNetPath(), "ping");
            t.Args = "localhost";
            t.Execute();

        }

        [Test]
        public void Verify()
        {
            var t = new LocalCommandLineTask(new DotNetPath(), "ping");
            t.Args = "localhost";
            var r = t.VerifyCanRun();
            var vi = new DeploymentItem(DeploymentItemStatus.Good, "");

            Assert.AreEqual(1, r.Results.Count);

            //Assert.Contains(vi, r.Results);
        }

        [Test]
        public void VerifyCanRunWithExecutablePathSet()
        {
            var t = new LocalCommandLineTask(new DotNetPath(), "ping")
            {
                ExecutableIsLocatedAt =
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "system32"),
                    Args = "localhost"
            };

            var result = t.Execute();
            result.Results.Select(r=> r.Status).ToList().ShouldContain(DeploymentItemStatus.Good);
        }
    }
}