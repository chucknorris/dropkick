namespace dropkick.Tasks.Files
{
    using System;
    using System.IO;
    using System.Text.RegularExpressions;
    using DeploymentModel;
    using Exceptions;
    using log4net;

    public class CopyFileTask :
        Task
    {
        private string _from;
        private string _to;
        readonly ILog _log = LogManager.GetLogger(typeof(CopyFileTask));
        readonly ILog _fileLog = LogManager.GetLogger("dropkick.filewrite");

        public CopyFileTask(string @from, string to)
        {
            _from = from;
            _to = to;
        }



        public string Name
        {
            get { return string.Format("Copy '{0}' to '{1}'", _from, _to); }
        }

        public DeploymentResult Execute()
        {
            var result = new DeploymentResult();

            ValidateIsFile(result, _from);
            ValidateIsDirectory(result, _to);

            _from = Path.GetFullPath(_from);
            _to = Path.GetFullPath(_to);


            CopyFile(new FileInfo(_from), new DirectoryInfo(_to));

            result.AddGood(Name);

            return result;
        }



        public DeploymentResult VerifyCanRun()
        {
            var result = new DeploymentResult();

            //TODO: make this not puke
            ValidateIsDirectory(result, _to);
            ValidateIsFile(result, _from);

            _from = Path.GetFullPath(_from);
            _to = Path.GetFullPath(_to);


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

        static readonly Regex _naiveFileRegex = new Regex(@".*\..+");
        void ValidateIsFile(DeploymentResult result, string path)
        {
            try
            {
                Path.GetFullPath(_to);
                if(!_naiveFileRegex.IsMatch(path))
                    throw new Exception("bob");
            }
            catch (Exception ex)
            {
                throw new DeploymentException("'{0}' is not an acceptable path. Must be a directory".FormatWith(_to));
            }
        }

        void ValidateIsDirectory(DeploymentResult result, string path)
        {
            try
            {
                Path.GetFullPath(_to);
                if (_naiveFileRegex.IsMatch(path))
                    throw new Exception("bob");
            }
            catch (Exception ex)
            {
                throw new DeploymentException("'{0}' is not an acceptable path. Must be a directory".FormatWith(_to));
            }
        }

        void CopyFile(FileInfo source, DirectoryInfo destination)
        {
            if (!destination.Exists)
            {
                destination.Create();
            }

            // Copy all files.
            
            var fileDestination = Path.Combine(destination.FullName,
                                               destination.Name);
            source.CopyTo(fileDestination);
            _log.DebugFormat("Copy file '{0}' to '{1}'", source.FullName, fileDestination);
            _fileLog.Info(fileDestination); //log where files are copied for tripwire
            
        }
    }
}