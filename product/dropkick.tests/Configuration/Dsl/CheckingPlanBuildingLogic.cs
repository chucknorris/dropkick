namespace dropkick.tests.Configuration.Dsl
{
    using dropkick.DeploymentModel;
    using NUnit.Framework;

    public class CheckingPlanBuildingLogic :
        WithTwoPartDeployContext
    {
        public DeploymentPlan Plan { get; set; }

        public override void BecauseOf()
        {
            //TODO: not a fan of Inspector.GetPlan - but do I really need to wrap it?
            Plan = Inspector.GetPlan(Deployment, Map);
        }

        [Test]
        public void PlanShouldNotBeNull()
        {
            Assert.IsNotNull(Plan);
        }

        [Test]
        public void PlanShouldHaveTwoRoles()
        {
            Assert.AreEqual(2, Plan.RoleCount, "Roles should be 2");
        }

        [Test]
        public void WebShouldHaveTwoServers()
        {
            Assert.AreEqual(2, Plan.GetRole("Web").ServerCount);
        }

        [Test]
        public void DbShouldHaveOneServer()
        {
            Assert.AreEqual(1, Plan.GetRole("Db").ServerCount);
        }

        [Test]
        public void DbServerShouldBeNamedSrvTopeka02()
        {
            Assert.IsNotNull(Plan.GetRole("Db").GetServer("SrvTopeka02"));
        }

        [Test]
        public void DbServersShouldHaveOneTask()
        {
            Plan.GetRole("Db").ForEachServer(s=> Assert.AreEqual(1, s.DetailCount));
        }

        [Test]
        public void WebServersShouldHaveOneTask()
        {
            Plan.GetRole("Web").ForEachServer(s => Assert.AreEqual(1, s.DetailCount));
        }
    }
}