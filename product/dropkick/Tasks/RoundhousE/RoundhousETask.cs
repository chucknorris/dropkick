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
using System.ComponentModel;

namespace dropkick.Tasks.RoundhousE
{
    using System;
    using System.IO;
    using DeploymentModel;
    using log4net;

    public class RoundhousETask :
        Task
    {
        static readonly ILog _logger = LogManager.GetLogger(typeof(RoundhousETask));

        private readonly DbConnectionInfo _connectionInfo;
        readonly string _scriptsLocation;
        readonly string _environmentName;
        private readonly DatabaseRecoveryMode _recoveryMode;
        private readonly RoundhousEMode _roundhouseMode;
        private readonly string _restorePath;
        private readonly int _restoreTimeout;
        private readonly string _restoreCustomOptions;
        private readonly string _repositoryPath;
        private readonly string _versionFile;
        private readonly string _versionXPath;
        private readonly int _commandTimeout;
        private readonly int _commandTimeoutAdmin;
        private readonly string _functionsFolderName;
        private readonly string _sprocsFolderName;
        private readonly string _viewsFolderName;
        private readonly string _upFolderName;
        private readonly string _scriptsRunTable;
        private readonly string _scriptsRunErrorTable;
        private readonly bool? _warnOnOneTimeScriptChanges;
        private readonly string _versionTable;

        public RoundhousETask(DbConnectionInfo connectionInfo, string scriptsLocation, string environmentName, RoundhousEMode roundhouseMode, DatabaseRecoveryMode recoveryMode, string restorePath, int restoreTimeout, string restoreCustomOptions, string repositoryPath, string versionFile, string versionXPath, int commandTimeout, int commandTimeoutAdmin, string functionsFolderName, string sprocsFolderName, string viewsFolderName, string upFolderName, string versionTable, string scriptsRunTable, string scriptsRunErrorTable, bool? warnOnOneTimeScriptChanges)
        {
            _connectionInfo = connectionInfo;
            _scriptsLocation = scriptsLocation;
            _environmentName = environmentName;
            _recoveryMode = recoveryMode;
            _roundhouseMode = roundhouseMode;
            _restorePath = restorePath;
            _restoreTimeout = restoreTimeout;
            _restoreCustomOptions = restoreCustomOptions;
            _repositoryPath = repositoryPath;
            _versionFile = versionFile;
            _versionXPath = versionXPath;
            _commandTimeout = commandTimeout;
            _commandTimeoutAdmin = commandTimeoutAdmin;
            _functionsFolderName = functionsFolderName;
            _sprocsFolderName = sprocsFolderName;
            _viewsFolderName = viewsFolderName;
            _upFolderName = upFolderName;
            _versionTable = versionTable;
            _scriptsRunTable = scriptsRunTable;
            _scriptsRunErrorTable = scriptsRunErrorTable;
            _warnOnOneTimeScriptChanges = warnOnOneTimeScriptChanges;
        }

        public string Name
        {
            get
            {
                return
                    "Using RoundhousE to deploy to database '{0}' on '{1}' instance with scripts folder '{2}'.".FormatWith(
                        _connectionInfo.DatabaseName,
                        _connectionInfo.Instance ?? "(default)",
                        _scriptsLocation);
            }
        }

        public DeploymentResult VerifyCanRun()
        {
            var results = new DeploymentResult();
            results.AddNote(Name);

            if(_connectionInfo.WillPromptForUserName())
                results.AddAlert("We are going to prompt for a username.");

            if (_connectionInfo.WillPromptForPassword())
                results.AddAlert("We are going to prompt for a password.");

            //check you can connect to the _instancename
            //check that the path _scriptsLocation exists

            return results;
        }

        public DeploymentResult Execute()
        {
            var results = new DeploymentResult();
            var scriptsPath = Path.GetFullPath(_scriptsLocation);
            var useSimpleRecovery = _recoveryMode == DatabaseRecoveryMode.Simple ? true : false;

            try
            {
                var connectionString = _connectionInfo.BuildConnectionString();
                switch (_roundhouseMode)
                {
                    case RoundhousEMode.Drop:
                        RoundhousEClientApi.Run(connectionString, scriptsPath, _environmentName, true, useSimpleRecovery, _repositoryPath, _versionFile, _versionXPath, _commandTimeout, _commandTimeoutAdmin, _functionsFolderName, _sprocsFolderName, _viewsFolderName, _upFolderName, _versionTable, _scriptsRunTable, _scriptsRunErrorTable, _warnOnOneTimeScriptChanges);
                        break;
                    case RoundhousEMode.Restore:
                        RoundhousEClientApi.Run(connectionString, scriptsPath, _environmentName, false, useSimpleRecovery, _repositoryPath, _versionFile, _versionXPath, _commandTimeout, _commandTimeoutAdmin, true, _restorePath, _restoreTimeout, _restoreCustomOptions, _functionsFolderName, _sprocsFolderName, _viewsFolderName, _upFolderName, _versionTable, _scriptsRunTable, _scriptsRunErrorTable, _warnOnOneTimeScriptChanges);
                        break;
                    case RoundhousEMode.DropCreate:
                        RoundhousEClientApi.Run(connectionString, @".\", _environmentName, true, useSimpleRecovery, _repositoryPath, _versionFile, _versionXPath, _commandTimeout, _commandTimeoutAdmin, _functionsFolderName, _sprocsFolderName, _viewsFolderName, _upFolderName, _versionTable, _scriptsRunTable, _scriptsRunErrorTable, _warnOnOneTimeScriptChanges);
                        goto case RoundhousEMode.Normal;
                    case RoundhousEMode.Normal:
                        RoundhousEClientApi.Run(connectionString, scriptsPath, _environmentName, false, useSimpleRecovery, _repositoryPath, _versionFile, _versionXPath, _commandTimeout, _commandTimeoutAdmin, _functionsFolderName, _sprocsFolderName, _viewsFolderName, _upFolderName, _versionTable, _scriptsRunTable, _scriptsRunErrorTable, _warnOnOneTimeScriptChanges);
                        break;
                    default:
                        goto case RoundhousEMode.Normal;
                }

                results.AddGood("[roundhouse] Deployed migrations changes successfully");

            }
            catch (Exception ex)
            {
                results.AddError("[roundhouse] An error occured during RoundhousE execution.", ex);
            }

            return results;
        }
    }
}