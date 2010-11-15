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

    public class MsmqGrantWriteTask :
        BaseTask
    {
        string _group;
        QueueAddress _address;

        public MsmqGrantWriteTask(QueueAddress address, string group)
        {
            _group = group;
            _address = address;
        }

        public MsmqGrantWriteTask(PhysicalServer server, string queueName, string group)
        {
            _group = group;
            var ub = new UriBuilder("msmq", server.Name) { Path = queueName };
            _address = new QueueAddress(ub.Uri);
        }



        public override string Name
        {
            get { return "Grant write to '{0}' for queue '{1}'".FormatWith(_group, _address.ActualUri); }
        }

        public override DeploymentResult VerifyCanRun()
        {
            var result = new DeploymentResult();

            if (_address.IsLocal)
                VerifyInAdministratorRole(result);
            else
                result.AddAlert("Cannot set permissions for the private remote queue '{0}' while on server '{1}'".FormatWith(_address.ActualUri, Environment.MachineName));

            return result;
        }

        public override DeploymentResult Execute()
        {
            var result = new DeploymentResult();
            if (_address.IsLocal)
                ProcessLocalQueue(result);
            else
                ProcessRemoteQueue(result);

            return result;
        }

        void ProcessLocalQueue(DeploymentResult result)
        {
            var q = new MessageQueue(_address.FormatName);
            q.SetPermissions(_group, MessageQueueAccessRights.GetQueuePermissions, AccessControlEntryType.Allow);
            q.SetPermissions(_group, MessageQueueAccessRights.GetQueueProperties, AccessControlEntryType.Allow);
            q.SetPermissions(_group, MessageQueueAccessRights.WriteMessage, AccessControlEntryType.Allow);

            result.AddGood("Successfully granted Write permissions to '{0}' for queue '{1}'".FormatWith(_group, _address.ActualUri));
        }

        void ProcessRemoteQueue(DeploymentResult result)
        {
            var message = "Cannot set permissions for the remote queue '{0}' while on server '{1}'.".FormatWith(_address.ActualUri, Environment.MachineName);

            result.AddError(message);
        }

    }
}