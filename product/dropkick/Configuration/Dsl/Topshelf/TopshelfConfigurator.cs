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
namespace dropkick.Configuration.Dsl.Topshelf
{
    using DeploymentModel;
    using FileSystem;
    using Tasks;
    using Tasks.Topshelf;

    public class TopshelfConfigurator :
        BaseProtoTask,
        TopshelfOptions
    {
        readonly Path _path;
        string _instanceName;
        string _location;
        string _exeName;
        string _password;
        string _username;

        public TopshelfConfigurator(Path path)
        {
            _path = path;
        }

        public void ExeName(string name)
        {
            _exeName = name;
        }

        public void Instance(string name)
        {
            _instanceName = name;
        }

        public void LocatedAt(string location)
        {
            _location = location;
        }

        public void PassCredentials(string username, string password)
        {
            _username = username;
            _password = password;
        }

        public override void RegisterRealTasks(PhysicalServer site)
        {
            if (site.IsLocal)
            {
                site.AddTask(new LocalTopshelfTask(_exeName, _location, _instanceName, _username, _password));
            }
            else
            {
                site.AddTask(new RemoteTopshelfTask(_exeName, _location, _instanceName, site, _username, _password));
            }
        }
    }
}