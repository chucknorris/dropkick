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
    using Microsoft.Web.Administration;
    using System.Linq;

    //http://blogs.msdn.com/carlosag/archive/2006/04/17/MicrosoftWebAdministration.aspx
    public class Iis7Task :
        BaseIisTask
    {
        static readonly ILog _logger = LogManager.GetLogger(typeof (Iis7Task));

        public override int VersionNumber
        {
            get { return 7; }
        }


        public override DeploymentResult VerifyCanRun()
        {
            var result = new DeploymentResult();

            CheckVersionOfWindowsAndIis(result);

            CheckServerName(result);

            var iisManager = ServerManager.OpenRemote(ServerName);
            CheckForSiteAndVDirExistance(DoesSiteExist, () => DoesVirtualDirectoryExist(GetSite(iisManager, WebsiteName)), result);

            return result;
        }

        public override DeploymentResult Execute()
        {
            var result = new DeploymentResult();

            var iisManager = ServerManager.OpenRemote(ServerName);

            BuildApplicationPool(iisManager, result);

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

        void BuildApplicationPool(ServerManager iisManager, DeploymentResult result)
        {
            if (string.IsNullOrEmpty(AppPoolName))
            {
                CreateAppPool(AppPoolName, iisManager, result);
            }
        }

        void CreateVirtualDirectory(Site site)
        {
            Magnum.Guard.Against.Null(site, "The site argument is null and should not be");
            var appPath = "/" + VdirPath;

            Application appToAdd = null;
            foreach (var app in site.Applications)
            {
                if (app.Path.Equals(appPath))
                {
                    appToAdd = app;
                    _logger.Debug("Found the app {0} and will not create it".FormatWith(VdirPath));
                    break;
                }
            }

            if (appToAdd == null)
            {
                _logger.Info("Application was not found, creating.");
                //create it
                _logger.Debug(PathOnServer);
                var app = site.Applications.Add(appPath, PathOnServer.FullName);
                
                if(AppPoolName != null)
                    app.ApplicationPoolName = AppPoolName;
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
            var iisManager = ServerManager.OpenRemote(ServerName);
            foreach (var site in iisManager.Sites)
            {
                if (site.Name.Equals(WebsiteName))
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

        public Site GetSite(ServerManager iisManager, string name)
        {
            foreach (var site in iisManager.Sites)
            {
                if (site.Name.Equals(name))
                {
                    return site;
                }
            }

            throw new Exception("cant find site '{0}'".FormatWith(name));
        }

        public void CreateAppPool(string name, ServerManager mgr, DeploymentResult result)
        {
            if (mgr.ApplicationPools.Any(x => x.Name == name))
            {
                result.AddAlert("Found the AppPool '{0}'", name);
                return;
            }
            result.AddAlert("Didn't find the AppPool '{0}' creating.", name);    

            mgr.ApplicationPools.Add(name);
            result.AddGood("Created app pool '{0}'", name);
        }
    }
}