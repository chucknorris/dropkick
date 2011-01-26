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

            if (local == null) throw new Exception("shouldn't be null");

            var filesToCopy = new[] { "dropkick.remote.exe", "dropkick.dll", "log4net.dll", "Magnum.dll" };
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

        public DeploymentResult GrantPermission(QueuePermission permission, QueueAddress address, string @group)
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

            var t = new RemoteCommandLineTask("dropkick.remote.exe")
            {
                Args = "grant {0} {1} {2}".FormatWith(perm, @group, address.ActualUri),
                ExecutableIsLocatedAt = @"C:\Temp\dropkick.remote\",
                Machine = _server.Name,
                WorkingDirectory = @"C:\Temp\dropkick.remote\"
            };
            return t.Execute();
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