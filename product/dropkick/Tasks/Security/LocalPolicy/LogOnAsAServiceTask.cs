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
namespace dropkick.Tasks.Security.LocalPolicy
{
    using System;
    using DeploymentModel;
    using Willys.LsaSecurity;

    public class LogOnAsAServiceTask :
        Task
    {
        readonly string _serverName;
        readonly string _userAccount;

        public LogOnAsAServiceTask(string serverName, string userAccount)
        {
            _serverName = serverName;
            _userAccount = userAccount;
        }

        public string Name
        {
            get { return "Give '{0}' Log On As A Service on server '{1}'".FormatWith(_userAccount, _serverName); }
        }

        public DeploymentResult VerifyCanRun()
        {
            throw new NotImplementedException();
        }

        public DeploymentResult Execute()
        {
            //http://weblogs.asp.net/avnerk/archive/2007/05/10/granting-user-rights-in-c.aspx
            var result = new DeploymentResult();

            using (var lsa = new LsaWrapper())
            {
                lsa.AddPrivileges(_userAccount, "SeServiceLogonRight");
            }

            return result;
        }
    }
}