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
    using System.Reflection;
    using CommandLine;
    using Configuration.Dsl.Msmq;
    using DeploymentModel;
    using FileSystem;
    using log4net;
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

            using (var remote = new CopyRemoteOut(_server))
            {
                //capture output
                var vresult = remote.VerifyQueue(Address);
                result.AddAlert("REMOTE QUEUE - DID NOTHING");
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
                result.AddAlert("REMOTE QUEUE - DID NOTHING");
            }


            return result;
        }
    }

    public class CopyRemoteOut :
        IDisposable
    {
        string _path;
        PhysicalServer _server;
        ILog _fineLog = LogManager.GetLogger("dropkick.finegrain");

        public CopyRemoteOut(PhysicalServer server)
        {
            _server = server;

            //copy remote out
            //TODO: make this path configurable
            var remotePath = RemotePathHelper.Convert(server, @"C:\Temp\dropkick.remote");
            if (!Directory.Exists(remotePath)) Directory.CreateDirectory(remotePath);
            _path = remotePath;

            var ewd = Assembly.GetExecutingAssembly().Location;
            var local = Path.GetDirectoryName(ewd);

            if(local == null) throw new Exception("shouldn't be null");

            var filesToCopy = new[] {"dropkick.remote.exe","dropkick.dll","log4net.dll","Magnum.dll"};
            foreach (var file in filesToCopy)
            {
                var dest = Path.Combine(remotePath, file);
                var src = Path.Combine(local, file);
                _fineLog.DebugFormat("[msmq][remote] '{0}'->'{1}'", src, dest);

                File.Copy(src, dest, true);
            }
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
                            Args = "create_queue {0}".FormatWith(path.ActualUri),
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

        public DeploymentResult GrantReadWrite(QueueAddress address, string @group)
        {
            var t = new RemoteCommandLineTask("dropkick.remote.exe")
                        {
                            Args = "grant rw {0} {1}".FormatWith(@group, address.ActualUri),
                            ExecutableIsLocatedAt = @"C:\Temp\dropkick.remote\",
                            Machine = _server.Name,
                            WorkingDirectory = @"C:\Temp\dropkick.remote\"
                        };
            return t.Execute();
        }
    }
}