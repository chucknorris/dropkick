namespace dropkick.Tasks.Files
{
    using System;
    using System.IO;
    using DeploymentModel;
    using Exceptions;
    using log4net;
    using ExtensionsToString = Magnum.Extensions.ExtensionsToString;
    using Path = FileSystem.Path;

    public abstract class BaseIoTask :
        BaseTask
    {
        readonly ILog _fileLog = LogManager.GetLogger("dropkick.changes.files");
        protected readonly Path _path;

        protected BaseIoTask(Path path)
        {
            _path = path;
        }

        protected void ValidateIsFile(DeploymentResult result, string path)
        {
            if (!(new FileInfo(_path.GetFullPath(path)).Exists))
                result.AddAlert("'{0}' does not exist.".FormatWith(path));

            if (!_path.IsFile(path))
                result.AddError("'{0}' is not a file.".FormatWith(path));
        }

        protected void ValidateIsDirectory(DeploymentResult result, string path)
        {
            if (!(new DirectoryInfo(_path.GetFullPath(path)).Exists))
                result.AddAlert("'{0}' does not exist and will be created.".FormatWith(path));

            if (!_path.IsDirectory(path))
                result.AddError("'{0}' is not a directory.".FormatWith(path));
        }

        protected void ValidatePath(DeploymentResult result, string path)
        {
            try
            {
                if (!_path.IsDirectory(path))
                {
                    throw new DeploymentException("'{0}' is not an acceptable path. Must be a directory".FormatWith(path));
                }
            }
            catch (Exception)
            {
                throw new DeploymentException("'{0}' is not an acceptable path. Must be a directory".FormatWith(path));
            }
        }

        protected void CopyDirectory(DeploymentResult result, DirectoryInfo source, DirectoryInfo destination)
        {
            if (!destination.Exists)
            {
                destination.Create();
            }

            // Copy all files.
            FileInfo[] files = source.GetFiles();
            foreach (var file in files)
            {
                string fileDestination = _path.Combine(destination.FullName,file.Name);

                CopyFileToFile(result, file, new FileInfo(fileDestination));
            }

            // Process subdirectories.
            DirectoryInfo[] dirs = source.GetDirectories();
            foreach (var dir in dirs)
            {
                // Get destination directory.
                string destinationDir = _path.Combine(destination.FullName, dir.Name);

                // Call CopyDirectory() recursively.
                CopyDirectory(result, dir, new DirectoryInfo(destinationDir));
            }
        }

        protected static void DeleteDestinationFirst(DirectoryInfo directory, DeploymentResult result)
        {
            if (directory.Exists)
            {
                directory.Delete(true);
                result.AddGood("'{0}' was successfully deleted".FormatWith(directory.FullName));
                //TODO: a delete list?
            }
        }

        protected void CopyFile(DeploymentResult result, string newFileName, string from, string to)
        {
            if (ExtensionsToString.IsNotEmpty(newFileName))
                CopyFileToFile(result, new FileInfo(from), new FileInfo(_path.Combine(to,newFileName)));
            else
                CopyFileToDirectory(result, new FileInfo(from), new DirectoryInfo(to));
        }

        void CopyFileToFile(DeploymentResult result, FileInfo source, FileInfo destination)
        {
            var overwrite = destination.Exists;

            
            source.CopyTo(destination.FullName, true);

            if (overwrite)
                LogFineGrain("[copy][overwrite] '{0}' -> '{1}'", source.FullName, destination.FullName);
            else
                LogFineGrain("[copy] '{0}' -> '{1}'", source.FullName, destination.FullName);

            //TODO: Adjust for remote pathing
            _fileLog.Info(destination.FullName); //log where files are modified
        }

        void CopyFileToDirectory(DeploymentResult result, FileInfo source, DirectoryInfo destination)
        {
            if (!destination.Exists)
                destination.Create();

            var fileDestination = _path.Combine(destination.FullName, source.Name);

            var overwrite = _path.IsFile(fileDestination);

            source.CopyTo(fileDestination, true);

            if(overwrite)
                LogFineGrain("[copy][overwrite] '{0}' -> '{1}'", source.FullName, fileDestination);
            else
                LogFineGrain("[copy] '{0}' -> '{1}'", source.FullName, fileDestination);

            //TODO: Adjust for remote pathing
            _fileLog.Info(fileDestination);
        }
    }
}