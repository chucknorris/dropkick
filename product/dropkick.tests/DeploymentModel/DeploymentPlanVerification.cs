namespace dropkick.tests.DeploymentModel
{
    using dropkick.DeploymentModel;
    using NUnit.Framework;

    public class DeploymentPlanVerification :
        WithSimpleModel
    {

        public DeploymentResult Result { get; private set; }

        public override void BecauseOf()
        {
            Plan.Verify();
        }

        [Test]
        public void ThreeTasksThreeResults()
        {
            Assert.AreEqual(3, Result.ResultCount);
        }
    }
}