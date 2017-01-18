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
using System.Runtime.InteropServices;
using dropkick.Exceptions;

namespace dropkick.Tasks.Iis
{
    using System;
    using System.Collections.Generic;
    using DeploymentModel;
    using FileSystem;
    using Microsoft.Web.Administration;
    using System.Linq;

    //http://blogs.msdn.com/carlosag/archive/2006/04/17/MicrosoftWebAdministration.aspx
    public class Iis7Task : BaseIisTask
    {
        public bool UseClassicPipeline { get; set; }
        public bool Enable32BitAppOnWin64 { get; set; }
		public bool SetProcessModelIdentity { get; set; }
    	public ProcessModelIdentityType ProcessModelIdentityType { get; set; }
		public string ProcessModelUsername { get; set; }
		public string ProcessModelPassword { get; set; }
      public Dictionary<IISAuthenticationMode, bool> AuthenticationToSet { get; set; }

        readonly Path _path = new DotNetPath();

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

            IisUtility.CheckForIis7(result);

            var iisManager = ServerManager.OpenRemote(ServerName);
            CheckForSiteAndVDirExistance(DoesSiteExist, () => DoesVirtualDirectoryExist(GetSite(iisManager, WebsiteName)), result);

            if (UseClassicPipeline) result.AddAlert("The Application Pool '{0}' will be set to Classic Pipeline Mode", AppPoolName);


            ConfigureAuthentication(iisManager, result, false);

            return result;
        }

        public override DeploymentResult Execute()
        {
            var result = new DeploymentResult();
            var iisManager = ServerManager.OpenRemote(ServerName);
            BuildApplicationPool(iisManager, result);

            if (!DoesSiteExist(result)) CreateWebSite(iisManager, WebsiteName, result);

            Site site = GetSite(iisManager, WebsiteName);
        	BuildVirtualDirectory(site, iisManager, result);

        	try
        	{
				iisManager.CommitChanges();
                result.AddGood("'{0}' was created/updated successfully.", VirtualDirectoryPath);
        	}
        	catch (COMException ex)
        	{
        		if (ProcessModelIdentityType == ProcessModelIdentityType.SpecificUser) throw new DeploymentException("An exception occurred trying to apply deployment changes. If you are attempting to set the IIS " +
						"Process Model's identity to a specific user then ensure that you are running DropKick with elevated privileges, or UAC is disabled.", ex);
        		throw;
        	}
        	LogCoarseGrain("[iis7] {0}", Name);
            
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
            if (_path.DirectoryDoesntExist(PathForWebsite))
            {
                _path.CreateDirectory(PathForWebsite);
                LogFineGrain("[iis7] Created '{0}'", PathForWebsite);
            }

            //TODO: check for port collision?
            var site = iisManager.Sites.Add(websiteName, PathForWebsite, PortForWebsite);
            LogIis("[iis7] Created website '{0}'", WebsiteName);
        }

        void BuildApplicationPool(ServerManager mgr, DeploymentResult result)
        {
            if (string.IsNullOrEmpty(AppPoolName)) return;

        	ApplicationPool pool = mgr.ApplicationPools.FirstOrDefault(x => x.Name == AppPoolName);
			if (pool == null)
			{
				LogIis("App pool '{0}' does not exist, creating.", AppPoolName);
				pool = mgr.ApplicationPools.Add(AppPoolName);
			}
			else
            {
                LogIis("[iis7] Found the AppPool '{0}', updating as necessary.", AppPoolName);
            }

			if (Enable32BitAppOnWin64)
			{
				pool.Enable32BitAppOnWin64 = true;
				LogIis("[iis7] Enabling 32bit application on Win64.");
			}

        	pool.ManagedRuntimeVersion = ManagedRuntimeVersion;
			LogIis("[iis7] Using managed runtime version '{0}'", ManagedRuntimeVersion);

			if (UseClassicPipeline)
			{
				pool.ManagedPipelineMode = ManagedPipelineMode.Classic;
				LogIis("[iis7] Using Classic managed pipeline mode.");
			}

            result.AddGood("App pool '{0}' created/updated.", AppPoolName);

			if (SetProcessModelIdentity)
			{
				SetApplicationPoolIdentity(pool);
				result.AddGood("Set process model identity '{0}'", ProcessModelIdentityType);
			}
			
			//pool.Recycle();
        }

