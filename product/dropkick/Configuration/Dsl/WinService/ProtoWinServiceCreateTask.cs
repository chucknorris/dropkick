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
namespace dropkick.Configuration.Dsl.WinService
{
    using System.Collections.Generic;
    using System.Text;
    using DeploymentModel;
    using Exceptions;
    using FileSystem;
    using Tasks;
    using Tasks.WinService;
    using Wmi;

    public class ProtoWinServiceCreateTask :
        BaseProtoTask,
        WinServiceCreateOptions
    {
        readonly List<string> _dependencies = new List<string>();
        readonly string _serviceName;
        string _displayName;
        string _installPath;
        string _password;
        ServiceStartMode _startMode;
        string _userName;
        Path _path;

        public ProtoWinServiceCreateTask(Path path, string serviceName)
        {
            _path = path;
            _serviceName = ReplaceTokens(serviceName);
        }

        public WinServiceCreateOptions WithDisplayName(string displayName)
        {
            _displayName = ReplaceTokens(displayName);
            return this;
        }

        public WinServiceCreateOptions WithServicePath(string path)
        {
            _installPath = ReplaceTokens(path);
            return this;
        }

        public WinServiceCreateOptions WithStartMode(ServiceStartMode mode)
        {
            _startMode = mode;
            return this;
        }

        public WinServiceCreateOptions WithCredentials(string username, string password)
        {
            _userName = ReplaceTokens(username);
            _password = ReplaceTokens(password);
            return this;
        }

        public WinServiceCreateOptions AddDependency(string name)
        {
            var dependencyName = ReplaceTokens(name);
            if (!string.IsNullOrEmpty(dependencyName))
                _dependencies.Add(dependencyName);
            return this;
        }

        public override void RegisterRealTasks(PhysicalServer site)
        {
            string serviceLocation = _installPath;
            serviceLocation = _path.GetPhysicalPath(site, _installPath, true);

            site.AddTask(new WinServiceCreateTask(site.Name, _serviceName)
                             {
                                 Dependencies = _dependencies.ToArray(),
                                 UserName = _userName,
                                 Password = _password,
                                 ServiceDisplayName = _displayName,
                                 ServiceLocation = serviceLocation,
                                 StartMode = _startMode
                             });
        }
    }
}