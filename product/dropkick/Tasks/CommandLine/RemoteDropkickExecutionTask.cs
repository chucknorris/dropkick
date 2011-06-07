using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using dropkick.Configuration.Dsl.Msmq;
using dropkick.DeploymentModel;
using dropkick.FileSystem;
using log4net;
using Path = System.IO.Path;

namespace dropkick.Tasks.CommandLine
{
    public class RemoteDropkickExecutionTask : IDisposable
    {
        //TODO: make this path configurable
        private const string _remotePath = @"C:\Temp\dropkick.remote";
        private readonly string _remotePhysicalPath;
        PhysicalServer _server;
        ILog _fineLog = LogManager.GetLogger("dropkick.finegrain");

        public RemoteDropkickExecutionTask(PhysicalServer server)
        {
            _server = server;
            _remotePhysicalPath = PathConverter.Convert(_server, _remotePath);
            CopyRemoteRunnerToServerIfNotExists();
        }

        private void CopyRemoteRunnerToServerIfNotExists()
        {
            //copy remote out

            var remotePath = _remotePhysicalPath;
            if (!Directory.Exists(remotePath)) Directory.CreateDirectory(remotePath);

            var ewd = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "dropkick.remote");
            Logging.Fine("Copying remote execution files from '{0}' to '{1}'", ewd, remotePath);

            var local = Path.GetDirectoryName(ewd);
            if (local == null) throw new Exception("shouldn't be null");

            var filesToCopy = Directory.GetFiles(ewd);
            foreach (var file in filesToCopy)
            {
                var dest = Path.Combine(remotePath, Path.GetFileName(file));
                var src = Path.Combine(local, file);
                _fineLog.DebugFormat("[remote] '{0}'->'{1}'", src, dest);

                try
                {
                    //TODO: This should get smarter about whether the file is the same or not.

                    // unsafe file copy (beats file locking that is showing up)
                    CopyFileA(src, dest, 0);
                    //File.Copy(src, dest, true);
                }
                catch (IOException ex)
                {
                    _fineLog.DebugFormat("[remote][file] Error copying '{0}' to '{1}'", file, remotePath);
                    throw;
                }
            }
        }

        [DllImport("kernel32")]
        private static extern int CopyFileA(string lpExistingFileName, string lpNewFileName, int bFailIfExists);


        public DeploymentResult VerifyQueueExists(QueueAddress path)
        {
            var t = SetUpRemote("verify_queue {0}".FormatWith(path.ActualUri));
            return ExecuteAndGetResults(t);
        }

        public DeploymentResult CreateQueue(QueueAddress path)
        {
            var t = SetUpRemote("create_queue {0}".FormatWith(path.ActualUri));
            return ExecuteAndGetResults(t);
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
            return ExecuteAndGetResults(t);
        }

        public DeploymentResult GrantReadCertificatePrivateKey(string group, string thumbprint, string storeName, string storeLocation)
        {
            var t = SetUpRemote("grant_cert r {0} {1} {2} {3}".FormatWith(group, thumbprint.Trim().Replace(" ", ""), storeName, storeLocation));
            return ExecuteAndGetResults(t);
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

        protected DeploymentResult ExecuteAndGetResults(RemoteCommandLineTask task)
        {
            string noteStatus = "[{0,-5}]".FormatWith(DeploymentItemStatus.Note);
            string goodStatus = "[{0,-5}]".FormatWith(DeploymentItemStatus.Good);
            string alertStatus = "[{0,-5}]".FormatWith(DeploymentItemStatus.Alert);
            string errorStatus = "[{0,-5}]".FormatWith(DeploymentItemStatus.Error);

            var vResult = task.Execute();

            string remoteLogPath = Path.Combine(_remotePhysicalPath, "dropkick.deployment.log");
            if (File.Exists(remoteLogPath))
            {
                var remoteLog = File.ReadAllText(remoteLogPath);
                var lines = Regex.Split(remoteLog, "\r\n");

                foreach (string line in lines)
                {
                    if (line.Trim() == string.Empty) continue;

                    //note: kind of nasty right now, but it does the job
                    if (line.Contains(noteStatus))
                    {
                        vResult.AddNote("[remote] {0}", line.Replace(noteStatus, string.Empty));
                    }
                    else if (line.Contains(goodStatus))
                    {
                        vResult.AddGood("[remote] {0}".FormatWith(line.Replace(goodStatus, string.Empty)));
                    }
                    else if (line.Contains(alertStatus))
                    {
                        vResult.AddAlert("[remote] {0}".FormatWith(line.Replace(alertStatus, string.Empty)));
                    }
                    else if (line.Contains(errorStatus))
                    {
                        vResult.AddError("[remote] {0}".FormatWith(line.Replace(errorStatus, string.Empty)));
                    }
                    else
                    {
                        Logging.Fine("[remote] {0}".FormatWith(line.Replace(noteStatus, string.Empty)));
                    }
                }
            }

            return vResult;
        }

        public void Dispose()
        {
            //Directory.Delete(_remotePhysicalPath, true);
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