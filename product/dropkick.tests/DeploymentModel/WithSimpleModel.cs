namespace dropkick.tests.DeploymentModel
{
    using dropkick.DeploymentModel;
    using NUnit.Framework;

    [TestFixture]
    public abstract class WithSimpleModel
    {
        public DeploymentPlan Plan { get; private set;}

        [TestFixtureSetUp]
        public void SetUp()
        {
            var detail = new DeploymentDetail(() => "test detail", () => new DeploymentResult()
                                                                    {
                                                                        new DeploymentItem(DeploymentItemStatus.Good, "verify")
                                                                    }, () => new DeploymentResult()
                                                                             {
                                                                                 new DeploymentItem(DeploymentItemStatus.Good, "execute")
                                                                             });
            var webRole = new DeploymentRole("Web");
            webRole.AddServer(new DeploymentServer("SrvWeb1"));
            webRole.AddServer(new DeploymentServer("SrvWeb2"));
            webRole.ForEachServer(s=>s.AddDetail(detail));
            var dbRole = new DeploymentRole("Db");
            dbRole.AddServer("SrvDb");
            dbRole.ForEachServer(s => s.AddDetail(detail));
            Plan = new DeploymentPlan();
            Plan.AddRole(webRole);
            Plan.AddRole(dbRole);

            BecauseOf();
        }

        public abstract void BecauseOf();
    }
}