using System;
using System.Linq;
using System.Threading;
using Microsoft.Web.Administration;
using NUnit.Framework;

namespace dropkick.tests.TestContexts
{
    [Category("Iis7OperationTask")]
    public abstract class ApplicationPoolTestContext : TinySpec
    {
        protected void CreateApplicationPool()
        {
            using (var iis = ServerManager.OpenRemote(WebServerName))
            {
                AppPool = iis.ApplicationPools[ApplicationPoolName];
                if (AppPool == null)
                {
                    AppPool = iis.ApplicationPools.Add(ApplicationPoolName);
                    iis.CommitChanges();
                    WaitForIis();
                }
            }
        }

        protected void DeleteApplicationPool()
        {
            using (var iis = ServerManager.OpenRemote(WebServerName))
            {
                var appPool = iis.ApplicationPools[ApplicationPoolName];
                if (appPool == null)
                    return;
                appPool.Delete();
                iis.CommitChanges();
                WaitForIis();
            }
        }

        protected void StartApplicationPool()
        {
            if (AppPool.State != ObjectState.Started)
                AppPool.Start();
        }

        protected void StopApplicationPool()
        {
            if (AppPool.State != ObjectState.Stopped)
                AppPool.Stop();
        }

        /// <summary>
        /// Wait for IIS to do its thang.
        /// </summary>
        protected void WaitForIis()
        {
            Thread.Sleep(500);
        }

        protected void ExecuteWithExceptionTrap(Action action)
        {
            ExecuteException = null;
            try
            {
                action();
            }
            catch (Exception ex)
            {
                ExecuteException = ex;
            }
        }

        protected void ApplicationPoolShouldNotExist()
        {
            using (var iis = ServerManager.OpenRemote(WebServerName))
                Assert.IsFalse(iis.ApplicationPools.Any(a => a.Name == ApplicationPoolName));
        }

        protected Exception ExecuteException;
        protected ApplicationPool AppPool { get; private set; }
        protected const string ApplicationPoolName = "DropKickTestApplicationPool6789";
        protected const string WebServerName = "localhost";
    }
}
