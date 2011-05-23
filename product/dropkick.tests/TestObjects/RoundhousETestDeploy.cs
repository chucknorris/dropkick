using dropkick.Tasks.RoundhousE;

namespace dropkick.tests.TestObjects
{
    using dropkick.Configuration.Dsl;
    using dropkick.Configuration.Dsl.RoundhousE;

    public class RoundhousETestDeploy :
        Deployment<RoundhousETestDeploy, SampleConfiguration>
    {
        public RoundhousETestDeploy()
        {
            Define((settings) => DeploymentStepsFor(Db, server =>
            {
                server.RoundhousE()
                    .ForEnvironment("TEST")
                    .OnInstance(".")
                    .OnDatabase("test")
                    .WithDatabaseRecoveryMode(DatabaseRecoveryMode.Simple);
            }));
        }

        public static Role Db { get; set; }
    }
}