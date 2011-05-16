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

using roundhouse.environments;
using roundhouse.folders;
using roundhouse.infrastructure.app;
using roundhouse.infrastructure.containers;
using roundhouse.infrastructure.filesystem;
using roundhouse.infrastructure.logging;
using roundhouse.infrastructure.logging.custom;
using roundhouse.migrators;
using roundhouse.resolvers;
using roundhouse.runners;

namespace dropkick.Tasks.RoundhousE
{
    using System.Collections.Generic;
    using log4net;

    public class RoundhousEClientApi
    {
        static string _connectionString;
        static string _scriptsLocation;
        static string _environmentName;
        static bool _dropDatabase;
        static bool _useSimpleRecoveryMode;

        static readonly ILog _logger = LogManager.GetLogger(typeof(RoundhousEClientApi));

        public static void Run(Logger log, string connectionString, string scriptsLocation, string environmentName, bool dropDatabase, bool useSimpleRecoveryMode)
        {
            _connectionString = connectionString;
            _scriptsLocation = scriptsLocation;
            _environmentName = environmentName;
            _dropDatabase = dropDatabase;
            _useSimpleRecoveryMode = useSimpleRecoveryMode;

            var loggers = new List<Logger>();
            loggers.Add(log);
            loggers.Add(new Log4NetLogger(_logger));

            var multiLogger = new MultipleLogger(loggers);
            var config = GetRoundhousEConfiguration(multiLogger);

            //should be wrapped in his api
            ApplicationConfiguraton.set_defaults_if_properties_are_not_set(config);

            //should be wrapped in his api
            ApplicationConfiguraton.build_the_container(config);

            var runner = GetMigrationRunner(config);
            runner.run();
        }

        static ConfigurationPropertyHolder GetRoundhousEConfiguration(Logger log)
        {
            var config = new RoundhousEConfig(log)
                             {
                                ConnectionString = _connectionString,
                                SqlFilesDirectory = _scriptsLocation,
                                EnvironmentName = _environmentName,
                                 Drop = _dropDatabase,
                                 RecoveryModeSimple = _useSimpleRecoveryMode,
                                 Silent = true
                             };
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


    public class RoundhousEConfig : ConfigurationPropertyHolder
    {
        public RoundhousEConfig(Logger log)
        {
            Logger = log;
        }

        public Logger Logger { get; set; }
        public string ServerName { get; set; }
        public string DatabaseName { get; set; }
        public string ConnectionString { get; set; }
        public string ConnectionStringAdmin { get; set; }
        public string SqlFilesDirectory { get; set; }
        public string RepositoryPath { get; set; }
        public string VersionFile { get; set; }
        public string VersionXPath { get; set; }
        public string UpFolderName { get; set; }
        public string DownFolderName { get; set; }
        public string RunFirstAfterUpFolderName { get; set; }
        public string FunctionsFolderName { get; set; }
        public string ViewsFolderName { get; set; }
        public string SprocsFolderName { get; set; }
        public string PermissionsFolderName { get; set; }
        public string SchemaName { get; set; }
        public string VersionTableName { get; set; }
        public string ScriptsRunTableName { get; set; }
        public string ScriptsRunErrorsTableName { get; set; }
        public string EnvironmentName { get; set; }
        public bool Restore { get; set; }
        public string RestoreFromPath { get; set; }
        public string RestoreCustomOptions { get; set; }
        public int RestoreTimeout { get; set; }
        public string CreateDatabaseCustomScript { get; set; }
        public string OutputPath { get; set; }
        public bool WarnOnOneTimeScriptChanges { get; set; }
        public bool Silent { get; set; }
        public string DatabaseType { get; set; }
        public bool Drop { get; set; }
        public bool DoNotCreateDatabase { get; set; }
        public bool WithTransaction { get; set; }
        public bool RecoveryModeSimple { get; set; }
        public bool Debug { get; set; }
        public bool DryRun { get; set; }
        public bool Baseline { get; set; }
        public bool RunAllAnyTimeScripts { get; set; }
    }
}