using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using dropkick.Tasks.AmazonS3;
using NUnit.Framework;
using Rhino.Mocks;

namespace dropkick.tests.Tasks.AmazonS3
{
	[TestFixture]
	public class AmazonS3Tests
	{
		[Test][Explicit]
		public void TestUploadDirectory()
		{
			var amazonService = MockRepository.GenerateMock<IAmazonService>();
			var tempDirectory = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString());
			try 
			{
				System.IO.Directory.CreateDirectory(tempDirectory);
				List<string> tempFiles = new List<string>();
				for(int i = 0; i < 100; i++)
				{
					string filePath = System.IO.Path.Combine(tempDirectory, "testfile_" + i.ToString());
					System.IO.File.WriteAllText(filePath, Guid.NewGuid().ToString());
					tempFiles.Add(filePath);
				}
				string subDirectory = System.IO.Path.Combine(tempDirectory,Guid.NewGuid().ToString());
				System.IO.Directory.CreateDirectory(subDirectory);
				for(int i = 0; i < 100; i++)
				{
					string filePath = System.IO.Path.Combine(subDirectory, "subDirectoryTestfile_" + i.ToString());
					System.IO.File.WriteAllText(filePath, Guid.NewGuid().ToString());
					tempFiles.Add(filePath);
				}


				var connectionInfo = new AmazonS3ConnectionInfo
				{
					AccessId = Guid.NewGuid().ToString(),
					SecretAccessKey = Guid.NewGuid().ToString(),
					BucketName = Guid.NewGuid().ToString()
				};
				string targetFolder = Guid.NewGuid().ToString();

				var sut = new AmazonS3UploadDirectoryTask(amazonService, connectionInfo, tempDirectory, targetFolder, null, null);
				sut.Execute();

				foreach (string filePath in tempFiles)
				{
					string targetFilePath = filePath.Substring(tempDirectory.Length + 1);
					targetFilePath = targetFilePath.Replace("\\", "/");
					amazonService.AssertWasCalled(i => i.UploadFile(connectionInfo.AccessId, connectionInfo.SecretAccessKey, filePath, targetFilePath, connectionInfo.BucketName, null));
				}
			}
			finally
			{
				try 
				{
					System.IO.Directory.Delete(tempDirectory);
				}
				catch{}
			}
		}

	}
}
