namespace dropkick.tests.TestObjects
{
    using dropkick.Configuration.Dsl;
    using dropkick.Configuration.Dsl.Iis;

    public class IisTestDeploy :
        Deployment<IisTestDeploy, object>
    {
        public IisTestDeploy()
        {
            Define((settings, environment) => DeploymentStepsFor(Web, server =>
            {
                server.Iis7Site("Default Web Site")
                    .VirtualDirectory("dk_test")
                    .UseClassicPipeline();

                server.Iis7Site("Default Web Site")
                    .VirtualDirectory("fp");
            }));
        }

        public static Role Web { get; set; }
    }
}