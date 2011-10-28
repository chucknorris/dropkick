using Microsoft.Web.Administration;
using NUnit.Framework;

namespace dropkick.tests
{
    public static class ApplicationPoolTestExtensions
    {
        public static void ShouldBeRunning(this ApplicationPool applicationPool)
        {
            Assert.AreEqual(ObjectState.Started, applicationPool.State);
        }

        public static void ShouldBeStopped(this ApplicationPool applicationPool)
        {
            Assert.AreEqual(ObjectState.Stopped, applicationPool.State);
        }
    }

}