		void SetApplicationPoolIdentity(ApplicationPool pool)
		{
			if (ProcessModelIdentityType != ProcessModelIdentityType.SpecificUser && pool.ProcessModel.IdentityType == ProcessModelIdentityType)
				return;

			pool.ProcessModel.IdentityType = ProcessModelIdentityType;
			var identityUsername = ProcessModelIdentityType.ToString();
			if (ProcessModelIdentityType == ProcessModelIdentityType.SpecificUser)
			{
				pool.ProcessModel.UserName = ProcessModelUsername;
				pool.ProcessModel.Password = ProcessModelPassword;
				identityUsername = ProcessModelUsername;
			}
			LogIis("[iis7] Set process model identity '{0}'", identityUsername);
		}

        void BuildVirtualDirectory(Site site, ServerManager mgr, DeploymentResult result)
        {
            Magnum.Guard.AgainstNull(site, "The site argument is null and should not be");
            var appPath = "/" + VirtualDirectoryPath;

            //this didn't find the application if there is a difference in letter casing like '/MyApplication' and '/Myapplication'. But threw an exception when tried to add it.
        	   //var application = site.Applications.FirstOrDefault(x => x.Path == appPath);
            var application = site.Applications.FirstOrDefault(x => x.Path.Equals(appPath, StringComparison.OrdinalIgnoreCase));

			if (application == null)
			{
				result.AddAlert("'{0}' doesn't exist. creating.", VirtualDirectoryPath);
				application = site.Applications.Add(appPath, PathOnServer);
				LogFineGrain("[iis7] Created application '{0}'", VirtualDirectoryPath);
			}
			else
			{
				result.AddNote("'{0}' already exists. Updating settings.", VirtualDirectoryPath);
			}

			if (application.ApplicationPoolName != AppPoolName)
			{
				application.ApplicationPoolName = AppPoolName;
				LogFineGrain("[iis7] Set the ApplicationPool for '{0}' to '{1}'", VirtualDirectoryPath, AppPoolName);
			}

			var vdir = application.VirtualDirectories["/"];
			if (vdir.PhysicalPath != PathOnServer)
			{
				vdir.PhysicalPath = PathOnServer;
				LogFineGrain("[iis7] Updated physical path for '{0}' to '{1}'", VirtualDirectoryPath, PathOnServer);
			}

         ConfigureAuthentication(mgr, result, true);
            //result.AddGood("'{0}' was created/updated successfully", VirtualDirectoryPath);
		}

       /// <summary>
       /// 
       /// </summary>
       /// <param name="mgr"></param>
       /// <param name="result"></param>
       /// <param name="doUpdate">true: do the update; false: just verify it</param>
        private void ConfigureAuthentication(ServerManager mgr, DeploymentResult result, bool doUpdate) {
           if(AuthenticationToSet != null && AuthenticationToSet.Count > 0) {
              Configuration config = mgr.GetApplicationHostConfiguration();
              foreach(var item in AuthenticationToSet) {
                 ConfigurationSection section = config.GetSection("system.webServer/security/authentication/" + item.Key, WebsiteName + "/" + VirtualDirectoryPath);// settings.WFSiteName + "/" + settings.WFDirectoryName
                 if(section == null) { result.AddError(String.Format(@"authentication type '{0}' not found!", item.Key)); } else {
                    if(doUpdate) {
                       LogCoarseGrain("[iis7] Setting authentication for application '{0}': '{1}' from '{2}' to '{3}'", VirtualDirectoryPath, item.Key, section["enabled"], item.Value);
                       section["enabled"] = item.Value;
                    }
                 }
              }
           }
        }

        public bool DoesVirtualDirectoryExist(Site site)
        {
           return site.Applications.Any(x => x.Path.Equals(VirtualDirectoryPath, StringComparison.OrdinalIgnoreCase));            
        }

        public Site GetSite(ServerManager iisManager, string name)
        {
            foreach (var site in iisManager.Sites)
            {
                if (site.Name.Equals(name)) return site;
            }

            throw new Exception("Unable to find site named '{0}'".FormatWith(name));
        }
    }
}