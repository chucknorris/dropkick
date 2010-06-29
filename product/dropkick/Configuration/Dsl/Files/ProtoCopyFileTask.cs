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
        readonly string _from;
        string _to;
        string _newFileName;

        public ProtoCopyFileTask(object settings, string @from)
        {
            _from = ReplaceTokens(from);
        }

        #region FileCopyOptions Members

        public FileCopyOptions ToDirectory(string destinationPath)
        {
            _to = destinationPath;
            return this;
        }

        public FileCopyOptions RenameTo(string newFileName)
        {
            _newFileName = newFileName;
            return this;
        }

        #endregion

        public override void RegisterRealTasks(PhysicalServer site)
        {
            string to = _to;
            if (!site.IsLocal)
                to = RemotePathHelper.Convert(site.Name, to);


            var o = new CopyFileTask(_from, to, _newFileName, new DotNetPath());
            site.AddTask(o);
        }
    }
}