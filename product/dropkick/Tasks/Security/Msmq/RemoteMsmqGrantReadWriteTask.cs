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
    using Configuration.Dsl.Msmq;
    using DeploymentModel;
    using Tasks.Msmq;

    public class RemoteMsmqGrantReadWriteTask :
        BaseSecurityTask
    {
        readonly string _group;
        readonly QueueAddress _address;
        readonly PhysicalServer _server;

        public RemoteMsmqGrantReadWriteTask(PhysicalServer server, QueueAddress address, string group)
        {
            _server = server;
            _group = group;
            _address = address;
        }

        public RemoteMsmqGrantReadWriteTask(PhysicalServer server, string queueName, string group)
        {
            _server = server;
            _group = group;
            var ub = new UriBuilder("msmq", server.Name) { Path = queueName };
            _address = new QueueAddress(ub.Uri);
        }

        public override string Name
        {
            get { return "Grant read/write to '{0}' for queue '{1}'".FormatWith(_group, _address.ActualUri); }
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

            Logging.Coarse("[msmq][remote] Setting permission for '{0}' on remote queue '{1}'.", _group, _address.ActualUri);

            using (var remote = new CopyRemoteOut(_server))
            {
                remote.GrantPermission(QueuePermission.ReadWrite, _address, _group);
            }

            return result;
        }


    }
}