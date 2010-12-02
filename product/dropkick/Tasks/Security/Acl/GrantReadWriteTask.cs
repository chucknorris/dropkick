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
namespace dropkick.Tasks.Security.Acl
{
    using System.Security.AccessControl;
    using DeploymentModel;
    using FileSystem;

    public class GrantReadWriteTask : 
        BaseSecurityTask
    {
        string _target;
        readonly string _group;
        readonly Path _path;
        
        public GrantReadWriteTask(string target, string @group, Path dnPath)
        {
            _target = target;
            _group = group;
            _path = dnPath;
        }

        public override string Name
        {
            get { return "Grant Read/Write permissions to '{0}' for path '{1}'".FormatWith(_group, _target); }
        }

        public override DeploymentResult VerifyCanRun()
        {
            var result = new DeploymentResult();

            _target = _path.GetFullPath(_target);

            if (!_path.DirectoryExists(_target) && !_path.FileExists(_target))
                result.AddAlert("'{0}' does not exist. It may be created.".FormatWith(_target));

            return result;
        }

        public override DeploymentResult Execute()
        {
            var result = new DeploymentResult();

            _target = _path.GetFullPath(_target);

            //TODO: I/O Change
            if (_path.DirectoryDoesntExist(_target)) _path.CreateDirectory(_target);

            _path.SetFileSystemRights(_target, _group, FileSystemRights.Modify, result);
            
            LogSecurity("[security][acl] Granted MODIFY to '{1}' on '{0}'", _target, _group);
                
            return result;
        }

    }
}