namespace dropkick.Tasks.Files
{
	using System.IO;
	using DeploymentModel;
	using Path = FileSystem.Path;

	public class DeleteFileTask : Task
	{
		string _fileName;

		public DeleteFileTask(string fileName)
		{
			_fileName = fileName;
		}

		public string Name
		{
			get { return "Deleting file '{0}'".FormatWith(_fileName); }
		}

		public DeploymentResult VerifyCanRun()
		{
			var result = new DeploymentResult();

			if (File.Exists(System.IO.Path.GetFileName(_fileName)))
			{
				result.AddGood(string.Format("{0} exists and will be deleted.", _fileName));
			}
			else
			{
				result.AddGood(string.Format("{0} does not exist, so cannot be deleted.", _fileName));
			}

			return result;
		}

		public DeploymentResult Execute()
		{
			var result = new DeploymentResult();

			if (File.Exists(_fileName))
			{
				File.Delete(_fileName);
				result.AddGood(string.Format("{0} exists and was deleted.", _fileName));
			}
			else
			{
				result.AddGood(string.Format("{0} does not exist, so cannot be deleted.", _fileName));
			}

			return result;
		}
	}
}
