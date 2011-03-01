namespace dropkick.tests.TestObjects
{
    using dropkick.Configuration.Dsl;
    using dropkick.Configuration.Dsl.MsSrss;

    public class MsSsrsDeploy :
        Deployment<MsSsrsDeploy, SampleConfiguration>
    {
        public MsSsrsDeploy()
        {
            Define(cfg=>
            {
                DeploymentStepsFor(Reports, server=>
                {
                    server.SqlReports(rpt=>
                    {
                        rpt.PublishTo("address");

                        rpt.PublishAllIn("folder"); //*.rdl
                        rpt.Publish("name without extension");
                    });
                });
            });
        }


        public static Role Reports { get; set; }
    }
}