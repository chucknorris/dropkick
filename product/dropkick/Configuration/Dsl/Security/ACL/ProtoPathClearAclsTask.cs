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
using System;
using System.Collections.Generic;
using dropkick.FileSystem;
using dropkick.Tasks.Security;
using dropkick.Tasks.Security.Acl;

namespace dropkick.Configuration.Dsl.Security.ACL
{
    using DeploymentModel;
    using Tasks;

    public class ProtoPathClearAclsTask :
        BaseProtoTask, ClearAclOptions
    {
        private readonly IList<string> _groupsToPreserve = new List<string>();
        private readonly IList<string> _groupsToRemove = new List<string>();
        string _path;

        public ProtoPathClearAclsTask(string path)
        {
            _path = ReplaceTokens(path);
        }
             
        public ClearAclOptions Preserve(string groupName)
        {
            _groupsToPreserve.Add(groupName);

            return this;
        }

        //public ClearAclOptions PreserveCurrentUser()
        //{
        //    _groupsToPreserve.Add(WellKnownSecurityRoles.CurrentUser);

        //    return this;
        //}

        public ClearAclOptions RemoveSystemAccount()
        {
            _groupsToRemove.Add(WellKnownSecurityRoles.System);
            return this;
        }

        public ClearAclOptions RemoveAdministratorsGroup()
        {
            _groupsToRemove.Add(WellKnownSecurityRoles.Administrators);
            return this;
        }

        public ClearAclOptions RemoveUsersGroup()
        {
            _groupsToRemove.Add(WellKnownSecurityRoles.Users);
            return this;
        }

        public override void RegisterRealTasks(PhysicalServer site)
        {
            var path = PathConverter.Convert(site, _path);

            var task = new ClearAclsTask(path, _groupsToPreserve, _groupsToRemove);
            site.AddTask(task);
        }

  
    }
}