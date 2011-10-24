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
namespace dropkick.Tasks.Files
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using DeploymentModel;
    using log4net;
    using Path = FileSystem.Path;
    using System.Text.RegularExpressions;

    public class CopyDirectoryTask : BaseIoTask
    {
        readonly ILog _log = LogManager.GetLogger(typeof(CopyDirectoryTask));
        private string _from;
        private string _to;
        private readonly IList<Regex> _copyIgnorePatterns;
        readonly DestinationCleanOptions _clearOptions;
        private readonly IList<Regex> _clearIgnorePatterns;

        public CopyDirectoryTask(string @from, string to, IList<Regex> copyIgnorePatterns, DestinationCleanOptions clearOptions, IList<Regex> clearIgnorePatterns, Path path)
            : base(path)
        {
            _from = from;
            _to = to;
            _copyIgnorePatterns = copyIgnorePatterns;
            _clearOptions = clearOptions;
            _clearIgnorePatterns = clearIgnorePatterns;
        }

        public override string Name
        {
            get { return string.Format("Copy '{0}' to '{1}'", _from, _to); }
        }

        public override DeploymentResult Execute()
        {
            var result = new DeploymentResult();

            ValidatePath(result, _to);
            ValidatePath(result, _from);

            _from = _path.GetFullPath(_from);
            _to = _path.GetFullPath(_to);

            if (_clearOptions == DestinationCleanOptions.Delete) DeleteDestinationFirst(new DirectoryInfo(_to), result);
            if (_clearOptions == DestinationCleanOptions.Clear) CleanDirectoryContents(result, new DirectoryInfo(_to), _clearIgnorePatterns);

            CopyDirectory(result, new DirectoryInfo(_from), new DirectoryInfo(_to), _copyIgnorePatterns);

            result.AddGood(Name);

            return result;
        }

        public override DeploymentResult VerifyCanRun()
        {
            var result = new DeploymentResult();

            ValidatePath(result, _to);
            ValidatePath(result, _from);

            _from = _path.GetFullPath(_from);
            _to = _path.GetFullPath(_to);

            //check can write from _to
            if (_path.DirectoryDoesntExist(_to)) result.AddAlert(string.Format("'{0}' doesn't exist and will be created", _to));

            if (_clearOptions == DestinationCleanOptions.Delete) result.AddAlert("The files and directories in '{0}' will be deleted before deploying, except for items being ignored.", _to);
            if (_clearOptions == DestinationCleanOptions.Clear) result.AddAlert("The files in '{0}' will be cleared before deploying, except for items being ignored.", _to);

            DirectoryInfo fromDirectory = new DirectoryInfo(_from);
            if (fromDirectory.Exists)
            {
                result.AddGood(string.Format("'{0}' exists", fromDirectory.FullName));

                //check can read from _from
                FileInfo[] readFiles = fromDirectory.GetFiles();
                foreach (var file in readFiles.Where(f => !IsIgnored(_copyIgnorePatterns, f)))
                {
                    Stream fs = new MemoryStream();
                    try
                    {
                        fs = File.Open(file.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                        _log.DebugFormat("Going to copy '{0}' to '{1}'", file.FullName, _to);
                    }
                    catch (Exception)
                    {
                        result.AddAlert("CopyDirectoryTask: Can't read file '{0}'");
                    }
                    finally
                    {
                        fs.Dispose();
                    }
                }
            }
            else
            {
                result.AddAlert(string.Format("'{0}' doesn't exist", _from));
            }

            return result;
        }
    }
}