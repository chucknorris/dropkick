namespace dropkick.tests.DeploymentModel
{
    using dropkick.DeploymentModel;
    using NUnit.Framework;

    public class DeploymentPlanFailingExecution :
        WithFailingModel
    {

        public DeploymentResult Result { get; private set; }

        public override void BecauseOf()
        {
            Result = Plan.Execute();
        }

        [Test]
        public void ThreeTasksFiveResults()
        {
            Assert.AreEqual(5, Result.ResultCount);
        }
    }
}