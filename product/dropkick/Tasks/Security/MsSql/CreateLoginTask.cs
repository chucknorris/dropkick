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
    using System;
    using DeploymentModel;

    public class CreateLoginTask :
        BaseSecuritySqlTask
    {
        readonly string _login;
        readonly string _defaultLanguage;

        public CreateLoginTask(string serverName, string databaseName, string login) : this(serverName, databaseName, login, "us_english")
        {

        }

        public CreateLoginTask(string serverName, string databaseName, string login, string defaultLanguage) : base(serverName, databaseName)
        {
            _login = login;
            _defaultLanguage = defaultLanguage;
        }

        public override string Name
        {
            get { throw new NotImplementedException(); }
        }

        public override DeploymentResult VerifyCanRun()
        {
            throw new NotImplementedException();
        }

        public override DeploymentResult Execute()
        {
            var result = new DeploymentResult();

            var sql = @"CREATE LOGIN [{0}] FROM WINDOWS WITH DEFAULT_DATABASE=[{1}], DEFAULT_LANGUAGE=[{2}]"
                .FormatWith(_login, DatabaseName, _defaultLanguage);

            ExecuteSqlWithNoReturn(sql);

            return result;
        }
    }
}