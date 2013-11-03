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
    using DeploymentModel;
    using Wmi;

    public class WinServiceDeleteTask :
        BaseServiceTask
    {
        public WinServiceDeleteTask(string machineName, string serviceName, string wmiUserName=null, string wmiPassword=null) : base(machineName, serviceName, wmiUserName, wmiPassword)
        {
        }

        public override string Name
        {
            get { return "Deleting service '{0}' on '{1}'".FormatWith(ServiceName, MachineName); }
        }

        public override DeploymentResult VerifyCanRun()
        {
            var result = new DeploymentResult();
            if(!ServiceExists())
            {
                result.AddNote("Cannot delete service '{0}', service does not exist".FormatWith(ServiceName));
            }
            return result;
        }

        public override DeploymentResult Execute()
        {
            var result = new DeploymentResult();

            if (!ServiceExists())
            {
                result.AddNote("Cannot delete service '{0}', service does not exist".FormatWith(ServiceName));
            }
            else 
            {
                ServiceReturnCode returnCode = WmiService.Delete(MachineName, ServiceName, WmiUserName, WmiPassword);
                if(returnCode != ServiceReturnCode.Success)
                {
                    result.AddAlert("Deleting service '{0}' failed: '{1}'".FormatWith(ServiceName, returnCode));
                }
            }
            return result;
        }
    }
}