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
    using System.Management;
    using System.ServiceProcess;

    public abstract class BaseServiceTask :
        BaseTask
    {
        protected BaseServiceTask(string machineName, string serviceName, string wmiUserName, string wmiPassword)
        {
            MachineName = machineName;
            ServiceName = serviceName;
            WmiUserName = wmiUserName;
            WmiPassword = wmiPassword;
        }

        public string MachineName { get; set; }
        public string ServiceName { get; set; }
        public string WmiUserName { get; set; }
        public string WmiPassword { get; set; }

        protected bool ServiceIsRunning()
        {
            try
            {
                if (string.IsNullOrEmpty(WmiUserName) || string.IsNullOrEmpty(WmiPassword))
                {
                    using (var c = new ServiceController(ServiceName, MachineName))
                    {
                        return (c.Status == ServiceControllerStatus.Running);
                    }
                }
                else
                {
                    var status = dropkick.Wmi.WmiService.QueryService(MachineName, ServiceName, WmiUserName, WmiPassword);
                    switch (status)
                    {
                        case Wmi.ServiceReturnCode.ServiceAlreadyRunning:
                        case Wmi.ServiceReturnCode.Success:
                            return true;
                        default:
                            return false;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        protected bool ServiceExists()
        {
            try
            {
                if(string.IsNullOrEmpty(WmiUserName) || string.IsNullOrEmpty(WmiPassword))
                {
                    using (var c = new ServiceController(ServiceName, MachineName))
                    {
                        ServiceControllerStatus currentStatus = c.Status;
                        return true;
                    }
                }
                else 
                {
                    var status = dropkick.Wmi.WmiService.QueryService(MachineName, ServiceName, WmiUserName, WmiPassword);
                    switch(status)
                    {
                        case Wmi.ServiceReturnCode.DependentServicesRunning:
                        case Wmi.ServiceReturnCode.ServiceAlreadyPaused:
                        case Wmi.ServiceReturnCode.ServiceAlreadyRunning:
                        case Wmi.ServiceReturnCode.StatusServiceExists:
                        case Wmi.ServiceReturnCode.Success:
                        case Wmi.ServiceReturnCode.ServiceNotActive:
                        case Wmi.ServiceReturnCode.InvalidServiceControl:
                            return true;
                        default:
                            return false;
                    }
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
            var scope = new ManagementScope("\\\\{0}\\root\\CIMV2".FormatWith(MachineName));
            var oquery = new ObjectQuery(query);
            var searcher = new ManagementObjectSearcher(scope, oquery);
            int processId = -1;
            foreach (ManagementObject obj in searcher.Get())
            {
                processId = Convert.ToInt32((uint)obj["ProcessId"]);
            }
            return processId;
        }
    }
}