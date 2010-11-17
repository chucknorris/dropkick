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
    using DeploymentModel;
    using log4net;

    public class RoundhousETask :
        Task
    {
        static readonly ILog _logger = LogManager.GetLogger(typeof (RoundhousETask));

        readonly string _instanceName;
        readonly string _databaseName;
        readonly string _databaseType;
        readonly string _scriptsLocation;
        readonly string _environmentName;
        readonly bool _useSimpleRecoveryMode;

        public RoundhousETask(string instanceName, string databaseName, string databaseType, string scriptsLocation,
                              string environmentName, bool useSimpleRecoveryMode)
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
            get
            {
                return
                    "Using RoundhousE to deploy the '{0}' database to '{1}' with scripts folder '{2}'.".FormatWith(
                        _databaseName, _instanceName, _scriptsLocation);
            }
        }

        public DeploymentResult VerifyCanRun()
        {
            var results = new DeploymentResult();

            //check you can connect to the _instancename
            //check that the path _scriptsLocation exists
            results.AddNote("I don't know what to do here...");

            return results;
        }

        public DeploymentResult Execute()
        {
            var results = new DeploymentResult();

            var log = new DeploymentLogger(results);

            RoundhousEClientApi.Run(log, _instanceName, _databaseName, _databaseType, _scriptsLocation, _environmentName,
                                    _useSimpleRecoveryMode);


            return results;
        }
    }
}