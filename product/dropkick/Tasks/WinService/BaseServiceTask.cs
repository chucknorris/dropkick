namespace dropkick.Tasks.WinService
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Management;
    using System.ServiceProcess;
    using System.Threading;
    using DeploymentModel;


    public abstract class BaseServiceTask :
        Task
    {
        protected BaseServiceTask(string machineName, string serviceName)
        {
            MachineName = machineName;
            ServiceName = serviceName;
        }

        public string MachineName { get; set; }
        public string ServiceName { get; set; }

        public abstract string Name { get; }

        public abstract DeploymentResult VerifyCanRun();
        public abstract DeploymentResult Execute();



        protected void VerifyInAdministratorRole(DeploymentResult result)
        {
            if (Thread.CurrentPrincipal.IsInRole("Administrator"))
            {
                result.AddAlert("You are not in the 'Administrator' role. You will not be able to start/stop services");
            }
            else
            {
                result.AddGood("You are in the 'Administrator' role");
            }
        }

        protected bool ServiceExists()
        {
            try
            {
                using (var c = new ServiceController(ServiceName, MachineName))
                {
                    ServiceControllerStatus currentStatus = c.Status;
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        protected int GetProcessId(string serviceName)
        {
            string query = string.Format("SELECT ProcessId FROM Win32_Service WHERE Name='{0}'", serviceName);
            var searcher = new ManagementObjectSearcher(query);
            int processId = -1;
            foreach (ManagementObject obj in searcher.Get())
            {
                processId = Convert.ToInt32((uint) obj["ProcessId"]);
            }
            return processId;
        }
    }
}