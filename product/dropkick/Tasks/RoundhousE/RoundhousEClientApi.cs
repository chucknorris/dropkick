﻿// Copyright 2007-2010 The Apache Software Foundation.
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
    using roundhouse;
    using roundhouse.infrastructure.logging;

    public class RoundhousEClientApi
    {
        #region Constants

        private static readonly ILog _logger = LogManager.GetLogger(typeof(RoundhousEClientApi));

        #endregion

        #region Methods

        public static void Run(string connectionString, string scriptsLocation, string environmentName, bool dropDatabase, bool useSimpleRecoveryMode, string repositoryPath, string versionFile, string versionXPath, int commandTimeout, int commandTimeoutAdmin, string functionsFolderName, string sprocsFolderName, string viewsFolderName, string upFolderName, string versionTable, string scriptsRunTable, string scriptsRunErrorTable, bool? warnOnOneTimeScriptChanges, string outputPath)
        {
            Run(connectionString, scriptsLocation, environmentName, dropDatabase, useSimpleRecoveryMode, repositoryPath, versionFile, versionXPath, commandTimeout, commandTimeoutAdmin, false, @"", 0, string.Empty, functionsFolderName, sprocsFolderName, viewsFolderName, upFolderName, versionTable, scriptsRunTable, scriptsRunErrorTable, warnOnOneTimeScriptChanges, outputPath);
        }

        public static void Run(string connectionString, string scriptsLocation, string environmentName, bool dropDatabase, bool useSimpleRecoveryMode, string repositoryPath, string versionFile, string versionXPath,int commmandTimeout,int commandTimeoutAdmin, bool restore, string restorePath,int restoreTimeout, string restoreCustomOptions, string functionsFolderName, string sprocsFolderName, string viewsFolderName, string upFolderName, string versionTable, string scriptsRunTable, string scriptsRunErrorTable, bool? warnOnOneTimeScriptChanges, string outputPath)
        {
            var migrate = new Migrate();

            migrate
                .Set(p =>
                {
                    p.Logger = new roundhouse.infrastructure.logging.custom.Log4NetLogger(Logging.WellKnown.DatabaseChanges);

                    p.ConnectionString = connectionString;
                    p.SqlFilesDirectory = scriptsLocation;
                    p.EnvironmentName = environmentName;
                    p.Drop = dropDatabase;
                    p.RecoveryModeSimple = useSimpleRecoveryMode;
                    p.Restore = restore;
                    p.RestoreFromPath = restorePath;
                    p.RestoreTimeout = restoreTimeout;
                    p.RestoreCustomOptions = restoreCustomOptions;
                    p.RepositoryPath = repositoryPath;
                    p.VersionFile = versionFile;
                    p.VersionXPath = versionXPath;
                    p.CommandTimeout = commmandTimeout;
                    p.CommandTimeoutAdmin = commandTimeoutAdmin;

                    p.FunctionsFolderName = functionsFolderName;
                    p.SprocsFolderName = sprocsFolderName;
                    p.ViewsFolderName = viewsFolderName;
                    p.UpFolderName = upFolderName;

                    p.VersionTableName = versionTable;
                    p.ScriptsRunTableName = scriptsRunTable;
                    p.ScriptsRunErrorsTableName = scriptsRunErrorTable;
                    p.OutputPath = outputPath;

                    if (warnOnOneTimeScriptChanges.HasValue)
                    {
                        p.WarnOnOneTimeScriptChanges = warnOnOneTimeScriptChanges.Value;
                    }

                    p.Silent = true;
                })
                .Run();
        }

        #endregion
    }
}