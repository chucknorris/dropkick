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
using dropkick.Tasks.AmazonS3;
namespace dropkick.Configuration.Dsl.AmazonS3
{
	public interface AmazonS3Options
	{
		AmazonS3Options WithBucket(string bucketName);
		AmazonS3Options WithAuthentication(string accessId, string secretAccessKey);
		AmazonS3Options WithAcl(AmazonAcl acl);
		AmazonS3Options To(string targetFolder);
	}
}
