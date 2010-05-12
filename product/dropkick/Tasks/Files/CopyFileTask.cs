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
    using Exceptions;
    using log4net;
    using Path = FileSystem.Path;

    public class CopyFileTask :
        Task
    {
        readonly ILog _fileLog = LogManager.GetLogger("dropkick.filewrite");
        readonly ILog _log = LogManager.GetLogger(typeof (CopyFileTask));
        readonly Path _path;
        string _from;
        string _to;

        public CopyFileTask(string @from, string to, Path path)
        {
            _from = from;
            _to = to;
            _path = path;
        }


        public string Name
        {
            get { return string.Format("Copy '{0}' to '{1}'", _from, _to); }
        }

        public DeploymentResult Execute()
        {
            var result = new DeploymentResult();

            ValidateIsFile(result, _from);
            ValidateIsDirectory(result, _to);

            _from = _path.GetFullPath(_from);
            _to = _path.GetFullPath(_to);


            CopyFile(new FileInfo(_from), new DirectoryInfo(_to));

            result.AddGood(Name);

            return result;
        }


        public DeploymentResult VerifyCanRun()
        {
            var result = new DeploymentResult();

            ValidateIsDirectory(result, _to);
            ValidateIsFile(result, _from);

            _from = _path.GetFullPath(_from);
            _to = _path.GetFullPath(_to);


            if (File.Exists(_from))
            {
                result.AddGood(string.Format("'{0}' exists", _from));
            }
            else
            {
                result.AddError(string.Format("'{0}' doesn't exist", _from));
            }

            return result;
        }

        void ValidateIsFile(DeploymentResult result, string path)
        {
            if (!_path.IsFile(path))
                throw new DeploymentException("'{0}' is not an acceptable path. Must be a directory".FormatWith(_to));
        }

        void ValidateIsDirectory(DeploymentResult result, string path)
        {
            if (!_path.IsDirectory(path))
                throw new DeploymentException("'{0}' is not an acceptable path. Must be a directory".FormatWith(_to));
        }

        void CopyFile(FileInfo source, DirectoryInfo destination)
        {
            if (!destination.Exists)
            {
                destination.Create();
            }

            // Copy file.
            var fileDestination = _path.Combine(destination.FullName, source.Name);

            source.CopyTo(fileDestination);
            _log.DebugFormat("Copy file '{0}' to '{1}'", source.FullName, fileDestination);
            _fileLog.Info(fileDestination); //log where files are copied for tripwire
        }
    }
}