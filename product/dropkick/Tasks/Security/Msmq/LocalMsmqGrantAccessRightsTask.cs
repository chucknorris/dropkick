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
    using Configuration.Dsl.Msmq;
    using DeploymentModel;
    using Security;

    public class LocalMsmqGrantAccessRightsTask : BaseSecurityTask
    {
        readonly QueueAddress _address;
        readonly string _user;
        readonly MessageQueueAccessRights _accessRights;

        public LocalMsmqGrantAccessRightsTask(QueueAddress address, string user, MessageQueueAccessRights accessRights)
        {
            _accessRights = accessRights;
            _address = address;
            _user = user;
        }

        public override string Name
        {
            get { return "Grant '{0}' access rights for queue '{1}' to '{2}'".FormatWith(_accessRights, _address.ActualUri, _user); }
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

            Logging.Coarse("[msmq] Setting '{0}' access rights for '{1}' on local queue '{2}'", _accessRights, _user, _address.ActualUri);

            var q = new MessageQueue(_address.FormatName);
            q.SetPermissions(_user, _accessRights, AccessControlEntryType.Allow);

            result.AddGood("Successfully granted '{0}' access rights to '{1}' for queue '{2}'".FormatWith(_accessRights, _user, _address.ActualUri));

            return result;
        }
    }
}