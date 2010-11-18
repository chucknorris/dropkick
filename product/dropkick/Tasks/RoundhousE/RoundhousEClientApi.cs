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
using roundhouse.infrastructure.logging;

namespace dropkick.Tasks.RoundhousE
{
    using System.Collections.Generic;
    using log4net;
    using roundhouse.folders;
    using roundhouse.infrastructure.app;
    using roundhouse.infrastructure.containers;
    using roundhouse.infrastructure.filesystem;
    using roundhouse.infrastructure.logging.custom;
    using roundhouse.migrators;
    using roundhouse.resolvers;
    using roundhouse.runners;
    using Environment = roundhouse.environments.Environment;

    public class RoundhousEClientApi
    {
        static string _instanceName;
        static string _databaseType;
        static string _databaseName;
        static bool _dropDatabase;
        static string _scriptsLocation;
        static string _environmentName;
        static bool _useSimpleRecoveryMode;

        static readonly ILog _logger = LogManager.GetLogger(typeof(RoundhousEClientApi));

        public static void Run(Logger log, string instanceName, string databaseType, string databaseName, bool dropDatabase, string scriptsLocation, string environmentName, bool useSimpleRecoveryMode)
        {
            _instanceName = instanceName;
            _databaseType = databaseType;
            _databaseName = databaseName;
            _dropDatabase = dropDatabase;
            _scriptsLocation = scriptsLocation;
            _environmentName = environmentName;
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
            //roundhouse needs a client api - you may want to make one for him?

            var config = new RoundhousEConfig(log)
                             {
                                 DatabaseName = _databaseName,
                                 ServerName = _instanceName,
                                 DatabaseType = _databaseType,
                                 Drop = _dropDatabase,
                                 SqlFilesDirectory = _scriptsLocation,
                                 EnvironmentName = _environmentName,
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

    public class Log4NetLogger : Logger
    {
        readonly ILog _log;

        public Log4NetLogger(ILog log)
        {
            _log = log;
        }

        public void log_a_debug_event_containing(string message, params object[] args)
        {
            _log.DebugFormat(message, args);
        }

        public void log_an_info_event_containing(string message, params object[] args)
        {
            _log.InfoFormat(message, args);
        }

        public void log_a_warning_event_containing(string message, params object[] args)
        {
            _log.WarnFormat(message, args);
        }

        public void log_an_error_event_containing(string message, params object[] args)
        {
            _log.ErrorFormat(message, args);
        }

        public void log_a_fatal_event_containing(string message, params object[] args)
        {
            _log.FatalFormat(message, args);
        }
    }


    public class RoundhousEConfig : ConfigurationPropertyHolder
    {
        public RoundhousEConfig(Logger log)
        {
            Logger = log;
        }

        public Logger Logger { get; private set; }
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