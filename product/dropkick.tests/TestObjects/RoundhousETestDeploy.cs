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