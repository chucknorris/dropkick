namespace dropkick.Tasks.Topshelf
{
    using System;
    using CommandLine;
    using DeploymentModel;

    public class LocalTopshelfTask :
        BaseTask
    {
        LocalCommandLineTask _task;

        public LocalTopshelfTask(string exeName, string location, string instanceName)
        {
            var args = string.IsNullOrEmpty(instanceName)
                ? "" 
                : " /instance:" + instanceName;

            _task = new LocalCommandLineTask(exeName)
                        {
                            Args = "install" + args,
                            ExecutableIsLocatedAt = location,
                            WorkingDirectory = location
                        };
        }

        public override string Name
        {
            get { return "[topshelf] local Installing"; }
        }

        public override DeploymentResult VerifyCanRun()
        {
            return _task.VerifyCanRun();
        }

        public override DeploymentResult Execute()
        {
            return _task.Execute();
        }
    }

    public class RemoteTopshelfTask :
        BaseTask
    {
        RemoteCommandLineTask _task;

        public RemoteTopshelfTask(string exeName, string location, string instanceName, PhysicalServer site)
        {
            var args = string.IsNullOrEmpty(instanceName)
                ? "" 
                : " /instance:" + instanceName;

            _task = new RemoteCommandLineTask(exeName)
                        {
                            Args = "install" + args,
                            ExecutableIsLocatedAt = location,
                            Machine = site.Name,
                            WorkingDirectory = location
                        };
        }

        public override string Name
        {
            get { return "[topshelf] remote Installing"; }
        }

        public override DeploymentResult VerifyCanRun()
        {
            return _task.VerifyCanRun();
        }

        public override DeploymentResult Execute()
        {
            return _task.Execute();
        }
    }
}