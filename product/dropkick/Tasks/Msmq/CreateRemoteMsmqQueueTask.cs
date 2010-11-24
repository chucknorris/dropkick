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
namespace dropkick.Tasks.Msmq
{
    using System;
    using System.IO;
    using Configuration.Dsl.Msmq;
    using DeploymentModel;
    using FileSystem;

    public class CreateRemoteMsmqQueueTask :
        BaseTask
    {
        readonly PhysicalServer _server;

        public CreateRemoteMsmqQueueTask(PhysicalServer server, QueueAddress address)
        {
            _server = server;
            Address = address;
        }

        public CreateRemoteMsmqQueueTask(PhysicalServer server, string queueName)
        {
            _server = server;
            var ub = new UriBuilder("msmq", server.Name) {Path = queueName};
            Address = new QueueAddress(ub.Uri);
        }

        public QueueAddress Address { get; set; }

        public override string Name
        {
            get
            {
                return string.Format("Create Remote MSMQ Queue for server '{0}' and private queue named '{1}'",
                                     _server.Name, Address);
            }
        }

        public override DeploymentResult VerifyCanRun()
        {
            var result = new DeploymentResult();

            VerifyInAdministratorRole(result);

            using (var remote = new CopyRemoteOut(_server))
            {
                //capture output
                var vresult = remote.VerifyQueue(Address);
                result.AddAlert("DID NOTHING");
            }


            return result;
        }

        public override DeploymentResult Execute()
        {
            var result = new DeploymentResult();

            VerifyInAdministratorRole(result);

            using (var remote = new CopyRemoteOut(_server))
            {
                //capture output
                var vresult = remote.CreateQueue(Address);
                result.AddAlert("DID NOTHING");
            }


            return result;
        }

        class CopyRemoteOut :
            IDisposable
        {
            string _path;
            public CopyRemoteOut(PhysicalServer server)
            {
                //copy remote out
                var p = RemotePathHelper.Convert(server, "E:\\dropkick.remote");
                p = System.IO.Path.Combine(p, "dropkick.remote.exe");
                File.Copy(".\\dropkick.remote.exe", p);
            }

            public bool VerifyQueue(QueueAddress path)
            {
                //exe verify_queue path
                return false;
            }

            public bool CreateQueue(QueueAddress path)
            {
                //exe create_queue path
                return false;
            }

            public void Dispose()
            {
                File.Delete(_path);
            }
        }
    }
}