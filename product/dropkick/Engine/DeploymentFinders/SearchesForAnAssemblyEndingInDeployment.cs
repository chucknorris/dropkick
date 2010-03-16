namespace dropkick.Engine.DeploymentFinders
{
    using System.Collections.Generic;
    using System.Linq;
    using Configuration.Dsl;

    public class SearchesForAnAssemblyEndingInDeployment :
        DeploymentFinder
    {
        public Deployment Find(string assemblyName)
        {
            //how to find the list of assemblies
            var ass = new List<string>();
            var deploymentAssembly = ass.Where(x => x.EndsWith("Deploment")).First();

            return new AssemblyWasSpecifiedAssumingOnlyOneDeploymentClass().Find(deploymentAssembly);
        }
    }
}