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
namespace dropkick.Tasks.Dsn
{
    using System;
    using System.Runtime.InteropServices;
    using System.Threading;
    using DeploymentModel;
    using log4net;

    public class DsnTask :
        Task
    {
        readonly DsnAction _action;
        readonly string _databaseName;
        readonly DsnDriver _driver;
        readonly string _dsnName;
        readonly string _serverName;
        readonly ILog _coarseLog = LogManager.GetLogger("dropkick.coarsegrain");

        public DsnTask(string serverName, string dsnName, DsnAction action, DsnDriver driver, string databaseName)
        {
            _serverName = serverName;
            _databaseName = databaseName;
            _dsnName = dsnName;
            _action = action;
            _driver = driver;
        }

        #region Task Members

        public string Name
        {
            get { return "DSN: {0}".FormatWith(_dsnName); }
        }

        public DeploymentResult VerifyCanRun()
        {
            var result = new DeploymentResult();

            VerifyInAdministratorRole(result);

            return result;
        }

        public DeploymentResult Execute()
        {
            var result = new DeploymentResult();

            try
            {
                bool value = SQLConfigDataSource((IntPtr) 0, (int) _action, _driver.Value,
                                                 "SERVER={0}\0DSN={1}\0DESCRIPTION=NewDSN\0DATABASE={2}\0TRUSTED_CONNECTION=YES"
                                                     .FormatWith(_serverName, _dsnName, _databaseName));
                result.AddGood("Created DSN");
                _coarseLog.InfoFormat("[DSN] Created DSN '{0}'", _dsnName);

            }
            catch (Exception ex)
            {
                result.AddError("Failed to create DSN", ex);
                _coarseLog.ErrorFormat("[DSN] Error when creating DSN '{0}'", _dsnName);
            }


            return result;
        }

        #endregion

        [DllImport("ODBCCP32.dll")]
        static extern bool SQLConfigDataSource(IntPtr parent, int request, string driver, string attributes);

        static void VerifyInAdministratorRole(DeploymentResult result)
        {
            if (Thread.CurrentPrincipal.IsInRole("Administrator"))
            {
                result.AddAlert("You are not in the Administrator role");
            }
            else
            {
                result.AddGood("You are in the Administrator role");
            }
        }
    }
}