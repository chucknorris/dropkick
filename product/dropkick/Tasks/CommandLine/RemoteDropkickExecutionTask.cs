using System;
using System.IO;
using System.Reflection;
using dropkick.Configuration.Dsl.Msmq;
using dropkick.DeploymentModel;
using dropkick.FileSystem;
using log4net;
using Path = System.IO.Path;

namespace dropkick.Tasks.CommandLine
{
    public class RemoteDropkickExecutionTask : IDisposable
    {
        string _remoteRunnerPath;
        PhysicalServer _server;
        ILog _fineLog = LogManager.GetLogger("dropkick.finegrain");

        public RemoteDropkickExecutionTask(PhysicalServer server)
        {
            _server = server;
            CopyRemoteRunnerToServerIfNotExists();
        }

        private void CopyRemoteRunnerToServerIfNotExists()
        {
            //copy remote out
            //TODO: make this path configurable
            var remotePath = PathConverter.Convert(_server, @"C:\Temp\dropkick.remote");
            if (!Directory.Exists(remotePath)) Directory.CreateDirectory(remotePath);
            _remoteRunnerPath = remotePath;

            var ewd = Path.Combine(Assembly.GetExecutingAssembly().Location, "dropkick.remote");
            var local = Path.GetDirectoryName(ewd);

            if (local == null) throw new Exception("shouldn't be null");

            var filesToCopy = Directory.GetFiles(ewd);
            // var filesToCopy = new[] { "dropkick.remote.exe", "dropkick.dll", "log4net.dll", "Magnum.dll" };
            foreach (var file in filesToCopy)
            {
                var dest = Path.Combine(remotePath, file);
                var src = Path.Combine(local, file);
                _fineLog.DebugFormat("[remote] '{0}'->'{1}'", src, dest);

                try
                {
                    //NOTE: This should get smarter about whether the file is the same or not.
                    File.Copy(src, dest, true);
                }
                catch (IOException ex)
                {
                    _fineLog.DebugFormat("[remote][file] Error copying '{0}' to '{1}'", file, remotePath);
                    throw;
                }
            }
        }

        public DeploymentResult VerifyQueueExists(QueueAddress path)
        {
            var t = SetUpRemote("verify_queue {0}".FormatWith(path.ActualUri));
            return t.Execute();
        }

        public DeploymentResult CreateQueue(QueueAddress path)
        {
            var t = SetUpRemote("create_queue {0}".FormatWith(path.ActualUri));
            return t.Execute();
        }
        
        public DeploymentResult GrantMsmqPermission(QueuePermission permission, QueueAddress address, string @group)
        {
            string perm;
            switch (permission)
            {
                case QueuePermission.Read:
                    perm = "r";
                    break;
                case QueuePermission.Write:
                    perm = "w";
                    break;
                case QueuePermission.ReadWrite:
                    perm = "rw";
                    break;
                case QueuePermission.SetSensibleDefaults:
                    perm = "default";
                    break;
                default:
                    perm = "r";
                    break;
            }

            var t = SetUpRemote("grant_queue {0} {1} {2}".FormatWith(perm, @group, address.ActualUri));
            return t.Execute();
        }

        public DeploymentResult GrantReadCertificatePrivateKey(string group, string thumbprint,string storeName,string storeLocation)
        {
            var t = SetUpRemote("grant_cert r {0} {1} {2} {3}".FormatWith(group, thumbprint.Trim().Replace(" ", ""),storeName,storeLocation));
            return t.Execute();
        }

        protected RemoteCommandLineTask SetUpRemote(string arguments)
        {
            return new RemoteCommandLineTask("dropkick.remote.exe")
            {
                Args = arguments,
                ExecutableIsLocatedAt = @"C:\Temp\dropkick.remote\",
                Machine = _server.Name,
                WorkingDirectory = @"C:\Temp\dropkick.remote\"
            };
        }

        public void Dispose()
        {
            //Directory.Delete(_remoteRunnerPath, true);
        }
    }

    public enum QueuePermission
    {
        Read,
        Write,
        ReadWrite,
        SetSensibleDefaults
    }
}