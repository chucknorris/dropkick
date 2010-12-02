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
    using System.ComponentModel;
    using System.Text;
    using DeploymentModel;

    public class LogOnAsBatchTask :
        BaseSecurityTask
    {
        readonly string _serverName;
        readonly string _userAccount;

        public LogOnAsBatchTask(string serverName, string userAccount)
        {
            _serverName = serverName;
            _userAccount = userAccount;
        }

        public override string Name
        {
            get { return "Give '{0}' Log On As Batch on server '{1}'".FormatWith(_userAccount, _serverName); }
        }

        public override DeploymentResult VerifyCanRun()
        {
            var result = new DeploymentResult();
            VerifyInAdministratorRole(result);
            return result;
        }

        public override DeploymentResult Execute()
        {
            //http://weblogs.asp.net/avnerk/archive/2007/05/10/granting-user-rights-in-c.aspx
            var result = new DeploymentResult();
            try
            {
                using (var lsa = new LsaWrapper())
                {
                    lsa.AddPrivileges(_userAccount, "SeBatchLogonRight");
                }
                LogSecurity("[security][lpo] Grant '{0}' LogOnAsBatch on '{1}'", _userAccount, _serverName);
            }
            catch (Win32Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendFormat("Error while attempting to grant '{0}' the right '{1}'", _userAccount,
                                "SeBatchLogonRight");
                result.AddError(sb.ToString(), ex);
            }

            return result;
        }
    }
}