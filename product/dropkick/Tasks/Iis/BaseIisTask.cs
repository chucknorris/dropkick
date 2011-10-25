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
    using DeploymentModel;
    using log4net;

    public abstract class BaseIisTask :
        BaseTask
    {
        ILog _log = LogManager.GetLogger("dropkick.changes.iis");
        public string WebsiteName { get; set; }
        public string VirtualDirectoryPath { get; set; }
        public string PathOnServer { get; set; }
        public string ServerName { get; set; }
        public abstract int VersionNumber { get; }
        public string AppPoolName { get; set; }
        public int PortForWebsite { get; set; }

        protected BaseIisTask()
        {
            PortForWebsite = 80;
        }

        #region Task Members

        public override string Name
        {
            get
            {
                return "IIS{0}: Create vdir '{1}' in site '{2}' on server '{3}'".FormatWith(VersionNumber, VirtualDirectoryPath,
                                                                                            WebsiteName, ServerName);
            }
        }

        #endregion

        public void CheckForSiteAndVDirExistance(Func<DeploymentResult, bool> website, Func<bool> vdir, DeploymentResult result)
        {
            if (website(result))
            {
                result.AddGood("Found Website '{0}'", WebsiteName);

                if (vdir())
                {
                    result.AddGood("Found VirtualDirectory '{0}'", VirtualDirectoryPath);
                }
                else
                {
                    result.AddAlert("Couldn't find VirtualDirectory '{0}'", VirtualDirectoryPath);
                    result.AddAlert("The VirtualDirectory '{0}' will be created", VirtualDirectoryPath);
                }
            }
            else
            {
                result.AddAlert("Couldn't find Website '{0}'", WebsiteName);
                result.AddAlert("Website '{0}' and VirtualDirectory '{1}' will be created", WebsiteName, VirtualDirectoryPath);
            }
        }

        public void LogIis(string format, params object[] args)
        {
            _log.DebugFormat(format, args);
        }
    }
}