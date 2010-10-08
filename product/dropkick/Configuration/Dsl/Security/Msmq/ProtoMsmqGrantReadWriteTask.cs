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
    using dropkick.DeploymentModel;
    using dropkick.Dsl.Msmq;
    using dropkick.Tasks;
    using Tasks.Security.Msmq;

    public class ProtoMsmqGrantReadWriteTask :
        BaseProtoTask
    {
        readonly string _group;
        readonly string _queue;

        public ProtoMsmqGrantReadWriteTask(string queue, string @group)
        {
            _queue = ReplaceTokens(queue);
            _group = ReplaceTokens(group);
        }

        public override void RegisterRealTasks(PhysicalServer server)
        {
            var ub = new UriBuilder("msmq", server.Name) { Path = _queue };
            var task = new MsmqGrantReadWriteTask(server, new QueueAddress(ub.Uri), _group);

            server.AddTask(task);
        }
    }
}