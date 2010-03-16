namespace dropkick.Engine
{
    using System;
    using System.IO;
    using DeploymentFinders;
    using log4net;
    using Settings;

    public static class Runner
    {
        static readonly ILog _log = LogManager.GetLogger(typeof(Runner));
        static SettingsParser _parser = new SettingsParser();

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

                var pathToFile = Path.Combine(newArgs.SettingsDirectory,
                                              "{0}.settings".FormatWith(newArgs.Environment));
                var settings = _parser.Parse<object>(new FileInfo(pathToFile));

                deployment.SetSettings(settings);

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