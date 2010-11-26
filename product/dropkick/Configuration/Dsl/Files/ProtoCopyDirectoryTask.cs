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
    using System.Collections.Generic;
    using DeploymentModel;
    using FileSystem;
    using Tasks;
    using Tasks.Files;

    public class ProtoCopyDirectoryTask :
        BaseProtoTask,
        CopyOptions,
        FromOptions
    {
        readonly IList<string> _froms = new List<string>();
        DestinationCleanOptions _options = DestinationCleanOptions.None;
        string _to;

        public CopyOptions To(string destinationPath)
        {
            _to = ReplaceTokens(destinationPath);
            return this;
        }

        public void DeleteDestinationBeforeDeploying()
        {
            _options = DestinationCleanOptions.Delete;
        }

        public void Include(string path)
        {
            var p = ReplaceTokens(path);
            _froms.Add(p);
        }

        public CopyOptions From(string sourcePath)
        {
            var p = ReplaceTokens(sourcePath);
            _froms.Add(p);
            return this;
        }

        public override void RegisterRealTasks(PhysicalServer site)
        {
            string to = _to;

            if (!site.IsLocal)
                to = RemotePathHelper.Convert(site.Name, to);

            if (to.StartsWith("~"))
                to = RemotePathHelper.Convert(site.Name, to);

            foreach (var f in _froms)
            {
                var o = new CopyDirectoryTask(f, to, _options);
                site.AddTask(o);
            }
        }
    }
}