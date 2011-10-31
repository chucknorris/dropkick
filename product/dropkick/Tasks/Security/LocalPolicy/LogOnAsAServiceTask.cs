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

    public class LogOnAsAServiceTask :
        BaseSecurityTask
    {
        readonly PhysicalServer _server;
        readonly string _userAccount;

        public LogOnAsAServiceTask(PhysicalServer server, string userAccount)
        {
            _server = server;
            _userAccount = userAccount;
        }

        public override string Name
        {
            get { return "Give '{0}' Log On As A Service on server '{1}'".FormatWith(_userAccount, _server.Name); }
        }

        public override DeploymentResult VerifyCanRun()
        {
            var r = new DeploymentResult();
            r.AddAlert("NO CHECKS RUN");
            return r;
        }

        public override DeploymentResult Execute()
        {
            //http://weblogs.asp.net/avnerk/archive/2007/05/10/granting-user-rights-in-c.aspx
            var result = new DeploymentResult();

            try
            {
                var serverName = _server.Name;
                if (!_server.IsLocal) serverName = "\\\\{0}".FormatWith(_server.Name);
                LsaUtility.SetRight(serverName, _userAccount, "SeServiceLogonRight");

                //using (var lsa = new LsaWrapper())
                //{
                //    lsa.AddPrivileges(_userAccount, "SeServiceLogonRight");
                //}
                LogSecurity("[security][lpo] Grant '{0}' LogOnAsService on '{1}'", _userAccount, _server.Name);
            }
            catch (Win32Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendFormat("Error while attempting to grant '{0}' the right '{1}'", _userAccount, "SeServiceLogonRight");
                result.AddError(sb.ToString(), ex);
            }


            return result;
        }
    }
}