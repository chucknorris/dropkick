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
    using System;
    using System.Security.AccessControl;
    using DeploymentModel;
    using FileSystem;

    public class GrantReadWriteTask : BaseGrantPermissionTask, Task
    {
        public GrantReadWriteTask(string path, string @group, DotNetPath dnPath)
        {
            Path = path;
            Group = group;
            _dotNetPath = dnPath;
        }

        public string Name
        {
            get { return "Grant Read/Write permissions to '{0}' for path '{1}'".FormatWith(Group, Path); }
        }

        public DeploymentResult VerifyCanRun()
        {
            var result = new DeploymentResult();

            Path = _dotNetPath.GetFullPath(Path);

            if (!_dotNetPath.IsDirectory(Path) || !_dotNetPath.IsFile(Path))
                result.AddAlert("'{0}' does not exist.".FormatWith(Path));

            return result;
        }

        public DeploymentResult Execute()
        {
            var result = new DeploymentResult();

            Path = _dotNetPath.GetFullPath(Path);

            if (!_dotNetPath.IsDirectory(Path) && !_dotNetPath.IsFile(Path))
                result.AddAlert("'{0}' does not exist.".FormatWith(Path));

            SetDirectorySecurity(FileSystemRights.Modify);

            result.AddGood(Name);
            return result;
        }

    }
}