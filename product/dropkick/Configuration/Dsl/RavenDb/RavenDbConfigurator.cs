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
    using System.Collections.Generic;
    using DeploymentModel;
    using FileSystem;
    using Tasks;
    using Tasks.RavenDb;

    public class RavenDbConfigurator
        : BaseProtoTask,
        RavenDbOptions
    {
        readonly List<BaseProtoTask> _innerTasks;
        readonly Path _path;
        
        public RavenDbConfigurator(Path path)
        {
            _path = path;
            _innerTasks = new List<BaseProtoTask>();
        }

        public RavenDbOptionsAsService InstallAsService()
        {
            var task = new RavenDbConfiguratorInstallAsService(this);
            _innerTasks.Add(task);
            return task;
        }

        public RavenDbOptionsAsService UninstallAsService()
        {
            var task = new RavenDbConfiguratorUninstallAsService(this);
            _innerTasks.Add(task);
            return task;
        }

        public override void RegisterRealTasks(PhysicalServer server)
        {
            foreach (BaseProtoTask task in _innerTasks)
            {
                task.RegisterRealTasks(server);
            }
        }

        private class RavenDbConfiguratorInstallAsService : BaseProtoTask, RavenDbOptionsAsService
        {
            string _location;
            readonly RavenDbConfigurator _configurator;

            public RavenDbConfiguratorInstallAsService(RavenDbConfigurator configurator)
            {
                _configurator = configurator;
            }

            public override void RegisterRealTasks(PhysicalServer server)
            {
                var location = _configurator._path.GetPhysicalPath(server, _location, true);
                if (server.IsLocal)
                {
                    server.AddTask(new LocalInstallRavenDbAsServiceTask(location));
                }
                else
                {
                    server.AddTask(new RemoteInstallRavenDbAsServiceTask(server, location));
                }
            }

            public void LocatedAt(string location)
            {
                _location = location;
            }
        }

        private class RavenDbConfiguratorUninstallAsService : BaseProtoTask, RavenDbOptionsAsService
        {
            string _location;
            readonly RavenDbConfigurator _configurator;

            public RavenDbConfiguratorUninstallAsService(RavenDbConfigurator configurator)
            {
                _configurator = configurator;
            }

            public override void RegisterRealTasks(PhysicalServer server)
            {
                var location = _configurator._path.GetPhysicalPath(server, _location, true);
                if (server.IsLocal)
                {
                    server.AddTask(new LocalUninstallRavenDbAsServiceTask(location));
                }
                else
                {
                    server.AddTask(new RemoteUninstallRavenDbAsServiceTask(server, location));
                }
            }

            public void LocatedAt(string location)
            {
                _location = location;
            }
        }
    }
}