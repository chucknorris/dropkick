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
    using DeploymentModel;
    using Tasks;
    using Tasks.WinService;
    using Wmi;

    public class ProtoWinServiceCreateTask :
        BaseProtoTask,
        WinServiceCreateOptions
    {
        readonly List<string> _dependencies = new List<string>();
        readonly string _serviceName;
        string _description;
        string _installPath;
        string _password;
        ServiceStartMode _startMode;
        string _userName;

        public ProtoWinServiceCreateTask(string privateName)
        {
            _serviceName = privateName;
        }

        #region WinServiceCreateOptions Members

        public WinServiceCreateOptions WithDescription(string description)
        {
            _description = description;
            return this;
        }

        public WinServiceCreateOptions WithServicePath(string path)
        {
            _installPath = path;
            return this;
        }

        public WinServiceCreateOptions WithStartMode(ServiceStartMode mode)
        {
            _startMode = mode;
            return this;
        }

        public WinServiceCreateOptions WithCredentials(string username, string password)
        {
            _userName = username;
            _password = password;
            return this;
        }

        #endregion

        public override void RegisterRealTasks(PhysicalServer site)
        {
            site.AddTask(new WinServiceCreateTask(site.Name, _serviceName)
                             {
                                 Dependencies = _dependencies.ToArray(),
                                 UserName = _userName,
                                 Password = _password,
                                 //ServiceDescription =  _description, no place to put this currently
                                 ServiceLocation = _installPath,
                                 StartMode = _startMode
                             });
        }
    }

    public interface WinServiceCreateOptions
    {
        WinServiceCreateOptions WithDescription(string description);
        WinServiceCreateOptions WithServicePath(string path);
        WinServiceCreateOptions WithStartMode(ServiceStartMode mode);
        WinServiceCreateOptions WithCredentials(string username, string password);
    }
}