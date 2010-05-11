namespace dropkick.Configuration.Dsl.Files
{
    using DeploymentModel;
    using Tasks;
    using Tasks.Files;
    using tests;

    public class ProtoCopyFileTask :
        BaseTask,
        FileCopyOptions
    {
        string _to;
        readonly string _from;

        public ProtoCopyFileTask(string @from)
        {
            _from = from;
        }

        public override void RegisterRealTasks(PhysicalServer site)
        {
            var to = _to;
            if (!site.IsLocal)
                to = RemotePathHelper.Convert(site.Name, to);


            var o = new CopyFileTask(_from, to);
            site.AddTask(o);

        }

        public FileCopyOptions ToDirectory(string destinationPath)
        {
            _to = destinationPath;
            return this;
        }
    }
}