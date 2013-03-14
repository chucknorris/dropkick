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

using System;
using dropkick.Configuration.Dsl.Files;

namespace dropkick.Configuration.Dsl.AmazonS3
{
	public static class Extension
	{
		public static CopyOptions AmazonS3UploadDirectory(this ProtoServer protoServer, string from)
		{
			return AmazonS3UploadDirectory(protoServer, o => o.Include(from));
		}

		public static CopyOptions AmazonS3UploadDirectory(this ProtoServer protoServer, Action<FromOptions> a)
		{
			var proto = new ProtoAmazonS3UploadDirectoryTask();
			a(proto);
			protoServer.RegisterProtoTask(proto);
			return proto;
		}
	}
}
