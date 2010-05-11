
namespace dropkick.Tasks.Files
{
    using DeploymentModel;
    using System.IO;
    using Exceptions;
    using log4net;
    using Path = FileSystem.Path;

    public class RenameTask :
        Task
    {
        private string _from;
        private string _to;
        private Path _path;
        readonly ILog _log = LogManager.GetLogger(typeof(CopyFileTask));
        readonly ILog _fileLog = LogManager.GetLogger("dropkick.filewrite");

        public RenameTask(string from, string to, Path path)
        {
            _from = from;
            _to = to;
            _path = path;
        }

        public string Name
        {
            get { return @"Rename {0} to {1}.".FormatWith(_from, _to); }
        }

        public DeploymentResult VerifyCanRun()
        {
            var result = new DeploymentResult();

            ValidateFile(result, _from);
            ValidateFile(result, _to);

            _from = _path.GetFullPath(_from);
            _to = _path.GetFullPath(_to);

            if (File.Exists(_from))
            {
                result.AddGood(string.Format("'{0}' exists", _from));
            }
            else
            {
                result.AddError(string.Format("'{0}' doesn't exist", _from));
            }

            return result;
        }

        public DeploymentResult Execute()
        {
            var result = new DeploymentResult();

            ValidateFile(result, _from);
            ValidateFile(result, _to);

            _from = _path.GetFullPath(_from);
            _to = _path.GetFullPath(_to);


            RenameFile(new FileInfo(_from), new FileInfo(_to));

            result.AddGood(Name);

            return result;
        }

        private void RenameFile(FileInfo source, FileInfo destination)
        {
            source.MoveTo(destination.FullName);
            _log.DebugFormat("Rename file '{0}' destination '{1}'", source.FullName, destination.FullName);
            _fileLog.Info(destination); //log where files are copied for tripwire

        }

        void ValidateFile(DeploymentResult result, string path)
        {
            if (!_path.IsFile(path))
                throw new DeploymentException("'{0}' is not an acceptable path. Must be a file.".FormatWith(path));
        }
    }
}