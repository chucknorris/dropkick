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
namespace dropkick.Configuration.Dsl.NServiceBusHost
{
    using DeploymentModel;
    using FileSystem;
    using Tasks;
    using Tasks.NServiceBusHost;

    public class NServiceBusHostConfigurator :
        BaseProtoTask,
        NServiceBusHostOptions
    {
        readonly NServiceBusHostExeArgs _args;
        readonly Path _path;
        string _location;
        string _exeName;

        public NServiceBusHostConfigurator(Path path)
        {
            _path = path;
            _args = new NServiceBusHostExeArgs();
        }

        public void ExeName(string name)
        {
            _exeName = name;
        }

        public void Instance(string name)
        {
            _args.InstanceName = name;
        }

        public void LocatedAt(string location)
        {
            _location = location;
        }

        public void PassCredentials(string username, string password)
        {
            _args.Username = username;
            _args.Password = password;
        }

        public void ServiceName(string name)
        {
            _args.ServiceName = name;
        }

        public void ServiceDisplayName(string name)
        {
            _args.DisplayName = name;
        }

        public void ServiceDescription(string description)
        {
            _args.Description = description;
        }

        public void EndpointConfigurationType(string iConfigureThisEndpointTypeFullName, string assembly)
        {
            _args.EndpointConfigurationType = iConfigureThisEndpointTypeFullName + ", " + assembly;
        }

        public void EndpointName(string name)
        {
            _args.EndpointName = name;
        }

        public void Profiles(string profiles)
        {
            _args.Profiles = profiles;
        }

        public override void RegisterRealTasks(PhysicalServer site)
        {
            _args.PromptForUsernameAndPasswordIfNecessary(_exeName);

            var location = _path.GetPhysicalPath(site, _location, true);
            if (site.IsLocal)
            {
                site.AddTask(new LocalNServiceBusHostTask(_exeName, location, _args));
            }
            else
            {
                site.AddTask(new RemoteNServiceBusHostTask(_exeName, location, site, _args));
            }
        }
    }
}