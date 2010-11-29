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
namespace dropkick.Wmi
{
    using System;
    using System.Management;

    public static class Win32Share
    {
        public static string GetLocalPathForShare(string serverName, string shareName)
        {
            var query = new WqlObjectQuery("select Name, Path from Win32_Share");
            var scope = new ManagementScope(@"\\{0}\root\cimv2".FormatWith(serverName))
                            {
                                Options =
                                    {
                                        Impersonation = ImpersonationLevel.Impersonate,
                                        EnablePrivileges = true
                                    }
                            };

            using (var search = new ManagementObjectSearcher(scope, query))
            {
                foreach (var share in search.Get())
                {
                    string name = share["Name"].ToString();
                    string path = share["Path"].ToString();

                    if (name.EqualsIgnoreCase(shareName))
                        return path;
                }
            }
            throw new Exception("There is no share '{0}' on machine '{1}'".FormatWith(shareName, serverName));
        }
    }
}