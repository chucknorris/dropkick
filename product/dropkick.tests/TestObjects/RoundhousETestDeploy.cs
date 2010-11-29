namespace dropkick.tests.TestObjects
{
    using dropkick.Configuration.Dsl;
    using dropkick.Configuration.Dsl.RoundhousE;

    public class RoundhousETestDeploy :
        Deployment<IisTestDeploy, SampleConfiguration>
    {
        public RoundhousETestDeploy()
        {
            Define((settings) => DeploymentStepsFor(Db, server =>
            {
                server.RoundhousE()
                    .ForEnvironment("TEST")
                    .OnInstance(".")
                    .OnDatabase("test")
                    .UseSimpleRecoveryMode(true);
            }));
        }

        public static Role Db { get; set; }
    }
}