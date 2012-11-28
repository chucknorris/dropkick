using NUnit.Framework;
using dropkick.Configuration.Dsl;
using dropkick.Engine;
using dropkick.Exceptions;
using dropkick.tests.TestObjects;

namespace dropkick.tests.Configuration.Dsl
{
    [TestFixture]
    public class should_throw_if_missing_a_mapped_role
    {
        public TwoRoleDeploy Deployment { get; private set; }
        public DropkickDeploymentInspector Inspector { get; private set; }
        public RoleToServerMap Map { get; private set; }

        [TestFixtureSetUp]
        public void EstablishContext()
        {
            Deployment = new TwoRoleDeploy();
            Deployment.Initialize(new SampleConfiguration());

            Map = new RoleToServerMap();
            Map.AddMap("Web", "SrvTopeka09");
            Map.AddMap("Web", "SrvTopeka19");
            Inspector = new DropkickDeploymentInspector(Map);
        }

        [Test]
        [ExpectedException(typeof(DeploymentConfigurationException))]
        public void X()
        {
            var x = Inspector.GetPlan(Deployment);
        }
    }
}