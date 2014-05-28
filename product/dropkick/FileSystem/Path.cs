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
namespace dropkick.FileSystem
{
    using System.Security.AccessControl;
    using DeploymentModel;

    public interface Path
    {
        string GetPhysicalPath(PhysicalServer server, string path, bool forceLocalPath);
        string Combine(string root, params string[] ex);
        string GetFullPath(string path);
        bool IsFile(string path);
        bool IsDirectory(string path);
        void SetFileSystemRights(string target, string group, FileSystemRights permission, DeploymentResult result);
        bool DirectoryExists(string path);
        bool DirectoryDoesntExist(string path);
        void CreateDirectory(string path);
        bool FileExists(string path);
        bool FileDoesntExist(string path);
        string[] GetFiles(string path);
        string GetFileNameWithoutExtension(string file);
    }
}