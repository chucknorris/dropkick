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
    using CommandLine;
    using DeploymentModel;
    using FileSystem;

    public class LocalNServiceBusHostUninstallTask : BaseTask
    {
        private readonly LocalCommandLineTask _task;

        public LocalNServiceBusHostUninstallTask(string exeName, string location, string instanceName, string serviceName)
        {
            string args = string.IsNullOrEmpty(instanceName)
                            ? ""
                            : " /instance:\"{0}\"".FormatWith(instanceName);

            if (!string.IsNullOrEmpty(serviceName))
                args += " /serviceName:\"{0}\"".FormatWith(serviceName);

            _task = new LocalCommandLineTask(new DotNetPath(), exeName)
            {
                Args = "/uninstall " + args,
                ExecutableIsLocatedAt = location,
                WorkingDirectory = location
            };
        }

        public override string Name
        {
            get { return "[nservicebushost] local Uninstalling"; }
        }

        public override DeploymentResult VerifyCanRun()
        {
            return _task.VerifyCanRun();
        }

        public override DeploymentResult Execute()
        {
            Logging.Coarse("[nservicebushost] Uninstalling a local NServiceBus.Host service.");
            return _task.Execute();
        }

    }
}