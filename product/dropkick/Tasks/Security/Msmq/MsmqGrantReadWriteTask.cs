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
namespace dropkick.Tasks.Security.Msmq
{
    using System;
    using System.Messaging;
    using DeploymentModel;
    using Dsl.Msmq;

    public class MsmqGrantReadWriteTask :
        BaseTask
    {
        private PhysicalServer _server;
        readonly string _group;
        private QueueAddress Address { get; set; }

        public MsmqGrantReadWriteTask(PhysicalServer server, QueueAddress address, string group)
        {
            _server = server;
            _group = group;
            Address = address;
        }

        public MsmqGrantReadWriteTask(PhysicalServer server, string queueName, string group)
        {
            _server = server;
            _group = group;
            var ub = new UriBuilder("msmq", server.Name) { Path = queueName };
            Address = new QueueAddress(ub.Uri);
        }

        public override string Name
        {
            get { return "Grant read/write to '{0}' for queue '{1}'".FormatWith(_group, Address.ActualUri); }
        }

        public override DeploymentResult VerifyCanRun()
        {
            var result = new DeploymentResult();

            if (_server.IsLocal)
                VerifyInAdministratorRole(result);
            else
                result.AddAlert(string.Format("Cannot check for queue '{0}' on server '{1}' while on server '{2}'",
                                Address, _server.Name, Environment.MachineName));

            return result;
        }

        public override DeploymentResult Execute()
        {
            var result = new DeploymentResult();

            if (_server.IsLocal)
                ProcessLocalQueue(result);
            else
                ProcessRemoteQueue(result);


            return result;
        }

        void ProcessLocalQueue(DeploymentResult result)
        {
            var q = new MessageQueue(Address.FormatName);
            q.SetPermissions(_group, MessageQueueAccessRights.PeekMessage, AccessControlEntryType.Allow);
            q.SetPermissions(_group, MessageQueueAccessRights.ReceiveMessage, AccessControlEntryType.Allow);
            q.SetPermissions(_group, MessageQueueAccessRights.GetQueuePermissions, AccessControlEntryType.Allow);
            q.SetPermissions(_group, MessageQueueAccessRights.GetQueueProperties, AccessControlEntryType.Allow);
            q.SetPermissions(_group, MessageQueueAccessRights.WriteMessage, AccessControlEntryType.Allow);

            result.AddGood("Successfully granted Read/Write permissions to '{0}' for queue '{1}'".FormatWith(_group, Address.ActualUri));
        }

        void ProcessRemoteQueue(DeploymentResult result)
        {
            var message = "Cannot create a private queue on the remote machine '{0}' while on '{1}'."
                .FormatWith(_server.Name, Environment.MachineName);

            result.AddError(message);
        }


    }
}