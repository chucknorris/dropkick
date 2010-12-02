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

    //Reference http://msdn.microsoft.com/en-us/library/aa394435(v=VS.85).aspx

    public static class Win32Share
    {
        const string CLASSNAME = "Win32_Share";

        public static ShareReturnCode Create(string machineName, string shareName, string path, string description)
        {
            try
            {
                const string methodName = "Create";

                //todo: This only creates read only shares
                var parameters = new object[]
                                     {
                                         path, // Path
                                         shareName, // Name
                                         0x0, // Type
                                         null, // MaximumAllowed
                                         description, // Description
                                         null, // Password
                                         null // Win32_SecurityDescriptor Access
                                     };
                return (ShareReturnCode)WmiHelper.InvokeStaticMethod(machineName, CLASSNAME, methodName, parameters);
            }
            catch
            {
                return ShareReturnCode.UnknownFailure;
            }

        }

        public static ShareReturnCode Delete(string serverName, string shareName)
        {
            var query = new WqlObjectQuery("select Name from Win32_Share");
            var scope = WmiHelper.Connect(serverName);

            using (var search = new ManagementObjectSearcher(scope, query))
            {

                foreach (var share in search.Get())
                {
                    string name = share["Name"].ToString();
                    if (name.EqualsIgnoreCase(shareName))
                    {
                        //share.Delete();
                        //return ShareReturnCode.Success;
                        //break;
                    }
                }
            }

            return ShareReturnCode.UnknownFailure;
        }


        public static string GetLocalPathForShare(string serverName, string shareName)
        {
            var query = new WqlObjectQuery("select Name, Path from Win32_Share");
            var scope = WmiHelper.Connect(serverName);

            using (var search = new ManagementObjectSearcher(scope, query))
            {
                foreach (var share in search.Get())
                {
                    string name = share["Name"].ToString();
                    string path = share["Path"].ToString();

                    if (name.EqualsIgnoreCase(shareName)) return path;
                }
            }
            throw new Exception("There is no share '{0}' on machine '{1}'".FormatWith(shareName, serverName));
        }


    }
    //public class test
    //{
    //    public void yep()
    //    {
    //        // SELECT * FROM Win32_LogicalShareSecuritySetting WHERE Name = '" + Share.ToString() + "'"
    //        foreach (ManagementObject m in WmiHelper.Query("localhost", "SELECT * FROM Win32_LogicalShareSecuritySetting WHERE Name = 'code'"))
    //        {
    //            //manObj.ClassPath;
    //            string shareName = @"\\" + m["Name"];

    //            Console.WriteLine(shareName);


    //            InvokeMethodOptions options = new InvokeMethodOptions();
    //            ManagementBaseObject outParamsMthd = m.InvokeMethod("GetSecurityDescriptor", null, options);
    //            ManagementBaseObject descriptor = outParamsMthd["Descriptor"] as ManagementBaseObject;
    //            Console.WriteLine("ControlFlags are {0}".FormatWith(descriptor["ControlFlags"]));
    //            ManagementBaseObject[] dacl = descriptor["DACL"] as ManagementBaseObject[];

    //            foreach (ManagementBaseObject ace in dacl)
    //            {
    //                ManagementBaseObject trustee = ace["Trustee"] as ManagementBaseObject;
    //                uint flags = (uint)ace["AceFlags"];
    //                uint access = (uint)ace["AccessMask"];
    //                string domain = (string)trustee["Domain"];
    //                string name = (string)trustee["Name"];
    //                Console.WriteLine(domain + "\\" + name + " has flag to " + flags + "and access to " + access);
    //                //Everyone

    //            }


    //        }
    //    }
    //}
}