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
		FromOptions,
		CopyOptions
	{
		private string _bucketName;
		private string _accessId;
		private string _secretAccessKey;
		readonly IList<string> _froms = new List<string>();
		private string _to;
		private readonly IList<Regex> _copyIgnorePatterns = new List<Regex>();


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

		public CopyOptions From(string sourcePath)
		{
			var p = ReplaceTokens(sourcePath);
			_froms.Add(p);
			return this;
		}

		public override void RegisterRealTasks(PhysicalServer server)
		{
			string to = server.MapPath(_to);

			foreach (var f in _froms)
			{
				var o = new AmazonS3UploadDirectoryTask(new SdkAmazonService(), null, f, to, _copyIgnorePatterns);
				server.AddTask(o);
			}
		}

		public CopyOptions To(string destinationPath)
		{
			throw new NotImplementedException();
		}

		public void DeleteDestinationBeforeDeploying()
		{
			throw new NotImplementedException();
		}

		public void ClearDestinationBeforeDeploying()
		{
			throw new NotImplementedException();
		}

		public void ClearDestinationBeforeDeploying(Action<IList<Regex>> excludePatterns)
		{
			throw new NotImplementedException();
		}
	}
}
