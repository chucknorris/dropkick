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
namespace dropkick.Tasks.NServiceBusHost
{
    using Prompting;

    public class NServiceBusHostExeArgs
    {
        public string InstanceName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string ServiceName { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string EndpointName { get; set; }
        public string EndpointConfigurationType { get; set; }
        public string Profiles { get; set; }

        public override string ToString()
        {
            var args = "/install";

            if (!string.IsNullOrEmpty(InstanceName))
                args += " /instance:\"{0}\"".FormatWith(InstanceName);

            if (Username != null && Password != null)
                args += " /userName:\"{0}\" /password:\"{1}\"".FormatWith(Username, Password);

            if (!string.IsNullOrEmpty(ServiceName))
                args += " /serviceName:\"{0}\"".FormatWith(ServiceName);

            if (!string.IsNullOrEmpty(DisplayName))
                args += " /displayName:\"{0}\"".FormatWith(DisplayName);

            if (!string.IsNullOrEmpty(Description))
                args += " /description:\"{0}\"".FormatWith(Description);

            if (!string.IsNullOrEmpty(EndpointName))
                args += " /endpointName:\"{0}\"".FormatWith(EndpointName);

            if (!string.IsNullOrEmpty(EndpointConfigurationType))
                args += " /endpointConfigurationType:\"{0}\"".FormatWith(EndpointConfigurationType);

            if (!string.IsNullOrEmpty(Profiles))
                args += " {0}".FormatWith(Profiles);

            return args;
        }

        public void PromptForUsernameAndPasswordIfNecessary(string exeName)
        {
            var prompt = new ConsolePromptService();

            if (Username.ShouldPrompt())
                Username = prompt.Prompt("Win Service '{0}' UserName".FormatWith(exeName));
            if (ShouldPromptForPassword())
                Password = prompt.Prompt("Win Service '{0}' For User '{1}' Password".FormatWith(exeName, Username));
        }

        bool ShouldPromptForPassword()
        {
            return !WindowsAuthentication.IsBuiltInUsername(Username) && Password.ShouldPrompt();
        }
    }
}