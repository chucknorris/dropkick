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
using System.Text.RegularExpressions;

    public class ProtoCopyDirectoryTask :
        BaseProtoTask,
        CopyOptions,
        FromOptions
    {
        readonly Path _path;
        readonly IList<string> _froms = new List<string>();
        DestinationCleanOptions _options = DestinationCleanOptions.None;
        string _to;
		Regex[] _ignorePatterns = new Regex[] {};

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

		/// <summary>
		/// Clears the target directory before deploying, optionally ignoring some
		/// files in the target directory (e.g. app_offline.htm);
		/// </summary>
		/// <param name="ignorePatterns">The files to ignore in the target directory.</param>
		public void ClearFilesBeforeDeploying(Regex[] ignorePatterns)
		{
			_ignorePatterns = ignorePatterns;
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

        public override void RegisterRealTasks(PhysicalServer server)
        {
            string to = server.MapPath(_to);

            foreach (var f in _froms)
            {
                var o = new CopyDirectoryTask(f, to, _options, _path, _ignorePatterns);
                server.AddTask(o);
            }
        }
    }
}