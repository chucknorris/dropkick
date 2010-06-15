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
    using System.IO;
    using System.Security.AccessControl;
    using FileSystem;

    public class BaseGrantPermissionTask
    {
        protected string Path;
        protected string Group;
        protected DotNetPath _dotNetPath;

        //http://msdn.microsoft.com/en-us/library/system.io.directory.setaccesscontrol.aspx        
        protected void SetDirectorySecurity(FileSystemRights permission)
        {
            DirectorySecurity oldSecurity = Directory.GetAccessControl(Path);
            var newSecurity = new DirectorySecurity();

            newSecurity.SetSecurityDescriptorBinaryForm(oldSecurity.GetSecurityDescriptorBinaryForm());

            newSecurity.AddAccessRule(new FileSystemAccessRule(Group,
                                                               permission,
                                                               InheritanceFlags.ContainerInherit |
                                                               InheritanceFlags.ObjectInherit,
                                                               PropagationFlags.InheritOnly,
                                                               AccessControlType.Allow));
            Directory.SetAccessControl(Path, newSecurity);
        }
    }
}