namespace dropkick.Engine.DeploymentFinders
{
    using Configuration.Dsl;

    public interface DeploymentFinder
    {
        Deployment Find(string assemblyName);
    }
}