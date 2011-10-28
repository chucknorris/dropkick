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
namespace dropkick.Configuration.Dsl.Msmq
{
    using System;
    using DeploymentModel;
    using Tasks;
    using Tasks.Msmq;

    public class ProtoMsmqTask :
        BaseProtoTask,
        MsmqOptions
    {
        readonly QueueOptions _queueOptions = new QueueOptions();
        string _queueName;

        public MsmqQueueOptions PrivateQueue(string name)
        {
            _queueName = ReplaceTokens(name);
            return _queueOptions;
        }

        public override void RegisterRealTasks(PhysicalServer server)
        {
            var ub = new UriBuilder("msmq", server.Name) {Path = _queueName};

            if(server.IsLocal) server.AddTask(new CreateLocalMsmqQueueTask(server, new QueueAddress(ub.Uri), _queueOptions.transactional));
            else server.AddTask(new CreateRemoteMsmqQueueTask(server, new QueueAddress(ub.Uri), _queueOptions.transactional));
        }

        private class QueueOptions : MsmqQueueOptions
        {
            internal bool transactional;

            public void Transactional()
            {
                transactional = true;
            }
        }
    }
}