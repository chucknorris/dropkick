namespace dropkick.Tasks.Files
{
    using System.IO;
    using DeploymentModel;
    using Path = FileSystem.Path;

    public class EmptyFolderTask : Task
    {
        string _to;
        Path _path;

        public EmptyFolderTask(string to, Path path)
        {
            _to = to;
            _path = path;
        }

        public string Name
        {
            get { return "Creating new empty folder '{0}'".FormatWith(_to); }
        }

        public DeploymentResult VerifyCanRun()
        {
            var result = new DeploymentResult();
            var to = _path.GetFullPath(_to);
            //TODO figure out a good verify step...
            result.AddGood(Directory.Exists(to) ? "'{0}' already exists.".FormatWith(to) : Name);
            return result;
        }

        public DeploymentResult Execute()
        {
            var result = new DeploymentResult();
            var to = _path.GetFullPath(_to);

            if (Directory.Exists(to))
                result.AddGood("'{0}' already exists.".FormatWith(to));
            else
            {
                result.AddGood(Name);
                Directory.CreateDirectory(to);
            }

            return result;
        }
    }
}