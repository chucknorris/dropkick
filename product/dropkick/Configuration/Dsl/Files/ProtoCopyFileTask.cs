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

    public class ProtoCopyFileTask :
        BaseProtoTask,
        FileCopyOptions
    {
        readonly Path _path;
        readonly string _from;
        string _to;
        string _newFileName;

        public ProtoCopyFileTask(Path path, string @from)
        {
            _path = path;
            _from = ReplaceTokens(from);
        }

        public FileCopyOptions ToDirectory(string destinationPath)
        {
            _to = ReplaceTokens(destinationPath);
            return this;
        }

        public FileCopyOptions RenameTo(string newFileName)
        {
            _newFileName = ReplaceTokens(newFileName);
            return this;
        }

        public override void RegisterRealTasks(PhysicalServer site)
        {
            string to = PathConverter.Convert(site, _to); 

            var o = new CopyFileTask(_from, to, _newFileName, new DotNetPath());
            site.AddTask(o);
        }
    }
}