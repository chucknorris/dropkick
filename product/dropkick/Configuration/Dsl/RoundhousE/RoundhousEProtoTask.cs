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

using dropkick.FileSystem;

namespace dropkick.Configuration.Dsl.RoundhousE
{
    using System;
    using DeploymentModel;
    using Tasks.RoundhousE;
    using Tasks;

    public class RoundhousEProtoTask :
        BaseProtoTask,
        RoundhousEOptions
    {
        private readonly Path _path = new DotNetPath();

        string _environmentName;
        string _instanceName;
        string _databaseName;
        string _scriptsLocation;
        private RoundhousEMode _roundhouseMode;
        private DatabaseRecoveryMode _recoveryMode;
        private string _restorePath;
        private string _restoreCustomOptions;

        private string _userName;
        private string _password;
        private string _repositoryPath;
        private string _versionFile;
        private string _versionXPath;
        private int _commandTimeout;
        private int _restoreTimeout;
        private int _commandTimeoutAdmin;

        private string _functionsFolderName;
        private string _sprocsFolderName;
        private string _viewsFolderName;
        private string _upFolderName;
        private string _versionTable;
        private string _scriptsRunTable;
        private string _scriptsRunErrorTable;
        private bool? _warnOnOneTimeScriptChanges;

        public RoundhousEOptions OnInstance(string name)
        {
            _instanceName = ReplaceTokens(name);
            return this;
        }

        public RoundhousEOptions OnDatabase(string name)
        {
            _databaseName = ReplaceTokens(name);
            return this;
        }

        public RoundhousEOptions WithRoundhousEMode(RoundhousEMode roundhouseMode)
        {
            _roundhouseMode = roundhouseMode;
            return this;
        }

        public RoundhousEOptions WithScriptsFolder(string scriptsLocation)
        {
            _scriptsLocation = ReplaceTokens(scriptsLocation);
            return this;
        }

        public RoundhousEOptions WithFunctionsFolder(string functionsFolderName)
        {
            _functionsFolderName = ReplaceTokens(functionsFolderName);
            return this;
        }

        public RoundhousEOptions WithSprocsFolder(string sprocsFolderName)
        {
            _sprocsFolderName = ReplaceTokens(sprocsFolderName);
            return this;
        }

        public RoundhousEOptions WithViewsFolder(string viewsFolderName)
        {
            _viewsFolderName = ReplaceTokens(viewsFolderName);
            return this;
        }

        public RoundhousEOptions WithUpFolder(string upFolderName)
        {
            _upFolderName = ReplaceTokens(upFolderName);
            return this;
        }

        public RoundhousEOptions WithVersionTable(string versionTable)
        {
            _versionTable = ReplaceTokens(versionTable);
            return this;
        }

        public RoundhousEOptions WithScriptsRunTable(string scriptsRunTable)
        {
            _scriptsRunTable = ReplaceTokens(scriptsRunTable);
            return this;
        }

        public RoundhousEOptions WithScriptsRunErrorTable(string scriptsRunErrorTable)
        {
            _scriptsRunErrorTable = ReplaceTokens(scriptsRunErrorTable);
            return this;
        }

        public RoundhousEOptions ForEnvironment(string environment)
        {
            _environmentName = ReplaceTokens(environment);
            return this;
        }

        public RoundhousEOptions WithDatabaseRecoveryMode(DatabaseRecoveryMode recoveryMode)
        {
            _recoveryMode = recoveryMode;
            return this;
        }

        public RoundhousEOptions WithUserName(string userName)
        {
            _userName = userName;
            return this;
        }

        public RoundhousEOptions WithPassword(string password)
        {
            _password = password;
            return this;
        }

        public RoundhousEOptions WithRestorePath(string restorePath)
        {
            _restorePath = ReplaceTokens(restorePath);
            return this;
        }

        public RoundhousEOptions WithRestoreTimeout(int timeout)
        {
            _restoreTimeout = timeout;
            return this;
        }

        public RoundhousEOptions WithRepositoryPath(string repositoryPath)
        {
            _repositoryPath = ReplaceTokens(repositoryPath);
            return this;
        }

        public RoundhousEOptions WithVersionFile(string versionFile)
        {
            _versionFile = ReplaceTokens(versionFile);
            return this;
        }

        public RoundhousEOptions WithVersionXPath(string versionXPath)
        {
            _versionXPath = versionXPath;
            return this;
        }

        public RoundhousEOptions WithCommandTimeout(int timeout)
        {
            _commandTimeout = timeout;
            return this;
        }

        public RoundhousEOptions WithCommandTimeoutAdmin(int timeout)
        {
            _commandTimeoutAdmin = timeout;
            return this;
        }


        public RoundhousEOptions WithRestoreCustomOptions(string options)
        {
            _restoreCustomOptions = options;
            return this;
        }

        public RoundhousEOptions WarnAndContinueOnOneTimeScriptChanges()
        {
            _warnOnOneTimeScriptChanges = true;
            return this;
        }

        public RoundhousEOptions ErrorOnOneTimeScriptChanges()
        {
            _warnOnOneTimeScriptChanges = false;
            return this;
        }

        public override void RegisterRealTasks(PhysicalServer site)
        {
            // string scriptsLocation = PathConverter.Convert(site, _path.GetFullPath(_scriptsLocation));
            var instanceServer = site.Name;
            if (!string.IsNullOrEmpty(_instanceName))
                instanceServer = @"{0}\{1}".FormatWith(instanceServer, _instanceName);

            var connectionString = BuildConnectionString(instanceServer, _databaseName, _userName, _password);

            var task = new RoundhousETask(connectionString, _scriptsLocation,
                _environmentName, _roundhouseMode,
                _recoveryMode, _restorePath, _restoreTimeout, _restoreCustomOptions,
                _repositoryPath, _versionFile, _versionXPath, _commandTimeout, _commandTimeoutAdmin,
                _functionsFolderName, _sprocsFolderName, _viewsFolderName, _upFolderName,
                _versionTable, _scriptsRunTable, _scriptsRunErrorTable, _warnOnOneTimeScriptChanges);

            site.AddTask(task);
        }

        public string BuildConnectionString(string instanceServer, string databaseName, string userName, string password)
        {
            return "data source={0};initial catalog={1};{2}".FormatWith(instanceServer, databaseName, string.IsNullOrEmpty(userName) ? "integrated security=sspi;" : "user id={0};password={1};".FormatWith(userName, password));
        }
    }
}
