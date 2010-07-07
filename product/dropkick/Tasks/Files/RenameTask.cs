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
    using System.IO;
    using DeploymentModel;
    using log4net;
    using Path=dropkick.FileSystem.Path;

    public class RenameTask :
        Task
    {
        string _target;
        string _newName;
        readonly Path _path;
        readonly ILog _log = LogManager.GetLogger(typeof (CopyFileTask));
        readonly ILog _fileLog = LogManager.GetLogger("dropkick.filewrite");

        public RenameTask(string target, string newName, Path path)
        {
            _target = target;
            _newName = newName;
            _path = path;
        }

        public string Name
        {
            get { return @"Rename '{0}' to '{1}'.".FormatWith(_target, _newName); }
        }

        public DeploymentResult VerifyCanRun()
        {
            var result = new DeploymentResult();

            ValidateFile(result, _target);

            if (File.Exists(_target))
            {
                result.AddGood(string.Format("'{0}' exists", _target));
            }
            else
            {
                result.AddError(string.Format("'{0}' doesn't exist", _target));
            }

            return result;
        }

        public DeploymentResult Execute()
        {
            var result = new DeploymentResult();

            ValidateFile(result, _target);


            RenameFile(new FileInfo(_target), new FileInfo(_newName));

            result.AddGood(Name);

            return result;
        }

        void RenameFile(FileInfo source, FileInfo destination)
        {
            string path = _path.Combine(source.Directory.FullName, destination.Name);

            if (File.Exists(path))
                File.Delete(path);

            source.MoveTo(path);
            _log.DebugFormat("Rename file '{0}' destination '{1}'", source.FullName, destination.FullName);
            _fileLog.Info(destination); //log where files are copied for tripwire
        }

        void ValidateFile(DeploymentResult result, string path)
        {
            if (!_path.IsFile(path))
                result.AddError("'{0}' is not an acceptable path. It doesn't appear to be a file.".FormatWith(path));
        }
    }
}