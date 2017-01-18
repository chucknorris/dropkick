using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using dropkick.Configuration;
using dropkick.Configuration.Dsl.AmazonS3;
using dropkick.DeploymentModel;
using dropkick.Tasks.AmazonS3;
using NUnit.Framework;
using Rhino.Mocks;

namespace dropkick.tests.Configuration.Dsl
{
	[TestFixture]
	public class ProtoAmazonS3UploadDirectoryTaskTests
	{
		[SetUp]
		public void Bob()
		{
			HUB.Settings = new DropkickConfiguration()
			{
				Environment = "PROD"
			};
		}

		[Test]
		public void ShouldAddToDeploymentServer()
		{
			var sut = new ProtoAmazonS3UploadDirectoryTask();
			sut.From("C:\\Temp");
			var server = MockRepository.GenerateMock<PhysicalServer>();
			sut.RegisterRealTasks(server);
			server.AssertWasCalled(i=>i.AddTask(Arg<AmazonS3UploadDirectoryTask>.Is.Anything));
		}

		[Test]
		public void SetAcl_ShouldPassToTask()
		{
			var sut = new ProtoAmazonS3UploadDirectoryTask();
			sut.From("C:\\Temp");
			var acl = AmazonAcl.BucketOwnerFullControl;
			sut.WithAcl(acl);
			var server = MockRepository.GenerateMock<PhysicalServer>();
			sut.RegisterRealTasks(server);
			var args = server.GetArgumentsForCallsMadeOn(i => i.AddTask(Arg<AmazonS3UploadDirectoryTask>.Is.Anything));
			var task = (AmazonS3UploadDirectoryTask)args[0][0];
			Assert.AreEqual(acl, task.Acl);
		}

		[Test]
		public void SetFrom_ShouldPassToTask()
		{
			var sut = new ProtoAmazonS3UploadDirectoryTask();
			string from = "C:\\Temp";
			sut.From(from);
			var server = MockRepository.GenerateMock<PhysicalServer>();
			sut.RegisterRealTasks(server);
			var args = server.GetArgumentsForCallsMadeOn(i => i.AddTask(Arg<AmazonS3UploadDirectoryTask>.Is.Anything));
			var task = (AmazonS3UploadDirectoryTask)args[0][0];
			Assert.AreEqual(from, task.From);
		}

		[Test]
		public void SetTargetFolder_ShouldPassToTask()
		{
			var sut = new ProtoAmazonS3UploadDirectoryTask();
			string from = "C:\\Temp";
			sut.From(from);
			string targetFolder = Guid.NewGuid().ToString();
			sut.To(targetFolder);
			var server = MockRepository.GenerateMock<PhysicalServer>();
			sut.RegisterRealTasks(server);
			var args = server.GetArgumentsForCallsMadeOn(i => i.AddTask(Arg<AmazonS3UploadDirectoryTask>.Is.Anything));
			var task = (AmazonS3UploadDirectoryTask)args[0][0];
			Assert.AreEqual(targetFolder, task.TargetFolder);
		}

		[Test]
		public void SetCopyIgnorePatterns_ShouldPassToTask()
		{
			var sut = new ProtoAmazonS3UploadDirectoryTask();
			string from = "C:\\Temp";
			sut.From(from);
			var ignorePatterns = new List<Regex>() { new Regex("test1"), new Regex("test2") };
			sut.Exclude(ignorePatterns[0]);
			sut.Exclude(ignorePatterns[1]);
			var server = MockRepository.GenerateMock<PhysicalServer>();
			sut.RegisterRealTasks(server);
			var args = server.GetArgumentsForCallsMadeOn(i => i.AddTask(Arg<AmazonS3UploadDirectoryTask>.Is.Anything));
			var task = (AmazonS3UploadDirectoryTask)args[0][0];
			Assert.AreEqual(ignorePatterns[0], task.IgnorePatterns.First());
			Assert.AreEqual(ignorePatterns[1], task.IgnorePatterns.Skip(1).First());
		}

		[Test]
		public void SetConnectionInfo_ShouldPassToTask()
		{
			var sut = new ProtoAmazonS3UploadDirectoryTask();
			string from = "C:\\Temp";
			sut.From(from);
			var connectionInfo = new AmazonS3ConnectionInfo
			{
				AccessId = Guid.NewGuid().ToString(),
				SecretAccessKey = Guid.NewGuid().ToString(),
				BucketName = Guid.NewGuid().ToString()
			};
			sut.WithAuthentication(connectionInfo.AccessId, connectionInfo.SecretAccessKey);
			sut.WithBucket(connectionInfo.BucketName);
			var server = MockRepository.GenerateMock<PhysicalServer>();
			sut.RegisterRealTasks(server);
			var args = server.GetArgumentsForCallsMadeOn(i => i.AddTask(Arg<AmazonS3UploadDirectoryTask>.Is.Anything));
			var task = (AmazonS3UploadDirectoryTask)args[0][0];
			Assert.AreEqual(connectionInfo.AccessId, task.ConnectionInfo.AccessId);
			Assert.AreEqual(connectionInfo.SecretAccessKey, task.ConnectionInfo.SecretAccessKey);
			Assert.AreEqual(connectionInfo.BucketName, task.ConnectionInfo.BucketName);
		}
	}
}
