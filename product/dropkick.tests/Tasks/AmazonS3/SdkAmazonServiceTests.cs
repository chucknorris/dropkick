﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using dropkick.Tasks.AmazonS3;
using NUnit.Framework;

namespace dropkick.tests.Tasks.AmazonS3
{
	[TestFixture]
	public class SdkAmazonServiceTests
	{

		//[Test]
		//[Explicit]
		//public void TestBucketList()
		//{
		//	var sut = new RestAmazonService();
		//	var list = sut.GetBucketList(accessId, secretAccessKey);
		//	Assert.IsNotNull(list);
		//	Assert.IsNotEmpty(list);
		//	Assert.Contains(bucketName, list);
		//}

		private AmazonS3ConnectionInfo GetConnectionInfo()
		{
			string path = System.IO.Path.GetFullPath("AmazonS3ConnectionInfo.xml");
			if (!System.IO.File.Exists(path))
			{
				throw new Exception("Please create a settings file first at: " + path);
			}
			var serializer = new XmlSerializer(typeof(AmazonS3ConnectionInfo));
			using (var reader = new System.IO.StreamReader(path))
			{
				return (AmazonS3ConnectionInfo)serializer.Deserialize(reader);
			}
		}
		
		[Test]
		[Explicit]
		public void TestVerifyAccess()
		{
			var connectionInfo = GetConnectionInfo();
			var sut = new SdkAmazonService();
			bool access = sut.CheckBucketWriteAccess(connectionInfo.AccessId, connectionInfo.SecretAccessKey, connectionInfo.BucketName);
			Assert.IsTrue(access);
		}

		[Test] [Explicit]
		public void TestFileUpload()
		{
			var connectionInfo = GetConnectionInfo();
			var testFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".txt");
			StringBuilder sb = new StringBuilder();
			for(int i = 0; i < 100; i++)
			{
				sb.Append(Guid.NewGuid().ToString());
			}
			File.WriteAllText(testFilePath, sb.ToString());
			try 
			{
				var sut = new SdkAmazonService();
				string targetFilePath = Guid.NewGuid() + ".txt";
				sut.UploadFile(connectionInfo.AccessId, connectionInfo.SecretAccessKey, testFilePath, targetFilePath, connectionInfo.BucketName, null);
			}
			finally
			{
				try 
				{
					File.Delete(testFilePath);
				}
				catch {}
			}
		}
	}
}
