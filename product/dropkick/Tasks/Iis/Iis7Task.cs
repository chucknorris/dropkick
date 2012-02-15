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

using System.Collections.Generic;
using System.Runtime.InteropServices;
using dropkick.Exceptions;
using dropkick.Tasks.Security.Certificate;

namespace dropkick.Tasks.Iis
{
    using System;
    using DeploymentModel;
    using FileSystem;
    using Microsoft.Web.Administration;
    using System.Linq;

    //http://blogs.msdn.com/carlosag/archive/2006/04/17/MicrosoftWebAdministration.aspx
    public class Iis7Task : BaseIisTask
    {
        const string DefaultAppPoolName = "DefaultAppPool";
        const string DefaultManagedRuntimeVersion = Iis.ManagedRuntimeVersion.V2;

        public bool UseClassicPipeline { get; set; }
        public bool Enable32BitAppOnWin64 { get; set; }
		public bool SetProcessModelIdentity { get; set; }
    	public ProcessModelIdentityType ProcessModelIdentityType { get; set; }
		public string ProcessModelUsername { get; set; }
		public string ProcessModelPassword { get; set; }
        public string ManagedRuntimeVersion { get; set; }

        readonly Path _path = new DotNetPath();
        CertificateStore _certificateStore;

        public override int VersionNumber
        {
            get { return 7; }
        }

        public Iis7Task()
        {
            ManagedRuntimeVersion = DefaultManagedRuntimeVersion;
            PathOnServer = DefaultPathOnServer;
            AppPoolName = DefaultAppPoolName;
        }

        public override DeploymentResult VerifyCanRun()
        {
            var result = new DeploymentResult();

            IisUtility.CheckForIis7(result);

            setupCertificateStore();
            using (var iisManager = ServerManager.OpenRemote(ServerName))
            {
                CheckForSiteAndVDirExistance(DoesSiteExist,
                                             () => DoesVirtualDirectoryExist(GetSite(iisManager, WebsiteName)), result);
                checkForSiteBindingConflict(iisManager, WebsiteName, Bindings, result);
                if (Bindings != null)
                    checkCertificatesExist(Bindings.Where(x => 
                        !String.IsNullOrEmpty(x.CertificateThumbPrint)).Select(x => x.CertificateThumbPrint), result);
                checkHttpsBindingsHaveCertificate(result);

                if (UseClassicPipeline) result.AddAlert("The Application Pool '{0}' will be set to Classic Pipeline Mode", AppPoolName);
            }

            return result;
        }

        void setupCertificateStore()
        {
            _certificateStore = new CertificateStore(ServerName);
        }

        void checkHttpsBindingsHaveCertificate(DeploymentResult result)
        {
            if (Bindings == null) return;

            foreach (var invalidBinding in Bindings.Where(x => x.Protocol == "https" && String.IsNullOrEmpty(x.CertificateThumbPrint)))
                result.AddError("Cannot bind https to port '{0}' as no certificate thumbprint was specified.".FormatWith(invalidBinding.Port));
        }

        void checkCertificatesExist(IEnumerable<string> certificateThumbprints, DeploymentResult result)
        {
            foreach (var thumbprint in certificateThumbprints.Where(thumbprint => !_certificateStore.CertificateExists(thumbprint))) 
                result.AddError("No certificate found with thumbprint '{0}'".FormatWith(thumbprint));
        }

        public override DeploymentResult Execute()
        {
            var result = new DeploymentResult();
            setupCertificateStore();
            using (var iisManager = ServerManager.OpenRemote(ServerName))
            {
                buildApplicationPool(iisManager, result);

                if (!DoesSiteExist(result)) createWebSite(iisManager, WebsiteName);

                var site = GetSite(iisManager, WebsiteName);
                updateSiteBindings(site, result);
                buildVirtualDirectory(site, result);

                try
                {
                    iisManager.CommitChanges();
                    result.AddGood("Virtual Directory '{0}' was created/updated successfully.",
                                   VirtualDirectoryPath ?? "/");
                }
                catch (COMException ex)
                {
                    if (ProcessModelIdentityType == ProcessModelIdentityType.SpecificUser)
                        throw new DeploymentException(
                            "An exception occurred trying to apply deployment changes. If you are attempting to set the IIS " +
                                "Process Model's identity to a specific user then ensure that you are running DropKick with elevated privileges, or UAC is disabled.",
                            ex);
                    throw;
                }
                LogCoarseGrain("[iis7] {0}", Name);
            }
            return result;
        }

        void updateSiteBindings(Site site, DeploymentResult result)
        {
            if (Bindings == null)
            {
                result.AddNote("No site bindings specified.");
                return;
            }

            // https bindings certificates cannot be compared or updated, these bindings must be recreated each time.
            var httpsBindings = site.Bindings.Where(x => x.Protocol == "https").ToArray();
            foreach (var httpsBinding in httpsBindings)
            {
                site.Bindings.Remove(httpsBinding);
                LogIis("[iis7] Removed binding for {0}://{1}:{2}", httpsBinding.Protocol, httpsBinding.EndPoint.Address, httpsBinding.EndPoint.Port);
            }

            // Add new bindings
            var existingBindings = site.Bindings.AsEnumerable();
            foreach (var newBinding in Bindings)
            {
                var bindingInformation = getBindingInformation(newBinding);
                if (!existingBindings.Any(x => x.Protocol == newBinding.Protocol && x.BindingInformation == bindingInformation))
                {
                    if (newBinding.Protocol != "https") 
                        site.Bindings.Add(getBindingInformation(newBinding), newBinding.Protocol);
                    else
                        site.Bindings.Add(getBindingInformation(newBinding), getCertificateHashFor(newBinding), "MY"); // TODO, should "MY" by "\\machine\MY"?
                    LogIis("[iis7] Added binding for {0}://*:{1}", newBinding.Protocol, newBinding.Port);
                }
            }

            for (var i = site.Bindings.Count - 1; i >= 0; i--)
            {
                var existingBinding = site.Bindings[i];
                if (Bindings.Any(x => x.Protocol == existingBinding.Protocol && x.Port == existingBinding.EndPoint.Port)) 
                    continue;

                site.Bindings.Remove(existingBinding);
                LogIis("[iis7] Removed binding for {0}://*:{1}", existingBinding.Protocol, existingBinding.EndPoint.Port);
            }

            result.AddGood("Updated bindings for website '{0}'", WebsiteName);            
        }

