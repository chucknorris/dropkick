﻿// Copyright 2007-2010 The Apache Software Foundation.
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
    using System.Messaging;

    public class QueueSecurityConfiguration :
        QueueSecurityConfig
    {
        readonly string _queue;
        readonly ProtoServer _server;

        public QueueSecurityConfiguration(ProtoServer server, string queue)
        {
            _server = server;
            _queue = queue;
        }

        public void SetSensibleMsmqDefaults()
        {
            var proto = new ProtoSetSensibleMsmqDefaults(_queue);
            _server.RegisterProtoTask(proto);
        }

        public void GrantRead(string group)
        {
            var proto = new ProtoMsmqGrantReadTask(_queue, group);
            _server.RegisterProtoTask(proto);
        }

        public void GrantWrite(string group)
        {
            var proto = new ProtoMsmqGrantWriteTask(_queue, group);
            _server.RegisterProtoTask(proto);
        }

        public void GrantReadWrite(string group)
        {
            var proto = new ProtoMsmqGrantReadWriteTask(_queue, group);
            _server.RegisterProtoTask(proto);
        }

        public void GrantAccessRights(string group, MessageQueueAccessRights accessRights)
        {
            var proto = new ProtoMsmqGrantAccessRightsTask(_queue, group, accessRights);
            _server.RegisterProtoTask(proto);
        }
    }
}