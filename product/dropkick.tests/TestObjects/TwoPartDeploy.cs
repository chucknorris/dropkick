namespace dropkick.tests.TestObjects
{
    using dropkick.Configuration.Dsl;
    using dropkick.Configuration.Dsl.Files;

    public class TwoPartDeploy :
        Deployment<TwoPartDeploy, object>
    {
        public static Role Web { get; set; }
        public static Role Db { get; set; }

        static TwoPartDeploy()
        {
            Define(() =>
                {
                    DeploymentStepsFor(Web, p =>
                        {
                            p.CopyTo(".\\anthony").From(".\\katie");
                        });
                    DeploymentStepsFor(Db, p =>
                        {
                            p.CopyTo(".\\rob").From(".\\brandy");
                        });
                });
        }
    }
}