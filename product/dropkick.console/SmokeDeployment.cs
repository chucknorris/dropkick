namespace dropkick.console
{
    using Configuration.Dsl;
    using Configuration.Dsl.Notes;

    public class SmokeDeployment :
        Deployment<SmokeDeployment, SmokeSettings>
    {
        public SmokeDeployment()
        {
            Define(settings => DeploymentStepsFor(One, server=> server.Note("hi")));
        }

        public static Role One { get; set; }
    }

    public class SmokeSettings
    {
        public string Env { get; set; }
    }
}