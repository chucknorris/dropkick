namespace dropkick.Engine
{
    using System;
    using DeploymentFinders;
    using FileSystem;
    using log4net;
    using Settings;

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



                var pathToSettingsFile = _path.Combine(newArgs.SettingsDirectory,
                                              "{0}.settings".FormatWith(newArgs.Environment));
                var pathToMapFile = _path.Combine(newArgs.SettingsDirectory,
                                                 "{0}.servermaps".FormatWith(newArgs.Environment));
                newArgs.ServerMappings.Merge(_serverParser.Parse(new System.IO.FileInfo(pathToMapFile)));

                _log.Debug("*******SETTINGS*******");
                _log.InfoFormat("Command: {0}", newArgs.Command);
                _log.InfoFormat("Deployment: {0}", newArgs.Deployment);
                _log.InfoFormat("Environment: {0}", newArgs.Environment);
                _log.InfoFormat("Role: {0}", newArgs.Role);
                _log.InfoFormat("ServerMappings: {0}", newArgs.ServerMappings);
                _log.InfoFormat("Settings Path: {0}", pathToSettingsFile);
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
    }
}