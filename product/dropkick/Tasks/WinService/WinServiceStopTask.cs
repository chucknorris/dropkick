namespace dropkick.Tasks.WinService
{
    using System;
    using System.Linq;
    using System.ServiceProcess;
    using DeploymentModel;
    using Magnum.Extensions;


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
                    {
                        var pid = GetProcessId(ServiceName);

                        c.Stop();
                        c.WaitForStatus(ServiceControllerStatus.Stopped, 10.Seconds());
                        
                        WaitForProcessToDie(pid);
                    }
                }
                result.AddGood("Stopped Service '{0}'", ServiceName);
            }
            else
            {
                result.AddAlert("Service '{0}' does not exist and could not be stopped", ServiceName);
            }

            return result;
        }

        public void WaitForProcessToDie(int pid)
        {
            var timeout = DateTime.Now.AddMinutes(5);
            while(DateTime.Now < timeout)
            {
                var process = System.Diagnostics.Process.GetProcesses(MachineName);
                var p = process.Where(x => x.Id == pid);
                if(p.Count() == 0)
                {
                    return;
                }
            }
            throw new Exception("Service has not died yet!");
        }
    }
}