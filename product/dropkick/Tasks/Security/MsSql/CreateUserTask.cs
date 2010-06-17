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
namespace dropkick.Tasks.Security.MsSql
{
    using DeploymentModel;

    public class CreateUserTask :
        BaseSecuritySqlTask
    {
        readonly string _user;
        readonly string _login;
        readonly string _defaultSchema;

        public CreateUserTask(string serverName, string databaseName, string user)
            : this(serverName, databaseName, user, user)
        {
        }

        public CreateUserTask(string serverName, string databaseName, string user, string login)
            : this(serverName, databaseName, user, login, "dbo")
        {
        }

        public CreateUserTask(string serverName, string databaseName, string user, string login, string defaultSchema)
            : base(serverName, databaseName)
        {
            _user = user;
            _login = login;
            _defaultSchema = defaultSchema;
        }

        public override string Name
        {
            get { return "Creating User '{0}' in database '{1}'".FormatWith(_user, DatabaseName); }
        }

        public override DeploymentResult VerifyCanRun()
        {
            var result = new DeploymentResult();

            TestConnectivity(result);

            if (!DoesUserExist(_user))
            {
                result.AddAlert("User not found. going to add");
            }

            return result;
        }

        public override DeploymentResult Execute()
        {
            var result = new DeploymentResult();

            if (!DoesUserExist(_user))
            {
                var sql = @"CREATE USER [{0}] FOR LOGIN [{1}] WITH DEFAULT_SCHEMA=[{2}]"
                    .FormatWith(_user, _login, _defaultSchema);
                ExecuteSqlWithNoReturn(sql);
            }

            return result;
        }
    }
}