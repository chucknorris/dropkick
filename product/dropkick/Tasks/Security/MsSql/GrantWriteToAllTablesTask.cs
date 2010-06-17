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
    using Tasks.MsSql;

    public class GrantWriteToAllTablesTask :
        BaseSecuritySqlTask
    {
        readonly string _role;

        public GrantWriteToAllTablesTask(string server, string database, string role) : base(server, database)
        {
            _role = role;
        }

        public override string Name
        {
            get { return "Grant 'Write to All Tables' in database '{0}' to '{1}'".FormatWith(DatabaseName, _role); }
        }

        public override DeploymentResult VerifyCanRun()
        {
            var result = new DeploymentResult();
            TestConnectivity(result);
            return result;
        }

        public override DeploymentResult Execute()
        {
            var result = new DeploymentResult();

            //how to push this down?
            string sql = @"SELECT [Name] FROM sys.objects WHERE type in (N'U',N'V')";
            var objects = ExecuteSqlWithListReturn<string>(sql);

            //add logging
            foreach (var o in objects)
            {
                GrantSelect(o, _role);
            }

            return result;
        }
    }
}