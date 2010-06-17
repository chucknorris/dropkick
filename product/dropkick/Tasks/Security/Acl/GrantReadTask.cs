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

    public class GrantReadTask : Task
    {
        string _target;
        string _group;
        Path _path;
        
        public GrantReadTask(string target, string @group, Path dnPath)
        {
            _target = target;
            _group = group;
            _path = dnPath;
        }

        public string Name
        {
            get { return "Grant Read permissions to '{0}' for path '{1}'".FormatWith(_group, _target); }
        }

        public DeploymentResult VerifyCanRun()
        {
            var result = new DeploymentResult();

            _target = _path.GetFullPath(_target);

            if (!_path.IsDirectory(_target) && !_path.IsFile(_target))
                result.AddAlert("'{0}' does not exist.".FormatWith(_target));

            return result;
        }

        public DeploymentResult Execute()
        {

            var result = new DeploymentResult();

            _target = _path.GetFullPath(_target);

            if (!_path.SetTargetSecurity(_target, _group, FileSystemRights.ReadAndExecute))
                result.AddAlert("Could not apply Read permissions for '{0}' to '{1}'.".FormatWith(_target, _group));

            result.AddGood(Name);
            return result;
        }
    }
}