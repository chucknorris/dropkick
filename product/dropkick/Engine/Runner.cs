namespace dropkick.Engine
{
    using System;
    using System.IO;
    using DeploymentFinders;
    using FileSystem;
    using log4net;
    using Settings;
    using Path=dropkick.FileSystem.Path;

    public static class Runner
    {
        static readonly ILog _log = LogManager.GetLogger(typeof(Runner));
        static readonly SettingsParser _parser = new SettingsParser();
        static readonly ServerMapParser _serverParser = new ServerMapParser();
        static readonly MultipleFinder _finder = new MultipleFinder();
        static readonly Path _path = new DotNetPath();

        public static void Deploy(string commandLine)
        {
            try
            {
                var newArgs = DeploymentCommandLineParser.Parse(commandLine);



                var pathToSettingsFile = _path.Combine(newArgs.SettingsDirectory, "{0}.settings".FormatWith(newArgs.Environment));
                var pathToMapFile = _path.Combine(newArgs.SettingsDirectory, "{0}.servermaps".FormatWith(newArgs.Environment));
                if(!File.Exists(pathToMapFile))
                {
                    _log.FatalFormat("Cannot find the server maps for the environment '{0}' at '{1}'", newArgs.Environment, pathToMapFile);
                    return;
                }

                newArgs.ServerMappings.Merge(_serverParser.Parse(new FileInfo(pathToMapFile)));

                _log.Debug("*******SETTINGS*******");
                _log.InfoFormat("Command: {0}", newArgs.Command);
                _log.InfoFormat("Deployment: {0}", newArgs.Deployment);
                _log.InfoFormat("Environment: {0}", newArgs.Environment);
                _log.InfoFormat("Role: {0}", newArgs.Role);
                DisplayServerMappingsForEnvironment(newArgs.ServerMappings);
                VerifyPathToSettingsFile(pathToSettingsFile);
                _log.Debug("*******SETTINGS*******");

                Console.WriteLine("Press enter to kick it out there");
                Console.ReadKey(true);

                var deployment = _finder.Find(newArgs.Deployment);
                var settingsType = deployment.GetType().BaseType.GetGenericArguments()[1];
                var settings = _parser.Parse(settingsType, new System.IO.FileInfo(pathToSettingsFile));
                deployment.Initialize(settings);
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
            _log.Info("ServerMappings");
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