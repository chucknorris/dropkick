namespace dropkick.tests.Engine
{
    using dropkick.DeploymentModel;
    using NUnit.Framework;

    [TestFixture]
    public class PhysicalServerTests
    {
        [Test]
        public void LocalhostShouldBeLocal()
        {
            var srv = new DeploymentServer("localhost");
            Assert.IsTrue(srv.IsLocal);
        }
    }
}