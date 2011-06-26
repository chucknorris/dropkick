using System;
using System.IO;
using dropkick.DeploymentModel;
using Ionic.Zip;
using log4net;
using Path = dropkick.FileSystem.Path;

namespace dropkick.Tasks.Files
{
	public class UnzipArchiveTask : BaseIoTask
	{
		readonly ILog _log = LogManager.GetLogger(typeof(UnzipArchiveTask));
		readonly DestinationCleanOptions _options;
		string _zipArchiveFilename;
		string _to;

		public UnzipArchiveTask(string @zipArchiveFilename, string to, DestinationCleanOptions options, Path path)
			: base(path)
        {
			_zipArchiveFilename = zipArchiveFilename;
            _to = to;
        }

		public override string Name
		{
			get { return string.Format("Unzip '{0}' to '{1}'", _zipArchiveFilename, _to); }
		}

		public override DeploymentResult Execute()
		{
			var result = new DeploymentResult();

			validatePaths(result);

			_zipArchiveFilename = _path.GetFullPath(_zipArchiveFilename);
			_to = _path.GetFullPath(_to);

			if (_options == DestinationCleanOptions.Delete) DeleteDestinationFirst(new DirectoryInfo(_to), result);

			unzipArchive(result, new DirectoryInfo(_to));

			result.AddGood(Name);

			return result;
		}

		public override DeploymentResult VerifyCanRun()
		{
			var result = new DeploymentResult();

			validatePaths(result);

			_zipArchiveFilename = _path.GetFullPath(_zipArchiveFilename);
			_to = _path.GetFullPath(_to);

			if (_options == DestinationCleanOptions.Delete)
				result.AddAlert("The files and directories in '{0}' will be deleted before deploying", _to);

			testArchive(result);

			return result;
		}

		private void validatePaths(DeploymentResult result)
		{
			ValidateIsFile(result, _zipArchiveFilename);
			ValidateIsDirectory(result, _to);
		}

		private void testArchive(DeploymentResult result)
		{
			if (!ZipFile.IsZipFile(_zipArchiveFilename))
				result.AddError(String.Format("The file '{0}' is not a valid zip archive.", _zipArchiveFilename));

			using (var zip = ZipFile.Read(_zipArchiveFilename))
			{
				result.AddGood("{0} items will be extracted from '{1}' to '{2}'", zip.Count, _zipArchiveFilename, _to);
			}
		}

		private void unzipArchive(DeploymentResult result, DirectoryInfo destination)
		{
			if (!destination.Exists)
				destination.Create();

			var count = 0;
			using (var zip = ZipFile.Read(_zipArchiveFilename))
			{
				zip.ExtractExistingFile = getExistingFileAction();
				foreach (var zipEntry in zip)
				{
					logUnzip("Unzipping '{0}'", zipEntry.FileName);
					zipEntry.Extract(destination.FullName);
					count++;
				}
			}
			logUnzip("{0} files unzipped", count);
		}

		private ExtractExistingFileAction getExistingFileAction()
		{
			return _options == DestinationCleanOptions.Delete
			       	? ExtractExistingFileAction.OverwriteSilently
			       	: ExtractExistingFileAction.Throw;
		}

		private void logUnzip(string message, params object[] args)
		{
			_log.DebugFormat(message, args);
		}
	}
}
