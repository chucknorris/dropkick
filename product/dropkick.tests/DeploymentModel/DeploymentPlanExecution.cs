namespace dropkick.tests.DeploymentModel
{
    using dropkick.DeploymentModel;
    using NUnit.Framework;

    public class DeploymentPlanExecution :
        WithSimpleModel
    {

        public DeploymentResult Result { get; private set; }

        public override void BecauseOf()
        {
            Plan.Execute();
        }

        [Test]
        public void ThreeTasksSixResults()
        {
            Assert.AreEqual(6, Result.ResultCount);
        }
    }
}