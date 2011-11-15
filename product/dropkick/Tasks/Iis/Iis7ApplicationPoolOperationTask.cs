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

        public static void StartAndWaitForCompletion(this ApplicationPool applicationPool)
        {
            applicationPool.Start();
            do
            {
                IisUtility.WaitForIisToCompleteAnyOperations();
            } while (applicationPool.State == ObjectState.Stopping);
        }

        public static void StopAndWaitForCompletion(this ApplicationPool applicationPool)
        {
            applicationPool.Stop();
            do
            {
                IisUtility.WaitForIisToCompleteAnyOperations();
            } while (applicationPool.State == ObjectState.Starting);
        }
    }

    public enum Iis7ApplicationPoolOperation
    {
        Unspecified,
        StopApplicationPool,
        StartApplicationPool,
    }

    public class Iis7ApplicationPoolOperationTask : BaseTask
    {
        public string ServerName { get; set; }
        public string ApplicationPool { get; set; }
        public Iis7ApplicationPoolOperation Operation { get; set; }

        public override string Name
        {
            get { return "IIS7: {0} operation".FormatWith(Operation); }
        }

        public override DeploymentResult VerifyCanRun()
        {
            var result = new DeploymentResult();

            if (Operation == Iis7ApplicationPoolOperation.Unspecified) result.AddError("IIS7 Operation has not been specified.");
            if (String.IsNullOrEmpty(ServerName)) result.AddError("IIS7 Server Name has not been specified.");
            if (String.IsNullOrEmpty(ApplicationPool)) result.AddError("IIS7 Application Pool has not been specified.");

            IisUtility.CheckForIis7(result);

            using (var iisManager = ServerManager.OpenRemote(ServerName))
            {
                CheckApplicationPoolExists(iisManager, result);    
            }
            
            return result;
        }

        private void CheckApplicationPoolExists(ServerManager iisManager, DeploymentResult result)
        {
            if (!iisManager.ApplicationPools.Any(a => a.Name == ApplicationPool)) result.AddAlert(ApplicationPoolDoesNotExistError);
        }

        private string ApplicationPoolDoesNotExistError
        {
            get { return "Application Pool '{0}' does not exist on server '{1}'".FormatWith(ApplicationPool, ServerName); }
        }

        public override DeploymentResult Execute()
        {
            var result = new DeploymentResult();

            using (var iisManager = ServerManager.OpenRemote(ServerName))
            {
                var appPool = iisManager.ApplicationPools[ApplicationPool];
                ExecuteOperation(appPool, result);
            }
            return result;
        }

        private void CheckForElevatedPrivileges(Action action)
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

        //todo: there is quite a bit going on in here...this is going to need to be looked at...
        private void ExecuteOperation(ApplicationPool appPool, DeploymentResult result)
        {
            switch (Operation)
            {
                case Iis7ApplicationPoolOperation.StopApplicationPool:
                    if (appPool == null)
                    {
                        result.AddAlert(ApplicationPoolDoesNotExistError);
                    }
                    else if (appPool.CanBeStopped())
                    {
                        CheckForElevatedPrivileges(appPool.StopAndWaitForCompletion);
                        result.AddGood("Application Pool '{0}' stopped.".FormatWith(ApplicationPool));
                    }
                    else
                    {
                        result.AddNote("Application Pool '{0}' is not running.".FormatWith(ApplicationPool));
                    }
                    break;
                case Iis7ApplicationPoolOperation.StartApplicationPool:
                    if (appPool == null)
                    {
                        throw new InvalidOperationException(ApplicationPoolDoesNotExistError);
                    }
                    else if (appPool.CanBeStarted())
                    {
                        IisUtility.WaitForIisToCompleteAnyOperations();
                        CheckForElevatedPrivileges(appPool.StartAndWaitForCompletion);
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
