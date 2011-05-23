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

        private string _connectionString;
        readonly string _scriptsLocation;
        readonly string _environmentName;
        private readonly DatabaseRecoveryMode _recoveryMode;
        private readonly RoundhousEMode _roundhouseMode;
        private readonly string _restorePath;
        private readonly string _repositoryPath;
        private readonly string _versionFile;
        private readonly string _versionXPath;

        public RoundhousETask(string connectionString, string scriptsLocation, string environmentName, RoundhousEMode roundhouseMode, DatabaseRecoveryMode recoveryMode, string restorePath, string repositoryPath, string versionFile, string versionXPath)
        {
            _connectionString = connectionString;
            _scriptsLocation = scriptsLocation;
            _environmentName = environmentName;
            _recoveryMode = recoveryMode;
            _roundhouseMode = roundhouseMode;
            _restorePath = restorePath;
            _repositoryPath = repositoryPath;
            _versionFile = versionFile;
            _versionXPath = versionXPath;
        }

        public string Name
        {
            get
            {
                return
                    "Using RoundhousE to deploy to connection '{0}' with scripts folder '{1}'.".FormatWith(
                        _connectionString, _scriptsLocation);
            }
        }

        public DeploymentResult VerifyCanRun()
        {
            var results = new DeploymentResult();
            results.AddNote(Name);

            //check you can connect to the _instancename
            //check that the path _scriptsLocation exists

            return results;
        }

        public DeploymentResult Execute()
        {
            var results = new DeploymentResult();
            var log = new DeploymentLogger(results);
            var scriptsPath = Path.GetFullPath(_scriptsLocation);
            var useSimpleRecovery = _recoveryMode == DatabaseRecoveryMode.Simple ? true : false;

            try
            {
                switch (_roundhouseMode)
                {
                    case RoundhousEMode.Drop:
                        RoundhousEClientApi.Run(log, _connectionString, scriptsPath, _environmentName, true, useSimpleRecovery,_repositoryPath,_versionFile,_versionXPath);
                        break;
                    case RoundhousEMode.Restore:
                        RoundhousEClientApi.Run(log, _connectionString, scriptsPath, _environmentName, false, useSimpleRecovery, _repositoryPath, _versionFile, _versionXPath, true, _restorePath);
                        break;
                    case RoundhousEMode.DropCreate:
                        RoundhousEClientApi.Run(log, _connectionString, @".\", _environmentName, true, useSimpleRecovery, _repositoryPath, _versionFile, _versionXPath);
                        goto case RoundhousEMode.Normal;
                    case RoundhousEMode.Normal:
                        RoundhousEClientApi.Run(log, _connectionString, scriptsPath, _environmentName, false, useSimpleRecovery, _repositoryPath, _versionFile, _versionXPath);
                        break;
                    default:
                        goto case RoundhousEMode.Normal;
                }

            }
            catch (Exception ex)
            {
                results.AddError("An error occured during RoundhousE execution.", ex);
            }

            return results;
        }
    }
}