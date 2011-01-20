// Copyright 2007-2010 The Apache Software Foundation.
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace dropkick.Tasks.WinService
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.ServiceProcess;
    using DeploymentModel;
    using Magnum.Extensions;

    public class WinServiceStopTask :
        BaseServiceTask
    {
        public WinServiceStopTask(string machineName, string serviceName)
            : base(machineName, serviceName)
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
                        int pid = GetProcessId(ServiceName);

                        c.Stop();
                        c.WaitForStatus(ServiceControllerStatus.Stopped, 30.Seconds());

                        WaitForProcessToDie(pid);
                    }
                }
                result.AddGood("Stopped Service '{0}'", ServiceName);
                Logging.Coarse("[svc] Stopped service '{0}'", ServiceName);
            }
            else
            {
                result.AddAlert("Service '{0}' does not exist and could not be stopped", ServiceName);
                Logging.Coarse("[svc] Service '{0}' does not exist.", ServiceName);
            }

            return result;
        }

        public void WaitForProcessToDie(int pid)
        {
            DateTime timeout = DateTime.Now.AddMinutes(5);
            while (DateTime.Now < timeout)
            {
                Process[] process = Process.GetProcesses(MachineName);
                IEnumerable<Process> p = process.Where(x => x.Id == pid);
                if (p.Count() == 0)
                {
                    return;
                }
            }
            throw new Exception("Service has not died yet!");
        }
    }
}