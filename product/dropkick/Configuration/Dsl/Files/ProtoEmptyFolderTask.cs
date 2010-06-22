namespace dropkick.Configuration.Dsl.Files
{
    using System;
    using DeploymentModel;
    using FileSystem;
    using Tasks;
    using Tasks.Files;

    public class ProtoEmptyFolderTask : BaseProtoTask
    {
        readonly string _folderName;

        public ProtoEmptyFolderTask(string folderName)
        {
            _folderName = folderName;
        }

        public override void RegisterRealTasks(PhysicalServer site)
        {
            var to = _folderName;
            if (!site.IsLocal)
                to = RemotePathHelper.Convert(site.Name, to);

            var task = new EmptyFolderTask(to, new DotNetPath());
            site.AddTask(task);
        }
    }
}