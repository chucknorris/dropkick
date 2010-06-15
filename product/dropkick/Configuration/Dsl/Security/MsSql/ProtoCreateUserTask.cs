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
namespace dropkick.Configuration.Dsl.Security.MsSql
{
    using DeploymentModel;
    using Tasks;
    using Tasks.Security.MsSql;

    public class ProtoCreateUserTask :
        BaseProtoTask,
        MsSqlUserOptions
    {
        readonly string _user;
        readonly string _database;
        string _role;

        public ProtoCreateUserTask(string database, string user)
        {
            _database = database;
            _user = user;
        }

        public override void RegisterRealTasks(PhysicalServer site)
        {
            var task = new CreateUserTask(site.Name, _database, _user);
            site.AddTask(task);
        }

        public void PutInRole(string role)
        {
            _role = role;
        }
    }
}