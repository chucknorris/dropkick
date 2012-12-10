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
namespace dropkick.Configuration.Dsl.Security.Msmq
{
    using System;
    using DeploymentModel;
    using Tasks;
    using Dsl.Msmq;
    using Tasks.Security.Msmq;

    public class ProtoMsmqNServiceBusPermissionsTask :
        BaseProtoTask
    {
        readonly string _user;
        readonly string _serviceName;

        public ProtoMsmqNServiceBusPermissionsTask(string serviceName, string user)
        {
            _serviceName = ReplaceTokens(serviceName);
            _user = ReplaceTokens(user);
        }

        public override void RegisterRealTasks(PhysicalServer site)
        {
            var uriBuilder = new UriBuilder("msmq", site.Name) { Path = _serviceName };
            var address = new QueueAddress(uriBuilder.Uri);

            Task task;

            if (site.IsLocal)
                task = new LocalMsmqNServiceBusPermissionsTask(address, _user);
            else
                task = new RemoteMsmqNServiceBusPermissionsTask(site, address, _user);

            site.AddTask(task);
        }
    }
}