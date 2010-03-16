// Copyright 2007-2008 The Apache Software Foundation.
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
namespace dropkick.Configuration.Dsl.MsSql
{
    using System;
    using DeploymentModel;
    using Tasks;
    using Tasks.MsSql;

    public class ProtoMsSqlTask :
        BaseTask,
        DatabaseOptions,
        SqlOptions
    {
        string _databaseName;
        string _scriptFile;

        public string InstanceName { get; set; }

        #region DatabaseOptions Members

        public void RunScript(string scriptFile)
        {
            _scriptFile = scriptFile;
        }

        #endregion

        #region SqlOptions Members

        public DatabaseOptions Database(string databaseName)
        {
            _databaseName = databaseName;
            return this;
        }

        #endregion

        public override Task ConstructTasksForServer(DeploymentServer server)
        {
            var t = new RunSqlScriptTask(server.Name, _databaseName)
                    {
                        ScriptToRun = _scriptFile,
                        InstanceName = this.InstanceName
                    };
            return t;
        }
    }
}