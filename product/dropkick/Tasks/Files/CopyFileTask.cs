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
    using DeploymentModel;
    using log4net;
    using Path = FileSystem.Path;

    public class CopyFileTask :
        BaseIoTask
    {
        readonly ILog _log = LogManager.GetLogger(typeof (CopyFileTask));
        string _from;
        string _to;
        readonly string _newFileName;

        public CopyFileTask(string @from, string to, string newFileName, Path path) : base(path)
        {
            _from = @from;
            _to = to;
            _newFileName = newFileName;
        }


        public override string Name
        {
            get { return string.Format("Copy '{0}' to '{1}'", _from, _to); }
        }

        public override DeploymentResult VerifyCanRun()
        {
            var result = new DeploymentResult();

            _from = _path.GetFullPath(_from);
            _to = _path.GetFullPath(_to);

            ValidatePaths(result);

            result.AddGood(Name);

            return result;
        }

        public override DeploymentResult Execute()
        {
            var result = new DeploymentResult();

            _from = _path.GetFullPath(_from);
            _to = _path.GetFullPath(_to);

            ValidatePaths(result);

            CopyFile(result, _newFileName, _from, _to);

            result.AddGood(Name);

            return result;
        }

        void ValidatePaths(DeploymentResult result)
        {
            ValidateIsFile(result, _from);
            ValidateIsDirectory(result, _to);
        }
    }
}