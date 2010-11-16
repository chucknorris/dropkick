// Copyright 2007-2010 The Apache Software Foundation.
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace dropkick.Tasks.RoundhousE
{
    using log4net;
    using roundhouse.consoles;
    using roundhouse.environments;
    using roundhouse.folders;
    using roundhouse.infrastructure.app;
    using roundhouse.infrastructure.containers;
    using roundhouse.infrastructure.filesystem;
    using roundhouse.migrators;
    using roundhouse.resolvers;
    using roundhouse.runners;

    public class RoundhousEClientApi
    {
        static string _instanceName;
        static string _databaseName;
        static string _databaseType;
        static string _scriptsLocation;
        static string _environmentName;
        static bool _useSimpleRecoveryMode;

        static readonly ILog _logger = LogManager.GetLogger(typeof (RoundhousEClientApi));

        public static void Run(string instanceName, string databaseName, string databaseType, string scriptsLocation,
                               string environmentName, bool useSimpleRecoveryMode)
        {
            _instanceName = instanceName;
            _databaseName = databaseName;
            _databaseType = databaseType;
            _scriptsLocation = scriptsLocation;
            _environmentName = environmentName;
            _useSimpleRecoveryMode = useSimpleRecoveryMode;

            ConfigurationPropertyHolder config = GetRoundhousEConfiguration();

            //should be wrapped in his api
            ApplicationConfiguraton.set_defaults_if_properties_are_not_set(config);

            //should be wrapped in his api
            ApplicationConfiguraton.build_the_container(config);

            IRunner runner = GetMigrationRunner(config);
            runner.run();
        }

        static ConfigurationPropertyHolder GetRoundhousEConfiguration()
        {
            //roundhouse needs a client api - you may want to make one for him?
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

        static IRunner GetMigrationRunner(ConfigurationPropertyHolder configuration)
        {
            return new RoundhouseMigrationRunner(
                configuration.RepositoryPath,
                Container.get_an_instance_of<Environment>(),
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