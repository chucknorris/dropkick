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
            if (local == null) throw new Exception("Path to dk remote files should not be null");

            var filesToCopy = Directory.GetFiles(ewd);
            foreach (var file in filesToCopy)
            {
                var dest = Path.Combine(remotePath, Path.GetFileName(file));
                var src = Path.Combine(local, file);
                _fineLog.DebugFormat("[remote] '{0}'->'{1}'", src, dest);

                try
                {
                    //TODO: This should get smarter about whether the file is the same or not.
                    CopyFileWithNoLocking(src, dest);
                    //File.Copy(src, dest, true);
                }
                catch (IOException)
                {
                    _fineLog.DebugFormat("[remote][file] Error copying '{0}' to '{1}'", file, remotePath);
                    throw;
                }
            }
        }

        /// <summary>
        /// This does a low level copy of a file so it doesn't affect when a file is locked. It will overwrite an existing file
        /// </summary>
        /// <param name="source">The file you want to copy</param>
        /// <param name="destination">The place you want it to go</param>
        private void CopyFileWithNoLocking(string source,string destination)
        {
            CopyFileA(source, destination, 0);
        }

        [DllImport("kernel32")]
        private static extern int CopyFileA(string lpExistingFileName, string lpNewFileName, int bFailIfExists);

        public RemoteCommandLineTask SetUpRemote(string arguments)
        {
            return new RemoteCommandLineTask("dropkick.remote.exe")
            {
                Args = arguments,
                ExecutableIsLocatedAt = @"C:\Temp\dropkick.remote\",
                Machine = _server.Name,
                WorkingDirectory = @"C:\Temp\dropkick.remote\"
            };
        }

        public DeploymentResult ExecuteAndGetResults(RemoteCommandLineTask task)
        {
            string noteStatus = "[{0,-5}]".FormatWith(DeploymentItemStatus.Note);
            string goodStatus = "[{0,-5}]".FormatWith(DeploymentItemStatus.Good);
            string alertStatus = "[{0,-5}]".FormatWith(DeploymentItemStatus.Alert);
            string errorStatus = "[{0,-5}]".FormatWith(DeploymentItemStatus.Error);

            var vResult = task.Execute();

            string remoteLogPath = Path.Combine(_remotePhysicalPath, "dropkick.deployment.log");
            if (File.Exists(remoteLogPath))
            {
                var remoteLogLocalPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "dropkick.remote.deployment.log");
                CopyFileWithNoLocking(remoteLogPath, remoteLogLocalPath);

                var remoteLog = File.ReadAllText(remoteLogLocalPath);
                var serverName = _server.Name;
                var lines = Regex.Split(remoteLog, "\r\n");

                foreach (string line in lines)
                {
                    if (line.Trim() == string.Empty) continue;

                    //review: kind of nasty right now, but it does the job - v2 should use a TCP sink to communicate between logs
                    if (line.Contains(noteStatus))
                    {
                        vResult.AddNote("[remote:{0}] {1}".FormatWith(serverName,line.Replace(noteStatus, string.Empty)));
                    }
                    else if (line.Contains(goodStatus))
                    {
                        vResult.AddGood("[remote:{0}] {1}".FormatWith(serverName, line.Replace(goodStatus, string.Empty)));
                    }
                    else if (line.Contains(alertStatus))
                    {
                        vResult.AddAlert("[remote:{0}] {1}".FormatWith(serverName, line.Replace(alertStatus, string.Empty)));
                    }
                    else if (line.Contains(errorStatus))
                    {
                        vResult.AddError("[remote:{0}] {1}".FormatWith(serverName, line.Replace(errorStatus, string.Empty)));
                    }
                    else
                    {
                        Logging.Fine("[remote:{0}] {1}".FormatWith(serverName, line.Replace(noteStatus, string.Empty)));
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
}