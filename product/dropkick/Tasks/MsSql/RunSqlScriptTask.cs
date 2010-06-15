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
namespace dropkick.Tasks.MsSql
{
    using System.Data;
    using System.IO;
    using DeploymentModel;

    public class RunSqlScriptTask :
        BaseSqlTask
    {
        public RunSqlScriptTask(string serverName, string databaseName) : base(serverName, databaseName)
        {
        }

        public override string Name
        {
            get
            {
                return string.Format("Run SqlScritp '{0}' on server '{1}\\{2}' for database '{3}'", ScriptToRun,
                                     ServerName, InstanceName,
                                     DatabaseName);
            }
        }


        public string ScriptToRun { get; set; }

        public string InstanceName { get; set; }


        public override DeploymentResult VerifyCanRun()
        {
            var result = new DeploymentResult();

            TestConnectivity(result);


            if (ScriptToRun != null)
            {
                result.AddAlert(string.Format("I will run the sql script at '{0}'", ScriptToRun));
                if (File.Exists(ScriptToRun))
                {
                    result.AddGood(string.Format("I found the script '{0}'", ScriptToRun));
                }
                else
                {
                    result.AddAlert("I can't find '{0}'", ScriptToRun);
                }
            }

            return result;
        }

        public override DeploymentResult Execute()
        {
            string s = File.ReadAllText(ScriptToRun);

            //TODO: need to use the sql dmo stuff
            ExecuteSqlWithNoReturn(s);

            return new DeploymentResult();
        }
    }
}