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

            checkForSiteBindingConflict(iisManager, websiteName, PortForWebsite);

            var site = iisManager.Sites.Add(websiteName, PathForWebsite, PortForWebsite);
            LogIis("[iis7] Created website '{0}'", WebsiteName);
        }

        static void checkForSiteBindingConflict(ServerManager iisManager, string targetSiteName, int port)
        {
            var conflictSite = iisManager.Sites
                .FirstOrDefault(x =>
                                x.Bindings.Any(b =>
                                               b.EndPoint.Port == port));
            if (conflictSite != null)
                throw new InvalidOperationException(
                    String.Format("Cannot create site '{0}': port '{1}' is already in use by '{2}'.",
                                  targetSiteName, port, conflictSite.Name));
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
        	var application = site.Applications.FirstOrDefault(x => x.Path == appPath);

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

            //result.AddGood("'{0}' was created/updated successfully", VirtualDirectoryPath);
		}

        public bool DoesVirtualDirectoryExist(Site site)
        {
            foreach (var app in site.Applications)
            {
                if (app.Path.Equals("/" + VirtualDirectoryPath))
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
                if (site.Name.Equals(name)) return site;
            }

            throw new ArgumentException("Unable to find site named '{0}'".FormatWith(name));
        }
    }
}