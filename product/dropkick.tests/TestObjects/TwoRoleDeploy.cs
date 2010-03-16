namespace dropkick.tests.TestObjects
{
    using dropkick.Configuration.Dsl;
    using dropkick.Configuration.Dsl.Files;

    public class TwoRoleDeploy :
        Deployment<TwoRoleDeploy, object>
    {
        public static Role Web { get; set; }
        public static Role Db { get; set; }

        public TwoRoleDeploy()
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