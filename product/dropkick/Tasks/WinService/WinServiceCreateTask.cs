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
using System;

namespace dropkick.Tasks.WinService
{
    using DeploymentModel;
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
        public string ServiceDisplayName { get; set; }

        public override DeploymentResult VerifyCanRun()
        {
            var result = new DeploymentResult();

            if (UserName.ShouldPrompt())
                result.AddAlert("We are going to prompt for a username.");

			if (shouldPromptForPassword())
                result.AddAlert("We are going to prompt for a password.");

            return result;
        }

        public override DeploymentResult Execute()
        {
            var result = new DeploymentResult();

            if (UserName.ShouldPrompt())
                UserName = _prompt.Prompt("Win Service '{0}' UserName".FormatWith(ServiceName));

            if (shouldPromptForPassword())
                Password = _prompt.Prompt("Win Service '{0}' For User '{1}' Password".FormatWith(ServiceName, UserName));

            ServiceReturnCode returnCode = WmiService.Create(MachineName, ServiceName, ServiceDisplayName, ServiceLocation,
                                                             StartMode, UserName, Password, Dependencies);
            
            if (returnCode != ServiceReturnCode.Success)
                result.AddAlert("Create service returned {0}".FormatWith(returnCode.ToString()));
            else
                result.AddGood("Create service succeeded.");

            return result;
        }

    	private bool shouldPromptForPassword()
    	{
    		return !WindowsAuthentication.IsBuiltInUsername(UserName) && Password.ShouldPrompt();
    	}
    }
}