namespace dropkick.tests.TestObjects
{
    using dropkick.Configuration.Dsl;
    using dropkick.Configuration.Dsl.Iis;

    public class IisTestDeploy :
        Deployment<IisTestDeploy, object>
    {
        public IisTestDeploy()
        {
            Define(settings => DeploymentStepsFor(Web, server =>
            {
                server.Iis7Site("Default Web Site")
                    .VirtualDirectory("dk_test")
                    .CreateIfItDoesntExist();

                server.Iis7Site("Default Web Site")
                    .VirtualDirectory("fp")
                    .CreateIfItDoesntExist();
            }));
        }

        public static Role Web { get; set; }
    }
}