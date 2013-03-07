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
using System.Text;

namespace dropkick.FileSystem
{
    using System.IO;
    using System.Security.AccessControl;
    using System.Text.RegularExpressions;
    using DeploymentModel;
    using Exceptions;
    using Wmi;

    public class DotNetPath : Path
    {
        #region Path Members

		public string GetPhysicalPath(PhysicalServer site, string path, bool forceLocalPath)
		{
			return GetPhysicalPath(site, path, forceLocalPath, null, null);
		}

        public string GetPhysicalPath(PhysicalServer site, string path, bool forceLocalPath, string userName, string password)
        {
            var standardizedPath = path;
            if (!IsUncPath(standardizedPath))
            {
                standardizedPath = @"\\{0}\{1}".FormatWith(site.Name, standardizedPath);
            }

            if (path.StartsWith("~")) { standardizedPath = standardizedPath.Replace(@"~\", ""); }

            standardizedPath = standardizedPath.Replace(':', '$');

            if (site.IsLocal || forceLocalPath)
            {
                var serviceLocation = standardizedPath;
                var regex = new Regex(@"(?<front>[\\\\]?.+?)\\(?<shareName>[A-Za-z0-9\+\.\~\!\@\#\$\%\^\&\(\)_\-'\{\}\s-[\r\n\f]]+)\\?(?<rest>.*)", RegexOptions.IgnoreCase & RegexOptions.Multiline & RegexOptions.IgnorePatternWhitespace);
                var shareMatch = regex.Match(standardizedPath);
                if (shareMatch.Success)
                {
                    var shareName = shareMatch.Groups["shareName"].Value;
                    serviceLocation = Win32Share.GetLocalPathForShare(site.Name, shareName, userName, password);
                }
                var rest = shareMatch.Groups["rest"].Value;
                standardizedPath = System.IO.Path.Combine(serviceLocation, rest);
            }

            return standardizedPath;
        }

        public static bool IsUncPath(string path)
        {
            return path.StartsWith(@"\\");
        }

        public string Combine(string root, params string[] ex)
        {
            string returnPath = root;

            foreach (string item in ex)
            {
                returnPath = System.IO.Path.Combine(returnPath, item);
            }

            return returnPath;
        }

        public string GetFullPath(string path)
        {
            return System.IO.Path.GetFullPath(path);
        }

        public bool IsFile(string path)
        {
            try
            {
                var fi = new FileInfo(GetFullPath(path));
                return (fi.Attributes & FileAttributes.Directory) != FileAttributes.Directory;
            }
            catch (IOException ex)
            {
                var msg = "Attempted to determine if '{0}' was a file, and encountered the following error.".FormatWith(path);
                throw new DeploymentException(msg, ex);
            }

        }

        public bool IsDirectory(string path)
        {
            try
            {
                var di = new DirectoryInfo(GetFullPath(path));
                return (di.Attributes & FileAttributes.Directory) == FileAttributes.Directory;
            }
            catch (IOException ex)
            {
                if (DirectoryDoesntExist(path))
                {

                    var msg2 = "Attempted to determine if '{0}' was a directory, but found that the path doesn't exist at all".FormatWith(path);
                    throw new DeploymentException(msg2, ex);
                }

                var msg = "Attempted to determine if '{0}' was a directory, and encountered the following error.".FormatWith(path);
                throw new DeploymentException(msg, ex);
            }

        }

        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        public bool DirectoryDoesntExist(string path)
        {
            return !DirectoryExists(path);
        }

        public bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public bool FileDoesntExist(string path)
        {
            return !FileExists(path);
        }

        public string[] GetFiles(string path)
        {
            return System.IO.Directory.GetFiles(path);
        }

        public string GetFileNameWithoutExtension(string file)
        {
            return System.IO.Path.GetFileNameWithoutExtension(file);
        }

        public void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }

        //http://www.west-wind.com/weblog/posts/4072.aspx
        public void SetFileSystemRights(string target, string group, FileSystemRights permission, DeploymentResult r)
        {
            if (!IsDirectory(target) && !IsFile(target))
                return;

            var oldSecurity = Directory.GetAccessControl(target);
            var newSecurity = new DirectorySecurity();

            newSecurity.SetSecurityDescriptorBinaryForm(oldSecurity.GetSecurityDescriptorBinaryForm());

            var accessRule = new FileSystemAccessRule(group,
                                                      permission,
                                                      InheritanceFlags.None,
                                                      PropagationFlags.NoPropagateInherit,
                                                      AccessControlType.Allow);
            bool result;
            newSecurity.ModifyAccessRule(AccessControlModification.Set, accessRule, out result);

            if (!result) r.AddError("Something wrong happened");

            accessRule = new FileSystemAccessRule(group,
                                                  permission,
                                                  InheritanceFlags.ContainerInherit |
                                                  InheritanceFlags.ObjectInherit,
                                                  PropagationFlags.InheritOnly,
                                                  AccessControlType.Allow);

            result = false;
            newSecurity.ModifyAccessRule(AccessControlModification.Add, accessRule, out result);
            if (!result) r.AddError("Something wrong happened");

            Directory.SetAccessControl(target, newSecurity);

            if (result) r.AddGood("Permissions set for '{0}' on folder '{1}'", group, target);

            if (!result) r.AddError("Something wrong happened");
        }

        #endregion
    }
}