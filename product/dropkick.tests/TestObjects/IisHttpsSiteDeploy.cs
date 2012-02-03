namespace dropkick.tests.TestObjects
{
    using dropkick.Configuration.Dsl;
    using dropkick.Configuration.Dsl.Iis;

    /// <summary>
    /// dk.exe execute /deployment:"dropkick.tests.TestObjects.IisHttpsSiteDeploy,dropkick.tests" 
    /// </summary>
    public class IisHttpsSiteDeploy :
        Deployment<IisHttpsSiteDeploy, SampleConfiguration>
    {
        public IisHttpsSiteDeploy()
        {
            Define(settings =>
                       DeploymentStepsFor(Web, server =>
                                                   server.Iis7Site("_Dropkick_6550", @"C:\Temp", bindings =>
                                                       {
                                                           bindings.Add("http", 16030);
                                                           bindings.Add("https", 16031, "13d8ae4000e8d5ac8930c3cdb6c995640c715b86");
                                                       })
                                                   .VirtualDirectory("dk_test")
                                                   .SetPathTo(@"C:\Temp")));
        }

        public static Role Web { get; set; }
    }
}