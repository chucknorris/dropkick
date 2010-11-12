using log4net;
using roundhouse.consoles;
using roundhouse.infrastructure.app;
using roundhouse.folders;
using roundhouse.infrastructure.containers;
using roundhouse.infrastructure.filesystem;
using roundhouse.migrators;
using roundhouse.resolvers;
using roundhouse.runners;

namespace dropkick.Tasks.RoundhousE
{
    using System;
    using DeploymentModel;

    public class RoundhousETask :
        Task
    {
        private static readonly log4net.ILog _logger = LogManager.GetLogger(typeof (RoundhousETask));
        
        private readonly string _instanceName;
        private readonly string _databaseName;
        private readonly string _databaseType;
        private readonly string _scriptsLocation;
        private readonly string _environmentName;
        private readonly bool _useSimpleRecoveryMode;

        public RoundhousETask(string instanceName, string databaseName, string databaseType, string scriptsLocation, string environmentName, bool useSimpleRecoveryMode)
        {
            _instanceName = instanceName;
            _databaseName = databaseName;
            _databaseType = databaseType;
            _scriptsLocation = scriptsLocation;
            _environmentName = environmentName;
            _useSimpleRecoveryMode = useSimpleRecoveryMode;
        }

        public string Name
        {
            get { return "Using RoundhousE to deploy the '{0}' database to '{1}' with scripts folder '{2}'.".FormatWith(_databaseName,_instanceName,_scriptsLocation); }
        }

        public DeploymentResult VerifyCanRun()
        {
            var results = new DeploymentResult();

            results.AddNote("I don't know what to do here...");

            return results;
        }

        public DeploymentResult Execute()
        {
            var results = new DeploymentResult();

            var config = GetRoundhousEConfiguration();
            ApplicationConfiguraton.set_defaults_if_properties_are_not_set(config);
            ApplicationConfiguraton.build_the_container(config);

            var runner = GetMigrationRunner(config);
            runner.run();

            return results;
        }

        private ConfigurationPropertyHolder GetRoundhousEConfiguration()
        {
            var config = new ConsoleConfiguration(_logger);

            config.DatabaseName = _databaseName;
            config.ServerName = _instanceName;
            config.DatabaseType = _databaseType;
            config.SqlFilesDirectory = _scriptsLocation;
            config.EnvironmentName = _environmentName;
            config.RecoveryModeSimple = _useSimpleRecoveryMode;

            config.Silent = true;
            return config;
        }


        private IRunner GetMigrationRunner(ConfigurationPropertyHolder configuration)
        {
            return new RoundhouseMigrationRunner(
                 configuration.RepositoryPath,
                 Container.get_an_instance_of<roundhouse.environments.Environment>(),
                 Container.get_an_instance_of<KnownFolders>(),
                 Container.get_an_instance_of<FileSystemAccess>(),
                 Container.get_an_instance_of<DatabaseMigrator>(),
                 Container.get_an_instance_of<VersionResolver>(),
                 configuration.Silent,
                 configuration.Drop,
                 configuration.DoNotCreateDatabase,
                 configuration.WithTransaction,
                 configuration.RecoveryModeSimple);
        }

    }
}