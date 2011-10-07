using NUnit.Framework;
using dropkick.Tasks.Iis;
using dropkick.tests.TestContexts;

namespace dropkick.tests.Tasks.Iis
{
    [TestFixture]
    public class When_stopping_a_running_application_pool : ApplicationPoolTestContext
    {
        [Test, Explicit]
        public void Then_the_application_pool_should_stop()
        {
            AppPool.ShouldBeStopped();
        }

        public override void Context()
        {
            CreateApplicationPool();
            StartApplicationPool();
        }

        public override void Because()
        {
            var task = new Iis7OperationTask
                           {
                               ServerName = WebServerName,
                               ApplicationPool = ApplicationPoolName,
                               Operation = Iis7Operation.StopApplicationPool
                           };
            task.Execute()
                .LogToConsole();
        }

        public override void AfterObservations()
        {
            DeleteApplicationPool();
        }
    }

    [TestFixture]
    public class When_stopping_a_stopped_application_pool : ApplicationPoolTestContext
    {
        [Test, Explicit]
        public void It_should_not_raise_an_exception()
        {
            Assert.IsNull(ExecuteException, "Exception encountered executing task: {0}".FormatWith(ExecuteException));
        }

        [Test, Explicit]
        public void Then_the_application_pool_should_remain_stopped()
        {
            AppPool.ShouldBeStopped();
        }

        public override void Context()
        {
            CreateApplicationPool();
            StopApplicationPool();
        }

        public override void Because()
        {
            AppPool.ShouldBeStopped();

            var task = new Iis7OperationTask
                {
                    ServerName = WebServerName,
                    ApplicationPool = ApplicationPoolName,
                    Operation = Iis7Operation.StopApplicationPool
                };
            ExecuteWithExceptionTrap(() => task.Execute().LogToConsole());
        }

        public override void AfterObservations()
        {
            DeleteApplicationPool();
        }
    }

    [TestFixture]
    public class When_stopping_a_non_existant_application_pool : ApplicationPoolTestContext
    {
        [Test, Explicit]
        public void It_should_not_raise_an_exception()
        {
            Assert.IsNull(ExecuteException, "Exception encounted executing task: {0}".FormatWith(ExecuteException));
        }

        public override void Context()
        {
            DeleteApplicationPool();
        }

        public override void Because()
        {
            ApplicationPoolShouldNotExist();

            var task = new Iis7OperationTask
            {
                ServerName = WebServerName,
                ApplicationPool = ApplicationPoolName,
                Operation = Iis7Operation.StopApplicationPool
            };
            ExecuteWithExceptionTrap(() => task.Execute().LogToConsole());
        }
    }

    [TestFixture]
    public class When_starting_a_stopped_application_pool : ApplicationPoolTestContext
    {
        [Test, Explicit]
        public void Then_the_application_pool_should_be_running()
        {
            AppPool.ShouldBeRunning();
        }

        public override void Context()
        {
            CreateApplicationPool();
            StopApplicationPool();
        }

        public override void Because()
        {
            AppPool.ShouldBeStopped();

            var task = new Iis7OperationTask
            {
                ServerName = WebServerName,
                ApplicationPool = ApplicationPoolName,
                Operation = Iis7Operation.StartApplicationPool
            };
            task.Execute().LogToConsole();
        }

        public override void AfterObservations()
        {
            DeleteApplicationPool();
        }
    }

    [TestFixture]
    public class When_starting_a_running_application_pool : ApplicationPoolTestContext
    {
        [Test, Explicit]
        public void It_should_not_raise_an_exception()
        {
            Assert.IsNull(ExecuteException, "Exception encounted executing task: {0}".FormatWith(ExecuteException));
        }

        [Test, Explicit]
        public void Then_the_application_pool_should_be_running()
        {
            AppPool.ShouldBeRunning();
        }

        public override void Context()
        {
            CreateApplicationPool();
            StartApplicationPool();
        }

        public override void Because()
        {
            AppPool.ShouldBeRunning();

            var task = new Iis7OperationTask
            {
                ServerName = WebServerName,
                ApplicationPool = ApplicationPoolName,
                Operation = Iis7Operation.StartApplicationPool
            };
            task.Execute().LogToConsole();
        }

        public override void AfterObservations()
        {
            DeleteApplicationPool();
        }
    }

    [TestFixture, Category("Iis7OperationTask")]
    public class When_starting_a_non_existant_application_pool : ApplicationPoolTestContext
    {
        [Test, Explicit]
        public void It_should_throw_an_exception()
        {
            ExecuteException.ShouldNotBeNull();
        }

        public override void Context()
        {
            DeleteApplicationPool();
        }

        public override void Because()
        {
            ApplicationPoolShouldNotExist();

            var task = new Iis7OperationTask
            {
                ServerName = WebServerName,
                ApplicationPool = ApplicationPoolName,
                Operation = Iis7Operation.StartApplicationPool
            };
            ExecuteWithExceptionTrap(() =>task.Execute().LogToConsole());
        }
    }
}
