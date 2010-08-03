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
namespace dropkick.Configuration.Dsl.MsSql
{
    using DeploymentModel;
    using Tasks;
    using Tasks.MsSql;

    public class ProtoMsSqlTask :
        BaseProtoTask,
        DatabaseOptions,
        SqlOptions
    {
        string _databaseName;
        string _scriptFile;

        public string InstanceName { get; set; }

        public void RunScript(string scriptFile)
        {
            _scriptFile = ReplaceTokens(scriptFile);
        }

        public DatabaseOptions Database(string databaseName)
        {
            _databaseName = ReplaceTokens(databaseName);
            return this;
        }

        public override void RegisterRealTasks(PhysicalServer s)
        {
            s.AddTask(new RunSqlScriptTask(s.Name, _databaseName)
                          {
                              ScriptToRun = _scriptFile,
                              InstanceName = InstanceName
                          });
        }
    }
}