        public bool DoesSiteExist(DeploymentResult result)
        {
            using (var iisManager = ServerManager.OpenRemote(ServerName))
            {
                if (iisManager.Sites.Any(site => site.Name.Equals(WebsiteName)))
                {
                    result.AddGood("'{0}' site exists", WebsiteName);
                    return true;
                }
            }
            result.AddAlert("'{0}' site DOES NOT exist", WebsiteName);
            return false;
        }

        void createWebSite(ServerManager iisManager, string websiteName)
        {
            if (_path.DirectoryDoesntExist(PathOnServer))
            {
                _path.CreateDirectory(PathOnServer);
                LogFineGrain("[iis7] Created '{0}'", PathOnServer);
            }

            var firstBinding = Bindings.FirstOrDefault() ?? new IisSiteBinding();

            // Unfortunately the API for adding sites differs for HTTPS & HTTP
            var site = firstBinding.Protocol != "https"
                ? iisManager.Sites.Add(WebsiteName, firstBinding.Protocol, getBindingInformation(firstBinding),
                                       PathOnServer)
                : iisManager.Sites.Add(WebsiteName, getBindingInformation(firstBinding), PathOnServer,
                                       getCertificateHashFor(firstBinding));                    

            LogIis("[iis7] Created website '{0}' with binding for {1}://*:{2}", WebsiteName, firstBinding.Protocol, firstBinding.Port);
        }

        byte[] getCertificateHashFor(IisSiteBinding binding)
        {
            return binding.CertificateThumbPrint == null
                ? new byte[0]
                : binding.CertificateThumbPrint.FromHexToBytes();
        }

        static string getBindingInformation(IisSiteBinding binding)
        {
            return "*:{0}:".FormatWith(binding.Port);
        }

        static void checkForSiteBindingConflict(ServerManager iisManager, string targetSiteName, IEnumerable<IisSiteBinding> targetBindings, DeploymentResult result)
        {
            if (targetBindings == null || !targetBindings.Any())
            {
                result.AddNote("[iis7] No bindings specified for site '{0}'".FormatWith(targetSiteName));
                return;
            }

            foreach (var targetPort in targetBindings.Select(x => x.Port))
            {
                var conflictSite = iisManager.Sites
                    .FirstOrDefault(x => x.Bindings.Any(b =>
                        b.EndPoint != null &&
                        targetPort == b.EndPoint.Port) &&
                        x.Name != targetSiteName);

                if (conflictSite != null)
                    throw new InvalidOperationException(
                        String.Format("Cannot create site '{0}': port '{1}' is already in use by '{2}'.",
                                      targetSiteName, targetPort, conflictSite.Name));
            }
            result.AddGood("[iis7] No site binding conflicts detected.");
        }

        void buildApplicationPool(ServerManager mgr, DeploymentResult result)
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

        	pool.ManagedRuntimeVersion = ManagedRuntimeVersion ?? DefaultManagedRuntimeVersion;
			LogIis("[iis7] Using managed runtime version '{0}'", pool.ManagedRuntimeVersion);

			if (UseClassicPipeline)
			{
				pool.ManagedPipelineMode = ManagedPipelineMode.Classic;
				LogIis("[iis7] Using Classic managed pipeline mode.");
			}

            result.AddGood("App pool '{0}' created/updated.", AppPoolName);

			if (SetProcessModelIdentity)
			{
				setApplicationPoolIdentity(pool);
				result.AddGood("Set process model identity '{0}'", ProcessModelIdentityType);
			}
        }

		void setApplicationPoolIdentity(ApplicationPool pool)
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

        void buildVirtualDirectory(Site site, DeploymentResult result)
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
				result.AddNote("Virtual Directory '{0}' already exists. Updating settings.", VirtualDirectoryPath ?? "/");
			}

            var apn = AppPoolName ?? DefaultAppPoolName;
            if (application.ApplicationPoolName != apn)
			{
                application.ApplicationPoolName = apn;
                LogFineGrain("[iis7] Set the ApplicationPool for '{0}' to '{1}'", VirtualDirectoryPath, apn);
			}

			var vdir = application.VirtualDirectories["/"];
            var pon = PathOnServer ?? DefaultPathOnServer;
            if (vdir.PhysicalPath != pon)
			{
                vdir.PhysicalPath = pon;
                LogFineGrain("[iis7] Updated physical path for '{0}' to '{1}'", VirtualDirectoryPath, pon);
			}
		}

        public bool DoesVirtualDirectoryExist(Site site)
        {
            return site.Applications.Any(app => app.Path.Equals("/" + VirtualDirectoryPath));
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