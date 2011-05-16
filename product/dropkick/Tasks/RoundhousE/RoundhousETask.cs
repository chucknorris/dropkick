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
        readonly bool _dropDatabase;
        readonly bool _useSimpleRecoveryMode;
        
        public RoundhousETask(string connectionString, string scriptsLocation, string environmentName, bool useSimpleRecoveryMode, bool dropDatabase)
        {
            _connectionString = connectionString;
            _scriptsLocation = scriptsLocation;
            _environmentName = environmentName;
            _dropDatabase = dropDatabase;
            
            _useSimpleRecoveryMode = useSimpleRecoveryMode;
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
            results.AddNote("I don't know what to do here...");


            return results;
        }

        public DeploymentResult Execute()
        {
            var results = new DeploymentResult();

            var scriptsPath = Path.GetFullPath(_scriptsLocation);

            var log = new DeploymentLogger(results);
            try
            {
                if (_dropDatabase)
                    RoundhousEClientApi.Run(log, _connectionString,scriptsPath, _environmentName, true,  _useSimpleRecoveryMode);
                RoundhousEClientApi.Run(log,_connectionString,scriptsPath, _environmentName, false, _useSimpleRecoveryMode);
            }
            catch (Exception ex)
            {
                results.AddError("An error occured during RoundhousE execution.", ex);
            }

            return results;
        }
    }
}