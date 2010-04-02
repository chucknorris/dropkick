namespace dropkick.Tasks.WinService
{
    using System;
    using System.Management;
    using DeploymentModel;
    using Wmi;

    public class WinServiceDeleteTask :
        BaseServiceTask
    {
        public WinServiceDeleteTask(string machineName, string serviceName) : base(machineName, serviceName)
        {
        }

        public override string Name
        {
            get { return "Deleting service '{0}' on '{1}'".FormatWith(ServiceName, MachineName); }
        }

        public override DeploymentResult VerifyCanRun()
        {
            var result = new DeploymentResult();

            return result;
        }

        public override DeploymentResult Execute()
        {
            var result = new DeploymentResult();

            var returnCode = WmiService.Delete(MachineName, ServiceName);

            return result;
        }
    }
}