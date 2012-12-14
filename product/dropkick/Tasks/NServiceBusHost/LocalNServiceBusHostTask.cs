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

    public class LocalNServiceBusHostTask :
        BaseTask
    {
        private readonly LocalCommandLineTask _task;

        public LocalNServiceBusHostTask(string exeName, string location, NServiceBusHostExeArgs args)
        {
            _task = new LocalCommandLineTask(new DotNetPath(), exeName)
            {
                Args = args.ToString(),
                ExecutableIsLocatedAt = location,
                WorkingDirectory = location
            };
        }

        public override string Name
        {
            get { return "[nservicebushost] local Installing"; }
        }

        public override DeploymentResult VerifyCanRun()
        {
            return _task.VerifyCanRun();
        }

        public override DeploymentResult Execute()
        {
            Logging.Coarse("[nservicebushost] Installing a local NServiceBus.Host service.");
            return _task.Execute();
        }
    }
}