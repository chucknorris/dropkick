using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using dropkick.DeploymentModel;
using dropkick.Tasks.Files;
using log4net;

namespace dropkick.Tasks.AmazonS3
{
	public class AmazonS3UploadDirectoryTask : BaseIoTask
	{
		readonly ILog _log = LogManager.GetLogger(typeof(AmazonS3UploadDirectoryTask));
		private AmazonS3ConnectionInfo _connectionInfo;
		private string _from;
		private string _targetFolder;
		private IAmazonService _amazonService;
		private IEnumerable<Regex> _copyIgnorePatterns;
		private AmazonAcl? _acl;

		public AmazonS3UploadDirectoryTask(IAmazonService amazonService, AmazonS3ConnectionInfo connectionInfo, string @from, string targetFolder, IEnumerable<Regex> copyIgnorePatterns, AmazonAcl? acl)
			: base(new dropkick.FileSystem.DotNetPath())
		{
			_amazonService = amazonService;
			_connectionInfo = connectionInfo;
			_from = from;
			_targetFolder = targetFolder;
			_copyIgnorePatterns = copyIgnorePatterns;
			_acl = acl;
		}

		public override string Name
		{
			get { return string.Format("Upload from '{0}' to Amazon S3 '{1}'", _from, _connectionInfo.GetDescription()); }
		}

		public AmazonAcl? Acl { get { return _acl; } }
		public string From { get { return _from; } }
		public string TargetFolder { get { return _targetFolder; } }
		public IEnumerable<Regex> IgnorePatterns { get { return _copyIgnorePatterns; } }
		public AmazonS3ConnectionInfo ConnectionInfo { get { return _connectionInfo; } }

		public override DeploymentResult VerifyCanRun()
		{
			var result = new DeploymentResult();

			ValidatePath(result, _from);

			_from = _path.GetFullPath(_from);

			DirectoryInfo fromDirectory = new DirectoryInfo(_from);
			if (fromDirectory.Exists)
			{
				result.AddGood(string.Format("'{0}' exists", fromDirectory.FullName));

				//check can read from _from
				FileInfo[] readFiles = fromDirectory.GetFiles();
				foreach (var file in readFiles.Where(f => !IsIgnored(_copyIgnorePatterns, f)))
				{
					Stream fs = new MemoryStream();
					try
					{
						fs = File.Open(file.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
						_log.DebugFormat("Going to copy '{0}' to '{1}'", file.FullName, _targetFolder);
					}
					catch (Exception)
					{
						result.AddAlert("AmazonS3UploadDirectoryTask: Can't read file '{0}'");
					}
					finally
					{
						fs.Dispose();
					}
				}
			}
			else
			{
				result.AddAlert(string.Format("'{0}' doesn't exist", _from));
			}

			bool writeAccess = _amazonService.CheckBucketWriteAccess(_connectionInfo.AccessId, _connectionInfo.SecretAccessKey, _connectionInfo.BucketName);
			if(writeAccess)
			{
				result.AddGood("User has write access to bucket " + _connectionInfo.BucketName);
			}
			else 
			{
				result.AddAlert("User does not have write access to bucket " + _connectionInfo.BucketName);
			}

			return result;
		}

		public override DeploymentResult Execute()
		{
			var result = new DeploymentResult();

			ValidatePath(result, _from);

			_from = _path.GetFullPath(_from);

			UploadDirectory(result, new DirectoryInfo(_from), _copyIgnorePatterns);

			result.AddGood(Name);

			return result;
		}

		protected void UploadDirectory(DeploymentResult result, DirectoryInfo source, IEnumerable<Regex> ignoredPatterns)
		{
			// Copy all files.
			FileInfo[] files = source.GetFiles();
			foreach (FileInfo file in files.Where(f => !IsIgnored(ignoredPatterns, f)))
			{
				string targetFile = file.FullName.Substring(_from.Length);
				if(targetFile.StartsWith("\\"))
				{
					targetFile = targetFile.Substring(1);
				}
				targetFile = targetFile.Replace("\\","/");
				if(!string.IsNullOrEmpty(_targetFolder))
				{
					if(_targetFolder.EndsWith("/"))
					{
						targetFile = _targetFolder + targetFile;
					}
					else 
					{
						targetFile = _targetFolder + "/" + targetFile;
					}
				}
				_amazonService.UploadFile(_connectionInfo.AccessId,_connectionInfo.SecretAccessKey, file.FullName, targetFile, _connectionInfo.BucketName, _acl);
			}

			// Process subdirectories.
			DirectoryInfo[] dirs = source.GetDirectories();
			foreach (var dir in dirs)
			{
				// Call CopyDirectory() recursively.
				UploadDirectory(result, dir, ignoredPatterns);
			}
		}
	}
}
