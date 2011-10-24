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
    using System;
    using System.Collections.Generic;
    using DeploymentModel;
    using FileSystem;
    using Tasks;
    using Tasks.Files;
    using System.Text.RegularExpressions;

    public class ProtoCopyDirectoryTask :
        BaseProtoTask,
        CopyOptions,
        FromOptions
    {
        readonly Path _path;
        readonly IList<string> _froms = new List<string>();
        string _to;
        readonly IList<Regex> _copyIgnorePatterns = new List<Regex>();
        DestinationCleanOptions _options = DestinationCleanOptions.None;
        readonly IList<Regex> _clearIgnorePatterns = new List<Regex>();

        public ProtoCopyDirectoryTask(Path path)
        {
            _path = path;
        }

        public CopyOptions To(string destinationPath)
        {
            _to = ReplaceTokens(destinationPath);
            return this;
        }

        public void DeleteDestinationBeforeDeploying()
        {
            _options = DestinationCleanOptions.Delete;
        }

        public void ClearDestinationBeforeDeploying()
        {
            _options = DestinationCleanOptions.Clear;
        }

        public void ClearDestinationBeforeDeploying(Action<IList<Regex>> excludePatterns)
        {
            excludePatterns(_clearIgnorePatterns);
            
            _options = DestinationCleanOptions.Clear;
        }

        public void Include(string path)
        {
            var p = ReplaceTokens(path);
            _froms.Add(p);
        }

        public void Exclude(string pattern)
        {
            Exclude(new Regex(pattern));
        }

        public void Exclude(Regex pattern)
        {
            _copyIgnorePatterns.Add(pattern);
        }

        public CopyOptions From(string sourcePath)
        {
            var p = ReplaceTokens(sourcePath);
            _froms.Add(p);
            return this;
        }

        public override void RegisterRealTasks(PhysicalServer server)
        {
            string to = server.MapPath(_to);

            foreach (var f in _froms)
            {
                var o = new CopyDirectoryTask(f, to, _copyIgnorePatterns, _options, _clearIgnorePatterns, _path);
                server.AddTask(o);
            }
        }
    }
}