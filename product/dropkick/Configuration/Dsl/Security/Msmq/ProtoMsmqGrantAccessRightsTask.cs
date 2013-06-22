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
    using System.Messaging;
    using DeploymentModel;
    using Tasks;
    using Tasks.Security.Msmq;
    using Dsl.Msmq;

    public class ProtoMsmqGrantAccessRightsTask :
        BaseProtoTask
    {
        readonly MessageQueueAccessRights _accessRights;
        readonly string _group;
        readonly string _queue;

        public ProtoMsmqGrantAccessRightsTask(string queue, string group, MessageQueueAccessRights accessRights)
        {
            _accessRights = accessRights;
            _queue = ReplaceTokens(queue);
            _group = ReplaceTokens(group);
        }

        public override void RegisterRealTasks(PhysicalServer site)
        {
            var uriBuilder = new UriBuilder("msmq", site.Name) { Path = _queue };
            var address = new QueueAddress(uriBuilder.Uri);

            Task task;

            if (site.IsLocal)
                task = new LocalMsmqGrantAccessRightsTask(address, _group, _accessRights);
            else
                task = new RemoteMsmqGrantAccessRightsTask(site, address, _group, _accessRights);

            site.AddTask(task);
        }
    }
}