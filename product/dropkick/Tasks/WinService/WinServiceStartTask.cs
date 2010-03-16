namespace dropkick.Tasks.WinService
{
    using System;
    using System.ServiceProcess;
    using DeploymentModel;


    public class WinServiceStartTask :
        BaseServiceTask
    {
        public WinServiceStartTask(string machineName, string serviceName) : base(machineName, serviceName)
        {
        }

        public override string Name
        {
            get { return string.Format("Starting service '{0}'", ServiceName); }
        }

        public override DeploymentResult VerifyCanRun()
        {
            var result = new DeploymentResult();

            VerifyInAdministratorRole(result);

            try
            {
                using (var c = new ServiceController(ServiceName, MachineName))
                {
                    ServiceControllerStatus currentStatus = c.Status;
                    result.AddGood(string.Format("Found service '{0}' it is '{1}'", ServiceName, currentStatus));
                }
            }
            catch (Exception)
            {
                result.AddAlert(string.Format("Can't find service '{0}'", ServiceName));
            }

            return result;
        }

        public override DeploymentResult Execute()
        {
            var result = new DeploymentResult();

            using (var c = new ServiceController(ServiceName, MachineName))
            {
                c.Start();
                c.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(5));
            }

            result.AddGood("Started the service");

            return result;
        }
    }
}