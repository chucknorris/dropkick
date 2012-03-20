using System;
using System.Linq;
using Microsoft.Web.Administration;
using dropkick.DeploymentModel;
using dropkick.Exceptions;

namespace dropkick.Tasks.Iis
{
    public class Iis7SiteOperation
    {
        public static Iis7SiteOperation DeleteSite = new Iis7SiteOperation { Name = "DeleteSite", Action = site => site.Delete(), ReportingLevel = DeploymentItemStatus.Alert };

        public string Name { get; private set; }
        public Action<Site> Action { get; private set; }
        public DeploymentItemStatus ReportingLevel { get; private set; }

        private Iis7SiteOperation()
        {
            ReportingLevel = DeploymentItemStatus.Good;
        }
    }

    public class Iis7SiteOperationTask : BaseTask
    {
        public string ServerName { get; set; }
        public string SiteName { get; set; }
        public Iis7SiteOperation Operation { get; set; }

        public override string Name
        {
            get { return "IIS7: {0} operation".FormatWith(Operation); }
        }

        public override DeploymentResult VerifyCanRun()
        {
            var result = new DeploymentResult();

            if (Operation == null) result.AddError("IIS7 Operation has not been specified.");
            if (String.IsNullOrEmpty(ServerName)) result.AddError("IIS7 Server Name has not been specified.");
            if (String.IsNullOrEmpty(SiteName)) result.AddError("IIS7 Site Name has not been specified.");

            IisUtility.CheckForIis7(result);

            using (var iisManager = ServerManager.OpenRemote(ServerName))
            {
                checkSiteExists(iisManager, result);
            }

            return result;
        }

        public override DeploymentResult Execute()
        {
            var result = new DeploymentResult();

            using (var iisManager = ServerManager.OpenRemote(ServerName))
            {
                var site = iisManager.Sites[SiteName];
                if (site == null)
                    result.AddNote(siteDoesNotExist);
                else
                {
                    checkForElevatedPrivileges(() => Operation.Action(site));
                    iisManager.CommitChanges();
                    result.Add(new DeploymentItem(Operation.ReportingLevel, "Site operation '{0}' executed on '{1}'".FormatWith(Operation.Name, SiteName)));
                }
            }
            return result;
        }

        private void checkSiteExists(ServerManager iisManager, DeploymentResult result)
        {
            if (!iisManager.Sites.Any(a => a.Name == SiteName)) result.AddNote(siteDoesNotExist);
        }

        private string siteDoesNotExist
        {
            get { return "Site '{0}' does not exist on server '{1}'".FormatWith(SiteName, ServerName); }
        }

        private void checkForElevatedPrivileges(Action action)
        {
            try
            {
                action();
            }
            catch (UnauthorizedAccessException)
            {
                throw new DeploymentException("The IIS7 operation you are attempting '{0}' requires elevated privileges. Retry running the task as an Administrator or with UAC disabled.".FormatWith(Operation));
            }
        }
    }
}
