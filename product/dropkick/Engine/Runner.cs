namespace dropkick.Engine
{
    using System;
    using System.IO;
    using Configuration.Dsl;
    using DeploymentFinders;
    using log4net;
    using Settings;
    using Magnum.Reflection;

    public static class Runner
    {
        static readonly ILog _log = LogManager.GetLogger(typeof(Runner));
        static readonly SettingsParser _parser = new SettingsParser();
        static readonly ServerMapParser _serverParser = new ServerMapParser();
        static readonly MultipleFinder _finder = new MultipleFinder();

        public static void Deploy(string commandLine)
        {
            try
            {
                //TODO: change this to the new magnum command line parser
                var newArgs = DeploymentCommandLineParser.Parse(commandLine);

                if(!File.Exists(newArgs.PathToServerMapsFile))
                {
                    _log.FatalFormat("Cannot find the server maps for the environment '{0}' at '{1}'", newArgs.Environment, newArgs.PathToServerMapsFile);
                    return;
                }

                newArgs.ServerMappings.Merge(_serverParser.Parse(new FileInfo(newArgs.PathToServerMapsFile)));

                _log.Info("*******SETTINGS*******");
                _log.InfoFormat("Command: {0}", newArgs.Command);
                _log.InfoFormat("Deployment: {0}", newArgs.Deployment);
                _log.InfoFormat("Environment: {0}", newArgs.Environment);
                _log.InfoFormat("Role: {0}", newArgs.Role);
                DisplayServerMappingsForEnvironment(newArgs.ServerMappings);
                VerifyPathToSettingsFile(newArgs.PathToSettingsFile);
                _log.Info("*******SETTINGS*******");

                Console.WriteLine("Press enter to kick it out there");
                Console.ReadKey(true);

                var deployment = _finder.Find(newArgs.Deployment);
                var settingsType = deployment.GetType().BaseType.GetGenericArguments()[1];
                
                //fast invoke had an object return
                var settings = _parser.FastInvoke<SettingsParser, object>(new[] {settingsType}, "Parse", new FileInfo(newArgs.PathToSettingsFile), commandLine,
                                   newArgs.Environment);
                
                
                deployment.Initialize(settings, newArgs.Environment);
                
                DeploymentPlanDispatcher.KickItOutThereAlready(deployment, newArgs);

            }
            catch (Exception ex)
            {
                _log.Debug(commandLine);
                _log.Error(ex);
            }
        }

        static void DisplayServerMappingsForEnvironment(RoleToServerMap mappings)
        {
            _log.Info("Server Mappings");
            foreach (var role in mappings.Roles())
            {
                _log.InfoFormat("  '{0}'", role);

                foreach (var server in mappings.GetServers(role))
                {
                    _log.InfoFormat("    '{0}'", server.Name);
                }
            }
        }

        static void VerifyPathToSettingsFile(string pathToSettingsFile)
        {
            if(File.Exists(pathToSettingsFile))
                _log.InfoFormat("Settings Path: {0}", pathToSettingsFile);
            else
                _log.ErrorFormat("Settings Path: {0}", pathToSettingsFile);
        }
    }
}