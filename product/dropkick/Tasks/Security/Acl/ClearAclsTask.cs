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
namespace dropkick.Tasks.Security.Acl
{
    using System;
    using System.IO;
    using System.Security.AccessControl;
    using System.Security.Principal;
    using DeploymentModel;
    using Msmq;

    public class ClearAclsTask :
        Task
    {
        readonly string _path;

        public ClearAclsTask(string path)
        {
            _path = path;
        }

        public string Name
        {
            get { return "clear acls"; }
        }

        public DeploymentResult VerifyCanRun()
        {
            throw new NotImplementedException();
        }

        public DeploymentResult Execute()
        {
            var result = new DeploymentResult();

            DirectorySecurity security = Directory.GetAccessControl(_path);
            AuthorizationRuleCollection rules = security.GetAccessRules(true, true, typeof (NTAccount));
            foreach (FileSystemAccessRule rule in rules)
            {
                if (WellKnownRoles.NotADefaultRule(rule) && WellKnownRoles.NotInherited(rule))
                {
                    Console.WriteLine("Removing {0}", rule.IdentityReference);
                    security.RemoveAccessRuleSpecific(rule); // won't remove inherited stuff
                }
            }
            Directory.SetAccessControl(_path, security);

            return result;
        }
    }
}