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
    using System.Messaging;
    using System.Security.Principal;
    using Configuration.Dsl.Msmq;
    using DeploymentModel;
    using Security;

    public class LocalMsmqNServiceBusPermissionsTask : BaseSecurityTask
    {
        readonly QueueAddress _address;
        readonly string _user;

        public LocalMsmqNServiceBusPermissionsTask(QueueAddress address, string user)
        {
            _address = address;
            _user = user;
        }

        public override string Name
        {
            get { return "Grant NServiceBus permissions for queue '{0}' to '{1}'".FormatWith(_address.ActualUri, _user); }
        }

        public override DeploymentResult VerifyCanRun()
        {
            var result = new DeploymentResult();

            VerifyInAdministratorRole(result);

            return result;
        }

        public override DeploymentResult Execute()
        {
            var result = new DeploymentResult();

            Logging.Coarse("[msmq] Setting NServiceBus permissions for '{0}' on local queue '{1}'", _user, _address.ActualUri);

            var q = new MessageQueue(_address.FormatName);

            var localAdministratorsGroupName = new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null).Translate(typeof(NTAccount)).ToString();
            q.SetPermissions(localAdministratorsGroupName, MessageQueueAccessRights.FullControl, AccessControlEntryType.Allow);

            var localEveryoneGroupName = new SecurityIdentifier(WellKnownSidType.WorldSid, null).Translate(typeof(NTAccount)).ToString();
            q.SetPermissions(localEveryoneGroupName, MessageQueueAccessRights.WriteMessage, AccessControlEntryType.Allow);

            var localAnonymousLogonName = new SecurityIdentifier(WellKnownSidType.AnonymousSid, null).Translate(typeof(NTAccount)).ToString();
            q.SetPermissions(localAnonymousLogonName, MessageQueueAccessRights.WriteMessage, AccessControlEntryType.Allow);

            q.SetPermissions(_user, MessageQueueAccessRights.WriteMessage, AccessControlEntryType.Allow);
            q.SetPermissions(_user, MessageQueueAccessRights.ReceiveMessage, AccessControlEntryType.Allow);
            q.SetPermissions(_user, MessageQueueAccessRights.PeekMessage, AccessControlEntryType.Allow);

            result.AddGood("Successfully granted NServiceBus permissions to '{0}' for queue '{1}'".FormatWith(_user, _address.ActualUri));

            return result;
        }
    }
}