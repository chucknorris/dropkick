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
using dropkick.Tasks.CommandLine;

namespace dropkick.Tasks.Security.Msmq
{
    using System;
    using System.Messaging;
    using Configuration.Dsl.Msmq;
    using DeploymentModel;
    using Tasks.Msmq;

    public class MsmqGrantWriteTask :
        BaseSecurityTask
    {
        readonly PhysicalServer _server;
        string _group;
        QueueAddress _address;

        public MsmqGrantWriteTask(QueueAddress address, string group)
        {
            _group = group;
            _address = address;
        }

        public MsmqGrantWriteTask(PhysicalServer server, QueueAddress address, string group)
        {
            _server = server;
            _address = address;
            _group = group;
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
            {
                Logging.Fine("[msmq] '{0}' is a local queue.", _address.ActualUri);
                ProcessLocalQueue(result);
            }
            else
            {
                Logging.Fine("[msmq][remote] '{0}' is a remote queue.", _address.ActualUri);
                ProcessRemoteQueue(result);
            }

            return result;
        }

        void ProcessLocalQueue(DeploymentResult result)
        {
            Logging.Coarse("[msmq] Setting permissions for '{0}' on local queue '{1}'", _group, _address.ActualUri);

            var q = new MessageQueue(_address.FormatName);
            q.SetPermissions(_group, MessageQueueAccessRights.GetQueuePermissions, AccessControlEntryType.Allow);
            q.SetPermissions(_group, MessageQueueAccessRights.GetQueueProperties, AccessControlEntryType.Allow);
            q.SetPermissions(_group, MessageQueueAccessRights.WriteMessage, AccessControlEntryType.Allow);

            result.AddGood("Successfully granted Write permissions to '{0}' for queue '{1}'".FormatWith(_group, _address.ActualUri));
        }

        void ProcessRemoteQueue(DeploymentResult result)
        {
            VerifyInAdministratorRole(result);
            Logging.Coarse("[msmq][remote] Setting permission for '{0}' on remote queue '{1}'.", _group, _address.ActualUri);

            using (var remote = new RemoteDropkickExecutionTask(_server))
            {
                //capture output
                var vresult = remote.GrantMsmqPermission(QueuePermission.Write, _address, _group);
                foreach (var r in vresult) result.Add(r);
            }

        }

    }
}