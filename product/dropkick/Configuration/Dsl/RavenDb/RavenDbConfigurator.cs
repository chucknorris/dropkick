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
namespace dropkick.Configuration.Dsl.RavenDb
{
    using DeploymentModel;
    using FileSystem;
    using Tasks;
    using Tasks.RavenDb;

    public class RavenDbConfigurator
        : BaseProtoTask,
        RavenDbOptions
    {
        BaseProtoTask _innerTask;
        readonly Path _path;
        
        public RavenDbConfigurator(Path path)
        {
            _path = path;
        }

        public RavenDbOptionsAsService Service()
        {
            _innerTask = new RavenDbConfiguratorAsService(this);
            return (RavenDbOptionsAsService) _innerTask;
        }

        public override void RegisterRealTasks(PhysicalServer server)
        {
            _innerTask.RegisterRealTasks(server);
        }

        private class RavenDbConfiguratorAsService : BaseProtoTask, RavenDbOptionsAsService
        {
            string _location;
            readonly RavenDbConfigurator _configurator;

            public RavenDbConfiguratorAsService(RavenDbConfigurator configurator)
            {
                _configurator = configurator;
            }

            public override void RegisterRealTasks(PhysicalServer server)
            {
                var location = _configurator._path.GetPhysicalPath(server, _location, true);
                if (server.IsLocal)
                {
                    server.AddTask(new LocalRavenDbAsServiceTask(location));
                }
                else
                {
                    server.AddTask(new RemoteRavenDbAsServiceTask(server, location));
                }
            }

            public void LocatedAt(string location)
            {
                _location = location;
            }
        }
    }
}