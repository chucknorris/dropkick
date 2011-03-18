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

    public class ClearAclsTask :
        BaseSecurityTask
    {
        readonly string _path;

        public ClearAclsTask(string path)
        {
            _path = path;
        }

        public override string Name
        {
            get { return "Clear ACLs on '{0}'".FormatWith(_path); }
        }

        public override DeploymentResult VerifyCanRun()
        {
            var result = new DeploymentResult();
            base.VerifyInAdministratorRole(result);
            return result;
        }

        public override DeploymentResult Execute()
        {
            var result = new DeploymentResult();

            var security = Directory.GetAccessControl(_path);
            var rules = security.GetAccessRules(true, true, typeof (NTAccount));
            
            foreach (FileSystemAccessRule rule in rules)
            {
                if (!WellKnownRoles.NotADefaultRule(rule) || !WellKnownRoles.NotInherited(rule)) 
                    continue;

                security.RemoveAccessRuleSpecific(rule); // won't remove inherited stuff
                LogSecurity("[security][acl] Removed '{0}' on '{1}'", rule.IdentityReference, _path);
                result.AddGood("Removed '{0}' on '{1}'", rule.IdentityReference, _path);
            }

            Directory.SetAccessControl(_path, security);

            return result;
        }
    }
}