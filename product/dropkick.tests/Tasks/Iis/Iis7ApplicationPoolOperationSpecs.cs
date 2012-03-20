namespace dropkick.tests.Tasks.Iis
{
    using System;
    using System.Linq;
    using System.Threading;
    using dropkick.Tasks.Iis;
    using Microsoft.Web.Administration;
    using NUnit.Framework;

    public class Iis7ApplicationPoolOperationSpecs
    {
        #region Nested type: Iis7ApplicationPoolOperationsSpecsBase

        public abstract class Iis7ApplicationPoolOperationsSpecsBase : TinySpec
        {
            protected Iis7ApplicationPoolOperationTask task;

            public override void Context()
            {
                CreateApplicationPool();
            }

            public override void AfterObservations()
            {
                DeleteApplicationPool();
            }

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
                    {
                        return;
                    }
                    appPool.Delete();
                    iis.CommitChanges();
                    WaitForIis();
                }
            }

            protected void StartApplicationPool()
            {
                if (AppPool.State != ObjectState.Started)
                {
                    AppPool.Start();
                }
            }

            protected void StopApplicationPool()
            {
                if (AppPool.State != ObjectState.Stopped)
                {
                    AppPool.Stop();
                }
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
                {
                    Assert.IsFalse(iis.ApplicationPools.Any(a => a.Name == ApplicationPoolName));
                }
            }

            protected Exception ExecuteException;
            protected ApplicationPool AppPool { get; private set; }
            protected const string ApplicationPoolName = "DropKickTestApplicationPool6789";
            protected const string WebServerName = "localhost";
        }

        #endregion

        #region Nested type: When_stopping_a_running_application_pool

        [Category("Integration")]
        public class When_stopping_a_running_application_pool : Iis7ApplicationPoolOperationsSpecsBase
        {
            public override void Context()
            {
                base.Context();
                StartApplicationPool();

                task = new Iis7ApplicationPoolOperationTask
                {
                    ServerName = WebServerName,
                    ApplicationPool = ApplicationPoolName,
                    Operation = Iis7ApplicationPoolOperation.StopApplicationPool
                };

            }

            public override void Because()
            {
                task.Execute().LogToConsole();
            }

            [Fact]
            public void the_application_pool_should_stop()
            {
                AppPool.ShouldBeStopped();
            }
        }

        #endregion

        #region Nested type: When_stopping_a_stopped_application_pool

        [Category("Integration")]
        public class When_stopping_a_stopped_application_pool : Iis7ApplicationPoolOperationsSpecsBase
        {
            public override void Context()
            {
                base.Context();
                StopApplicationPool();
                AppPool.ShouldBeStopped();

                task = new Iis7ApplicationPoolOperationTask
               {
                   ServerName = WebServerName,
                   ApplicationPool = ApplicationPoolName,
                   Operation = Iis7ApplicationPoolOperation.StopApplicationPool
               };
            }

            public override void Because()
            {
                ExecuteWithExceptionTrap(() => task.Execute().LogToConsole());
            }

            [Fact]
            public void should_not_raise_an_exception()
            {
                Assert.IsNull(ExecuteException, "Exception encountered executing task: {0}".FormatWith(ExecuteException));
            }

            [Fact]
            public void the_application_pool_should_remain_stopped()
            {
                AppPool.ShouldBeStopped();
            }
        }

        #endregion

        #region Nested type: When_stopping_a_non_existant_application_pool

        [Category("Integration")]
        public class When_stopping_a_non_existant_application_pool : Iis7ApplicationPoolOperationsSpecsBase
        {
            public override void Context()
            {
                base.Context();
                DeleteApplicationPool();
                ApplicationPoolShouldNotExist();

                task = new Iis7ApplicationPoolOperationTask
                {
                    ServerName = WebServerName,
                    ApplicationPool = ApplicationPoolName,
                    Operation = Iis7ApplicationPoolOperation.StopApplicationPool
                };
            }

            public override void Because()
            {
                ExecuteWithExceptionTrap(() => task.Execute().LogToConsole());
            }

            [Fact]
            public void should_not_raise_an_exception()
            {
                Assert.IsNull(ExecuteException, "Exception encounted executing task: {0}".FormatWith(ExecuteException));
            }
        }

        #endregion

        #region Nested type: When_starting_a_stopped_application_pool

        [Category("Integration")]
        public class When_starting_a_stopped_application_pool : Iis7ApplicationPoolOperationsSpecsBase
        {
            public override void Context()
            {
                base.Context();
                StopApplicationPool();
                AppPool.ShouldBeStopped();

                task = new Iis7ApplicationPoolOperationTask
                {
                    ServerName = WebServerName,
                    ApplicationPool = ApplicationPoolName,
                    Operation = Iis7ApplicationPoolOperation.StartApplicationPool
                };
            }

            public override void Because()
            {
                task.Execute().LogToConsole();
            }

            [Fact]
            public void the_application_pool_should_be_running()
            {
                AppPool.ShouldBeRunning();
            }
        }

        #endregion

        #region Nested type: When_starting_a_running_application_pool

        [Category("Integration")]
        public class When_starting_a_running_application_pool : Iis7ApplicationPoolOperationsSpecsBase
        {
            public override void Context()
            {
                base.Context();
                StartApplicationPool();
                AppPool.ShouldBeRunning();

                task = new Iis7ApplicationPoolOperationTask
                {
                    ServerName = WebServerName,
                    ApplicationPool = ApplicationPoolName,
                    Operation = Iis7ApplicationPoolOperation.StartApplicationPool
                };
            }

            public override void Because()
            {
                task.Execute().LogToConsole();
            }

            [Fact]
            public void should_not_raise_an_exception()
            {
                Assert.IsNull(ExecuteException, "Exception encounted executing task: {0}".FormatWith(ExecuteException));
            }

            [Fact]
            public void the_application_pool_should_be_running()
            {
                AppPool.ShouldBeRunning();
            }
        }

        #endregion

        #region Nested type: When_starting_a_non_existant_application_pool


        [Category("Iis7ApplicationPoolOperationTask")]
        [Category("Integration")]
        public class When_starting_a_non_existant_application_pool : Iis7ApplicationPoolOperationsSpecsBase
        {
            public override void Context()
            {
                base.Context();
                DeleteApplicationPool();
                ApplicationPoolShouldNotExist();

                task = new Iis7ApplicationPoolOperationTask
                {
                    ServerName = WebServerName,
                    ApplicationPool = ApplicationPoolName,
                    Operation = Iis7ApplicationPoolOperation.StartApplicationPool
                };
            }

            public override void Because()
            {
                ExecuteWithExceptionTrap(() => task.Execute().LogToConsole());
            }

            [Fact]
            public void It_should_throw_an_exception()
            {
                ExecuteException.ShouldNotBeNull();
            }
        }

        #endregion
    }
}