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
            Define(settings =>
                {
                    DeploymentStepsFor(Web, server =>
                        {
                            server.CopyTo(".\\anthony").From(".\\katie");
                            server.Copy(o =>
                                   {
                                       o.Include("..\\FHLBank.Cue.Website");
                                       o.Include("..\\settings\\web.test.config").Rename("web.config");
                                   }).To("E:\\FHLBApps\\CUE");
                        });
                    DeploymentStepsFor(Db, server =>
                        {
                            server.CopyTo(".\\rob").From(".\\brandy");
                        });
                });
        }
    }
}