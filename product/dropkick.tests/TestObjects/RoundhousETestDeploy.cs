namespace dropkick.tests.TestObjects
{
    using dropkick.Configuration.Dsl;
    using dropkick.Configuration.Dsl.RoundhousE;

    public class RoundhousETestDeploy :
        Deployment<IisTestDeploy, object>
    {
        public RoundhousETestDeploy()
        {
            Define((settings, environment) => DeploymentStepsFor(Db, server =>
            {
                server.RoundhousE()
                    .Environment("TEST")
                    .OnInstance(".")
                    .OnDatabase("test")
                    .UseMsSqlServer2005()
                    .WithRecoveryMode("simple");
            }));
        }

        public static Role Db { get; set; }
    }
}