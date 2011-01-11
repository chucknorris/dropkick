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
namespace dropkick.Configuration.Dsl.Files
{
    using DeploymentModel;
    using FileSystem;
    using Tasks;
    using Tasks.Files;

    public class ProtoRenameTask :
        BaseProtoTask,
        RenameOptions
    {
        readonly Path _path;
        readonly string _target;
        string _newName;

        public ProtoRenameTask(Path path, string target)
        {
            _path = path;
            _target = ReplaceTokens(target);
        }

        public void To(string name)
        {
            _newName = ReplaceTokens(name);
        }

        public override void RegisterRealTasks(PhysicalServer site)
        {
            string target = _target;
            string newName = _newName;

            var o = new RenameTask(target, newName, new DotNetPath());
            site.AddTask(o);
        }
    }
}