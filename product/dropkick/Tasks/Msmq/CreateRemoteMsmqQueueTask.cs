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
    using CommandLine;
    using Configuration.Dsl.Msmq;
    using DeploymentModel;
    using FileSystem;
    using Path = System.IO.Path;

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
            var ub = new UriBuilder("msmq", server.Name) { Path = queueName };
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

           // using (var remote = new CopyRemoteOut(_server))
            //{
                //capture output
               // var vresult = remote.VerifyQueue(Address);
                result.AddAlert("REMOTE QUEUE - DID NOTHING");
            //}


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
                result.AddAlert("REMOTE QUEUE - DID NOTHING");
            }


            return result;
        }

        class CopyRemoteOut :
            IDisposable
        {
            string _path;
            PhysicalServer _server;
            public CopyRemoteOut(PhysicalServer server)
            {
                _server = server;

                //copy remote out
                //TODO: make this path configurable
                var p = RemotePathHelper.Convert(server, @"C:\Temp\dropkick.remote");
                if (!Directory.Exists(p)) Directory.CreateDirectory(p);
                _path = p;


                string a = Path.Combine(p, "dropkick.remote.exe");
                File.Copy(@".\dropkick.remote.exe", a, true);

                string b = Path.Combine(p, "dropkick.dll");
                File.Copy(@".\dropkick.dll", b, true);

                string c = Path.Combine(p,"log4net.dll");
                File.Copy(@".\log4net.dll", c, true);

                string d = Path.Combine(p, "Magnum.dll");
                File.Copy(@".\Magnum.dll", d, true);
            }


            public bool VerifyQueue(QueueAddress path)
            {
                //exe verify_queue path
                return false;
            }

            public DeploymentResult CreateQueue(QueueAddress path)
            {
                var t = new RemoteCommandLineTask("dropkick.remote.exe")
                    {
                        Args = "create_queue msmq://srvutilscm/bob",
                        ExecutableIsLocatedAt = @"C:\Temp\dropkick.remote\",
                        Machine = _server.Name,
                        WorkingDirectory = @"C:\Temp\dropkick.remote\"

                    };
                
                return t.Execute();
            }

            public void Dispose()
            {
                //Directory.Delete(_path, true);
            }
        }
    }
}