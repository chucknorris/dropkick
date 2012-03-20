namespace dropkick.Configuration.Dsl.Files
{
	using DeploymentModel;
	using FileSystem;
	using Tasks;
	using Tasks.Files;

	public class ProtoDeleteFileTask :
		BaseProtoTask
	{
		readonly string _file;

		public ProtoDeleteFileTask(string file)
		{
			_file = ReplaceTokens(file);
		}

		public override void RegisterRealTasks(PhysicalServer site)
		{
			string serverFileName = site.MapPath(_file);
			site.AddTask(new DeleteFileTask(serverFileName));
		}
	}
}
