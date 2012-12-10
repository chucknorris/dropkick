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
    using CommandLine;
    using Configuration.Dsl.Msmq;
    using DeploymentModel;
    using Tasks.Msmq;

    public class RemoteMsmqNServiceBusPermissionsTask :
        BaseSecurityTask
    {
        readonly string _user;
        readonly QueueAddress _address;
        readonly PhysicalServer _server;

        public RemoteMsmqNServiceBusPermissionsTask(PhysicalServer server, QueueAddress address, string user)
        {
            _server = server;
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

            //TODO add meaningful verification

            return result;
        }

        public override DeploymentResult Execute()
        {
            var result = new DeploymentResult();

            Logging.Coarse("[msmq][remote] Setting NServiceBus permissions for '{0}' on local queue '{1}'", _user, _address.ActualUri);

            using (var remote = new RemoteDropkickExecutionTask(_server))
            {
                remote.GrantMsmqNServiceBusPermissions(_address, _user);
            }

            return result;
        }
    }
}