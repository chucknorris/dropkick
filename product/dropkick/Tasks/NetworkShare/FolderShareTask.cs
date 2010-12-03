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
namespace dropkick.Tasks.NetworkShare
{
    using System;
    using System.IO;
    using System.Management;
    using DeploymentModel;
    using Wmi;

    public class FolderShareTask :
        Task
    {
        public string Server { get; set; }
        public string Description { get; set; }
        public string ShareName { get; set; }
        public string PointingTo { get; set; }


        public DeploymentResult VerifyCanRun()
        {
            //verify admin
            var result = new DeploymentResult();

            if (!Directory.Exists(PointingTo))
                result.AddAlert("'{0}' doesn't exist", PointingTo);
            else
                result.AddGood("'{0}' exists", PointingTo);


            return result;
        }

        public DeploymentResult Execute()
        {
            var result = new DeploymentResult();

            ShareReturnCode returnCode = Win32Share.Create(Server, ShareName, PointingTo, Description);

            if (returnCode != ShareReturnCode.Success)
            {
                throw new Exception("Unable to share directory '{0}' as '{2}' on '{1}'.".FormatWith(PointingTo, Server, ShareName));
            }

            result.AddGood("Created share");

            return result;
        }

        public string Name
        {
            get { return "Share Folder '{0}' as '{1}' on '{2}'".FormatWith(PointingTo, ShareName, Server); }
        }
    }
}