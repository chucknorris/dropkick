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

using dropkick.DeploymentModel;
using dropkick.FileSystem;
using dropkick.Tasks;
using dropkick.Tasks.Security.Acl;

namespace dropkick.Configuration.Dsl.Security.ACL
{
    public class ProtoPathRemoveAclInheritanceTask :
        BaseProtoTask
    {
        readonly string _path;

        public ProtoPathRemoveAclInheritanceTask(string path)
        {
            _path = ReplaceTokens(path);
        }

        public override void RegisterRealTasks(PhysicalServer site)
        {
            var path =  PathConverter.Convert(site,_path);

            var task = new RemoveAclsInheritanceTask(path);
            site.AddTask(task);
        }
    }
}