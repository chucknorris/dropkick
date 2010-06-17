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

    public class GrantDataReaderTask :
        BaseSecuritySqlTask
    {
        readonly string _role;

        public GrantDataReaderTask(string serverName, string databaseName, string role)
            : base(serverName, databaseName)
        {
            _role = role;
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
            throw new NotImplementedException();
        }
    }
}