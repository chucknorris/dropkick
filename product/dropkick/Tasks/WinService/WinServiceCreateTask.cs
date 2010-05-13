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
    using Magnum.Extensions;
    using Prompting;
    using Wmi;

    public class WinServiceCreateTask :
        BaseServiceTask
    {
        //TODO: should be injected
        readonly PromptService _prompt = new ConsolePromptService();

        public WinServiceCreateTask(string machineName, string serviceName)
            : base(machineName, serviceName)
        {
        }

        public override string Name
        {
            get { return "Installing service '{0}' on '{1}'".FormatWith(ServiceName, MachineName); }
        }


        public string[] Dependencies { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        public ServiceStartMode StartMode { get; set; }
        public string ServiceLocation { get; set; }

        public override DeploymentResult VerifyCanRun()
        {
            var result = new DeploymentResult();

            if (UserName.IsNotEmpty())
                result.AddAlert("We are going to prompt for a username.");

            if (Password.IsNotEmpty())
                result.AddAlert("We are going to prompt for a password.");

            return result;
        }

        public override DeploymentResult Execute()
        {
            var result = new DeploymentResult();

            if (UserName.IsNotEmpty())
                UserName = _prompt.Prompt("Win Service '{0}' UserName".FormatWith(ServiceName));

            if (Password.IsNotEmpty())
                UserName = _prompt.Prompt("Win Service '{0}' Password".FormatWith(ServiceName));

            ServiceReturnCode returnCode = WmiService.Create(MachineName, ServiceName, ServiceName, ServiceLocation,
                                                             StartMode, UserName, Password, Dependencies);

            return result;
        }
    }
}