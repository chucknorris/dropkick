// Copyright 2007-2012 The Apache Software Foundation.
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
namespace dropkick.Tasks.RavenDb
{
    using CommandLine;
    using DeploymentModel;

    internal class RemoteRavenDbAsServiceTask : BaseTask
    {
        readonly string _location;
        readonly RemoteCommandLineTask _task;

        public RemoteRavenDbAsServiceTask(PhysicalServer site, string location)
        {
            _location = location;

            _task = new RemoteCommandLineTask("Raven.Server.exe")
            {
                Args = "/install",
                ExecutableIsLocatedAt = location,
                Machine = site.Name,
                WorkingDirectory = location
            };
        }

        public override string Name
        {
            get { return "[ravendb] Installing remote RavenDb as a service from location {0}.".FormatWith(_location); }
        }

        public override DeploymentResult VerifyCanRun()
        {
            return _task.VerifyCanRun();
        }

        public override DeploymentResult Execute()
        {
            Logging.Coarse("[ravendb] Installing a remote RavenDb as a service.");
            return _task.Execute();
        }
    }
}