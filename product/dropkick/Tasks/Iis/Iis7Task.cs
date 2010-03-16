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
    using Microsoft.Web.Administration;

    //http://blogs.msdn.com/carlosag/archive/2006/04/17/MicrosoftWebAdministration.aspx
    public class Iis7Task :
        BaseIisTask
    {
        public override int VersionNumber
        {
            get { return 7; }
        }

        public override DeploymentResult VerifyCanRun()
        {
            var result = new DeploymentResult();

            CheckVersionOfWindowsAndIis(result);

            CheckServerName(result);

            CheckForSiteAndVDirExistance(DoesSiteExist, () => DoesVirtualDirectoryExist(GetSite(new ServerManager(), WebsiteName)), result);

            return result;
        }

        public override DeploymentResult Execute()
        {
            var result = new DeploymentResult();

            var iisManager = new ServerManager();

            if (DoesSiteExist())
            {
                result.AddGood("'{0}' site exists", WebsiteName);
                Site site = GetSite(iisManager, WebsiteName);

                if (!DoesVirtualDirectoryExist(site))
                {
                    result.AddAlert("'{0}' doesn't exist. creating.", VdirPath);
                    CreateVirtualDirectory(site);
                    result.AddGood("'{0}' was created", VdirPath);
                }
            }


            iisManager.CommitChanges();

            return result;
        }

        void CreateVirtualDirectory(Site site)
        {
            Application appToAdd = null;
            foreach (var app in site.Applications)
            {
                if (app.Path.Equals("/" + VdirPath))
                {
                    appToAdd = app;
                }
            }

            if (appToAdd == null)
            {
                //create it
                site.Applications.Add("/" + VdirPath, PathOnServer.FullName);
            }
        }


        void CheckVersionOfWindowsAndIis(DeploymentResult result)
        {
            int shouldBe6 = Environment.OSVersion.Version.Major;
            if (shouldBe6 != 6)
                result.AddAlert("This machine does not have IIS7 on it");
        }

        public bool DoesSiteExist()
        {
            var iisManager = new ServerManager();
            foreach (var site in iisManager.Sites)
            {
                if (site.Name.Equals(base.WebsiteName))
                {
                    return true;
                }
            }
            return false;
        }

        public bool DoesVirtualDirectoryExist(Site site)
        {
            foreach (var app in site.Applications)
            {
                if (app.Path.Equals("/" + VdirPath))
                {
                    return true;
                }
            }

            return false;
        }

        public void CreateIfItDoesntExist()
        {
            ShouldCreate = true;
        }

        public Site GetSite(ServerManager iisManager, string name)
        {
            foreach (var site in iisManager.Sites)
            {
                if (site.Name.Equals(name))
                {
                    return site;
                }
            }

            throw new Exception("cant find site");
        }
    }
}