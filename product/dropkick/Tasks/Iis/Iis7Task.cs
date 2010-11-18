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
    using FileSystem;
    using log4net;
    using Microsoft.Web.Administration;
    using System.Linq;

    //http://blogs.msdn.com/carlosag/archive/2006/04/17/MicrosoftWebAdministration.aspx
    public class Iis7Task :
        BaseIisTask
    {
        static readonly ILog _logger = LogManager.GetLogger(typeof (Iis7Task));
        public bool UseClassicPipeline { get; set; }
        Path _path = new DotNetPath();

        public override int VersionNumber
        {
            get { return 7; }
        }

        public Iis7Task()
        {
            ManagedRuntimeVersion = Iis.ManagedRuntimeVersion.V2;
            PathForWebsite = Environment.ExpandEnvironmentVariables(@"%SystemDrive%\inetpub\wwwroot");
        }

        public string PathForWebsite { get; set; }
        public string ManagedRuntimeVersion { get; set; }

        public override DeploymentResult VerifyCanRun()
        {
            var result = new DeploymentResult();

            CheckVersionOfWindowsAndIis(result);

            var iisManager = ServerManager.OpenRemote(ServerName);
            CheckForSiteAndVDirExistance(DoesSiteExist, () => DoesVirtualDirectoryExist(GetSite(iisManager, WebsiteName)), result);
            
            if (UseClassicPipeline)
                result.AddAlert("The Application Pool '{0}' will be set to Classic Pipeline Mode", AppPoolName);

            return result;
        }

        public override DeploymentResult Execute()
        {
            var result = new DeploymentResult();

            var iisManager = ServerManager.OpenRemote(ServerName);

            BuildApplicationPool(iisManager, result);

            if (!DoesSiteExist(result))
            {
                CreateWebSite(iisManager, WebsiteName, result);
            }


            Site site = GetSite(iisManager, WebsiteName);

            if (!DoesVirtualDirectoryExist(site))
            {
                result.AddAlert("'{0}' doesn't exist. creating.", VdirPath);
                CreateVirtualDirectory(site, iisManager);
                result.AddGood("'{0}' was created", VdirPath);
            }


            iisManager.CommitChanges();

            return result;
        }

        public bool DoesSiteExist(DeploymentResult result)
        {
            var iisManager = ServerManager.OpenRemote(ServerName);
            foreach (var site in iisManager.Sites)
            {
                if (site.Name.Equals(WebsiteName))
                {
                    result.AddGood("'{0}' site exists", WebsiteName);
                    return true;
                }
            }

            result.AddAlert("'{0}' site DOES NOT exist", WebsiteName);
            return false;
        }

        void CreateWebSite(ServerManager iisManager, string websiteName, DeploymentResult result)
        {
            if(_path.DirectoryDoesntExist(PathForWebsite))
            {
                _path.CreateDirectory(PathForWebsite);
            }
            
            //TODO: check for port collision?
            var site = iisManager.Sites.Add(websiteName, PathForWebsite, PortForWebsite);           
        }

        

        void BuildApplicationPool(ServerManager iisManager, DeploymentResult result)
        {
            if (!string.IsNullOrEmpty(AppPoolName))
            {
                CreateAppPool(AppPoolName, iisManager, result);
            }
        }

        void CreateVirtualDirectory(Site site, ServerManager mgr)
        {
            Magnum.Guard.AgainstNull(site, "The site argument is null and should not be");
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
                var app = site.Applications.Add(appPath, PathOnServer);
                
                
                if(AppPoolName != null)
                    app.ApplicationPoolName = AppPoolName;
            }

            if(UseClassicPipeline)
            {
                if(appToAdd == null) return;
                //TODO: ideally we wouldn't jack with the default pool either

                var poolName = appToAdd.ApplicationPoolName;
                var pool = mgr.ApplicationPools[poolName];
                pool.ManagedPipelineMode = ManagedPipelineMode.Classic;
            }
                
        }


        void CheckVersionOfWindowsAndIis(DeploymentResult result)
        {
            int shouldBe6 = Environment.OSVersion.Version.Major;
            if (shouldBe6 != 6)
                result.AddAlert("This machine does not have IIS7 on it");
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

            var pool = mgr.ApplicationPools.Add(name);
            pool.ManagedRuntimeVersion = "v4.0";
            result.AddGood("Created app pool '{0}'", name);
        }
    }
}