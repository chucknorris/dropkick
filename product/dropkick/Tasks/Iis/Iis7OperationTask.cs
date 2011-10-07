using System;
using System.Linq;
using Microsoft.Web.Administration;
using dropkick.DeploymentModel;
using dropkick.Exceptions;

namespace dropkick.Tasks.Iis
{
    public static class ApplicationPoolExtensions
    {
        public static bool CanBeStopped(this ApplicationPool applicationPool)
        {
            return applicationPool.State != ObjectState.Stopped && applicationPool.State != ObjectState.Stopping;
        }

        public static bool CanBeStarted(this ApplicationPool applicationPool)
        {
            return applicationPool.State != ObjectState.Started && applicationPool.State != ObjectState.Starting;
        }
    }

    public enum Iis7Operation
    {
        Unspecified,
        StopApplicationPool,
        StartApplicationPool
    }

    public class Iis7OperationTask : BaseTask
    {
        public string ServerName { get; set; }
        public string ApplicationPool { get; set; }
        public Iis7Operation Operation { get; set; }

        public override string Name
        {
            get { return "IIS7: {0} operation".FormatWith(Operation); }
        }

        public override DeploymentResult VerifyCanRun()
        {
            var result = new DeploymentResult();

            if (Operation == Iis7Operation.Unspecified)
                result.AddError("IIS7 Operation has not been specified.");
            if (String.IsNullOrEmpty(ServerName))
                result.AddError("IIS7 Server Name has not been specified.");
            if (String.IsNullOrEmpty(ApplicationPool))
                result.AddError("IIS7 Application Pool has not been specified.");

            IisUtility.CheckForIis7(result);

            using (var iisManager = ServerManager.OpenRemote(ServerName))
                checkApplicationPoolExists(iisManager, result);

            return result;
        }

        void checkApplicationPoolExists(ServerManager iisManager, DeploymentResult result)
        {
            if (!iisManager.ApplicationPools.Any(a => a.Name == ApplicationPool))
                result.AddAlert(applicationPoolDoesNotExistError);
        }

        string applicationPoolDoesNotExistError
        {
            get { return "Application Pool '{0}' does not exist on server '{1}'".FormatWith(ApplicationPool, ServerName); }
        }

        public override DeploymentResult Execute()
        {
            var result = new DeploymentResult();

            using (var iisManager = ServerManager.OpenRemote(ServerName))
            {
                var appPool = iisManager.ApplicationPools[ApplicationPool];
                executeOperation(appPool, result);
            }
            return result;
        }

        void checkForElevatedPrivileges(Action action)
        {
            try
            {
                action();
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new DeploymentException("The IIS7 operation you are attempting '{0}' requires elevated privileges. Retry running the task as an Administrator or with UAC disabled.".FormatWith(Operation));
            }
        }

        void executeOperation(ApplicationPool appPool, DeploymentResult result)
        {
            switch (Operation)
            {
                case Iis7Operation.StopApplicationPool:
                    if (appPool == null)
                    {
                        result.AddAlert(applicationPoolDoesNotExistError);
                    }
                    else if (appPool.CanBeStopped())
                    {
                        checkForElevatedPrivileges(() => appPool.Stop());
                        result.AddGood("Application Pool '{0}' stopped.".FormatWith(ApplicationPool));
                    }
                    else
                    {
                        result.AddNote("Application Pool '{0}' is not running.".FormatWith(ApplicationPool));
                    }
                    break;
                case Iis7Operation.StartApplicationPool:
                    if (appPool == null)
                    {
                        throw new InvalidOperationException(applicationPoolDoesNotExistError);
                    }
                    else if (appPool.CanBeStarted())
                    {
                        checkForElevatedPrivileges(() => appPool.Start());
                        result.AddGood("Application Pool '{0}' started.".FormatWith(ApplicationPool));
                    }
                    else
                    {
                        result.AddGood("Application Pool '{0}' is already running.".FormatWith(ApplicationPool));
                    }
                    break;
            }
        }
    }
}
