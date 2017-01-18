// Copyright 2007-2010 The Apache Software Foundation.
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.


namespace dropkick.Configuration.Dsl.AmazonS3
{
	using System;
	using System.Collections.Generic;
	using System.Text.RegularExpressions;
	using dropkick.Configuration.Dsl.Files;
	using dropkick.DeploymentModel;
	using dropkick.Tasks;
	using dropkick.Tasks.AmazonS3;

	public class ProtoAmazonS3UploadDirectoryTask :
		BaseProtoTask,
		AmazonS3Options,
		FromOptions
	{
		private string _bucketName;
		private string _accessId;
		private string _secretAccessKey;
		readonly IList<string> _froms = new List<string>();
		private string _to = string.Empty;
		private readonly IList<Regex> _copyIgnorePatterns = new List<Regex>();
		private AmazonAcl? _acl;


		public void Include(string path)
		{
			var p = ReplaceTokens(path);
			_froms.Add(p);
		}

		public void Exclude(string pattern)
		{
			Exclude(new Regex(pattern));
		}

		public void Exclude(Regex pattern)
		{
			_copyIgnorePatterns.Add(pattern);
		}

		public AmazonS3Options From(string sourcePath)
		{
			var p = ReplaceTokens(sourcePath);
			_froms.Add(p);
			return this;
		}

		public AmazonS3Options WithAcl(AmazonAcl acl)
		{
			_acl = acl;
			return this;
		}

		public override void RegisterRealTasks(PhysicalServer server)
		{
			foreach (var f in _froms)
			{
				var service = new SdkAmazonService();
				var connectionInfo = new AmazonS3ConnectionInfo
				{
					AccessId = _accessId,
					SecretAccessKey = _secretAccessKey,
					BucketName = _bucketName
				};
				var o = new AmazonS3UploadDirectoryTask(service, connectionInfo, f, _to, _copyIgnorePatterns, _acl);
				server.AddTask(o);
			}
		}

		public AmazonS3Options WithBucket(string bucketName)
		{
			_bucketName = bucketName;
			return this;
		}

		public AmazonS3Options WithAuthentication(string accessId, string secretAccessKey)
		{
			_accessId = accessId;
			_secretAccessKey = secretAccessKey;
			return this;
		}

		public AmazonS3Options To(string targetFolder)
		{
			_to = targetFolder;
			return this;
		}
	}
}
