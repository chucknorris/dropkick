namespace dropkick.Engine
{
    using System;
    using DeploymentFinders;
    using log4net;

    public static class Runner
    {
        static readonly ILog _log = LogManager.GetLogger(typeof(Runner));

        public static void Deploy(string commandLine)
        {
            try
            {
                var newArgs = DeploymentCommandLineParser.Parse(commandLine);
                _log.DebugFormat("Deployment: '{0}'", newArgs.Deployment);

                //what is the better way to state this?
                DeploymentFinder finder = newArgs.Deployment == "SEARCH" ?
                    new SearchesForAnAssemblyEndingInDeployment() :
                        newArgs.Deployment.EndsWith(".dll") || newArgs.Deployment.EndsWith(".exe") ?
                            new AssemblyWasSpecifiedAssumingOnlyOneDeploymentClass() :
                            (DeploymentFinder)new TypeWasSpecifiedAssumingItHasADefaultConstructor();


                var deployment = finder.Find(newArgs.Deployment);

                DeploymentPlanDispatcher.KickItOutThereAlready(deployment, newArgs);

            }
            catch (Exception ex)
            {
                _log.Debug(commandLine);
                _log.Error(ex);
            }
        }
    }
}