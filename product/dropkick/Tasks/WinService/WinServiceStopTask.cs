namespace dropkick.Tasks.WinService
{
    using System;
    using System.ServiceProcess;
    using DeploymentModel;


    public class WinServiceStopTask :
        BaseServiceTask
    {
        public WinServiceStopTask(string machineName, string serviceName) : base(machineName, serviceName)
        {
        }

        public override string Name
        {
            get { return string.Format("Stopping service '{0}'", ServiceName); }
        }

        public override DeploymentResult VerifyCanRun()
        {
            var result = new DeploymentResult();

            VerifyInAdministratorRole(result);

            if (ServiceExists())
            {
                result.AddGood(string.Format("Found service '{0}'", ServiceName));
            }
            else
            {
                result.AddAlert(string.Format("Can't find service '{0}'", ServiceName));
            }

            return result;
        }

        public override DeploymentResult Execute()
        {
            var result = new DeploymentResult();

            if (ServiceExists())
            {
                using (var c = new ServiceController(ServiceName, MachineName))
                {
                    if (c.CanStop)
                        c.Stop();
                    c.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(5));
                }
                result.AddGood("Stopped Service '{0}'", ServiceName);
            }
            else
            {
                result.AddAlert("Service '{0}' does not exist and could not be stopped", ServiceName);
            }

            return result;
        }
    }
}