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
    using Magnum.Extensions;
    using Path = FileSystem.Path;

    public class CopyFileTask :
        Task
    {
        readonly ILog _fileLog = LogManager.GetLogger("dropkick.filewrite");
        readonly ILog _log = LogManager.GetLogger(typeof (CopyFileTask));
        readonly Path _path;
        string _from;
        string _to;
        readonly string _newFileName;

        public CopyFileTask(string @from, string to, string newFileName, Path path)
        {
            _from = from;
            _to = to;
            _newFileName = newFileName;
            _path = path;
        }


        public string Name
        {
            get { return string.Format("Copy '{0}' to '{1}'", _from, _to); }
        }

        public DeploymentResult VerifyCanRun()
        {
            var result = new DeploymentResult();

            _from = _path.GetFullPath(_from);
            _to = _path.GetFullPath(_to);

            ValidatePaths(result);

            result.AddGood(Name);

            return result;
        }

        public DeploymentResult Execute()
        {
            var result = new DeploymentResult();

            _from = _path.GetFullPath(_from);
            _to = _path.GetFullPath(_to);

            ValidatePaths(result);

            CopyFile(result);

            result.AddGood(Name);

            return result;
        }

        void ValidatePaths(DeploymentResult result)
        {
            ValidateIsFile(result, _from);

            if (_newFileName.IsNotEmpty())
            {
                ValidateIsFile(result, _path.Combine(_to,_newFileName));
            }
            else
            {
                ValidateIsDirectory(result, _to);
            }
        }

        void ValidateIsFile(DeploymentResult result, string path)
        {
            if (!(new FileInfo(_path.GetFullPath(path)).Exists))
                result.AddAlert("'{0}' does not exist.".FormatWith(path));

            if (!_path.IsFile(path))
                result.AddError("'{0}' is not a file.".FormatWith(path));
        }

        void ValidateIsDirectory(DeploymentResult result, string path)
        {
            if (!(new DirectoryInfo(_path.GetFullPath(path)).Exists))
                result.AddAlert("'{0}' does not exist and will be created.".FormatWith(path));

            if (!_path.IsDirectory(path))
                result.AddError("'{0}' is not a directory.".FormatWith(path));
        }

        void CopyFile(DeploymentResult result)
        {
            if (_newFileName.IsNotEmpty())
                CopyFileToFile(result, new FileInfo(_from), new FileInfo(_path.Combine(_to,_newFileName)));
            else
                CopyFileToDirectory(result, new FileInfo(_from), new DirectoryInfo(_to));
        }

        void CopyFileToFile(DeploymentResult result, FileInfo source, FileInfo destination)
        {
            if (destination.Exists)
                result.AddAlert("'{0}' exists, copy will overwrite the existing file.".FormatWith(destination.FullName));

            // Copy file.
            source.CopyTo(destination.FullName, true);
            _log.DebugFormat(Name);
            _fileLog.Info(destination.FullName); //log where files are copied for tripwire
        }

        void CopyFileToDirectory(DeploymentResult result, FileInfo source, DirectoryInfo destination)
        {
            if (!destination.Exists)
                destination.Create();

            // Copy file.
            var fileDestination = _path.Combine(destination.FullName, source.Name);

            if (_path.IsFile(fileDestination))
                result.AddAlert("'{0}' exists, copy will overwrite the existing file.".FormatWith(fileDestination));

            source.CopyTo(fileDestination, true);
            _log.DebugFormat(Name);
            _fileLog.Info(fileDestination); //log where files are copied for tripwire
        }
    }
}