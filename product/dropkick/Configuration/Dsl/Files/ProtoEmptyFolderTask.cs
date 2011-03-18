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

    public class ProtoEmptyFolderTask :
        BaseProtoTask
    {
        readonly Path _path;
        readonly string _folderName;

        public ProtoEmptyFolderTask(Path path, string folderName)
        {
            _path = path;
            _folderName = ReplaceTokens(folderName);
        }

        public override void RegisterRealTasks(PhysicalServer server)
        {
            string to = server.MapPath(_folderName);

            var task = new EmptyFolderTask(to, new DotNetPath());
            server.AddTask(task);
        }
    }
}