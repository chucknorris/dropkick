// Copyright 2007-2008 The Apache Software Foundation.
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
namespace dropkick.Tasks.Iis
{
    using System;
    using System.IO;
    using DeploymentModel;

    public abstract class BaseIisTask :
        Task
    {
        public bool ShouldCreate { get; protected set; }
        public string WebsiteName { get; set; }
        public string VdirPath { get; set; }
        public DirectoryInfo PathOnServer { get; set; }
        public string ServerName { get; set; }
        public abstract int VersionNumber { get; }

        #region Task Members

        public string Name
        {
            get
            {
                return "IIS{0}: Create vdir '{1}' in site '{2}' on server '{3}'".FormatWith(VersionNumber, VdirPath,
                                                                                            WebsiteName, ServerName);
            }
        }

        public abstract DeploymentResult VerifyCanRun();
        public abstract DeploymentResult Execute();

        #endregion

        public void CheckServerName(DeploymentResult result)
        {
            if (!Environment.MachineName.Equals(ServerName, StringComparison.InvariantCultureIgnoreCase))
            {
                result.AddAlert("You are not on the right server [On: '{0}' - Target: '{1}']", Environment.MachineName,
                                ServerName);
            }
        }

        public void CheckForSiteAndVDirExistance(Func<bool> website, Func<bool> vdir, DeploymentResult result)
        {
            if (website())
            {
                result.AddGood("Found Website '{0}'", WebsiteName);

                if (vdir())
                {
                    result.AddGood("Found VDir '{0}'", VdirPath);
                }
                else
                {
                    result.AddAlert("Couldn't find VDir '{0}'", VdirPath);

                    if (ShouldCreate)
                        result.AddAlert("The VDir '{0}' will be created", VdirPath);
                }
            }
            else
            {
                result.AddAlert("Couldn't find Website '{0}'", WebsiteName);

                if (ShouldCreate)
                    result.AddAlert("Website '{0}' and VDir '{1}' will be created", WebsiteName, VdirPath);
            }
        }
    }
}