using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace dropkick.Tasks.AmazonS3
{
	public class SdkAmazonService : IAmazonService
	{
		public bool CheckBucketWriteAccess(string accessId, string secretAccessKey, string bucketName)
		{
			using(var client = new Amazon.S3.AmazonS3Client(accessId, secretAccessKey))
			{
				var request = new Amazon.S3.Model.GetACLRequest
				{
					BucketName = bucketName
				};
				var response = client.GetACL(request);
				Debug.Write(response.ToString());
				var anyAccess = (from i in response.AccessControlList.Grants
									where (i.Grantee.CanonicalUser.First == accessId
											|| (!string.IsNullOrEmpty(i.Grantee.URI)
													&& (i.Grantee.URI.Equals("http://acs.amazonaws.com/groups/global/AuthenticatedUsers", StringComparison.CurrentCultureIgnoreCase)
														|| i.Grantee.URI.Equals("http://acs.amazonaws.com/groups/global/AllUsers", StringComparison.CurrentCultureIgnoreCase)))
											) 
											&& (i.Permission == Amazon.S3.Model.S3Permission.FULL_CONTROL
													|| i.Permission == Amazon.S3.Model.S3Permission.WRITE)
									select i
								).Any();
				return anyAccess;
			}
		}

		public void UploadFile(string accessId, string secretAccessKey, string localFilePath, string targetFilePath, string bucketName, AmazonAcl? acl)
		{
			using(var client = new Amazon.S3.AmazonS3Client(accessId, secretAccessKey))
			{
				var request = new Amazon.S3.Model.PutObjectRequest
				{
					BucketName = bucketName,
					Key = targetFilePath,
					FilePath = localFilePath
				};
				if(acl.HasValue)
				{
					switch(acl.Value)
					{
						case AmazonAcl.AuthenticatedRead:
							request.CannedACL = Amazon.S3.Model.S3CannedACL.AuthenticatedRead;
							break;
						case AmazonAcl.BucketOwnerFullControl:
							request.CannedACL =  Amazon.S3.Model.S3CannedACL.BucketOwnerFullControl;
							break;
						case AmazonAcl.BucketOwnerRead:
							request.CannedACL = Amazon.S3.Model.S3CannedACL.BucketOwnerRead;
							break;
						case AmazonAcl.Private:
							request.CannedACL = Amazon.S3.Model.S3CannedACL.Private;
							break;
						case AmazonAcl.PublicRead:
							request.CannedACL = Amazon.S3.Model.S3CannedACL.PublicRead;
							break;
						case AmazonAcl.PublicReadWrite:
							request.CannedACL = Amazon.S3.Model.S3CannedACL.PublicReadWrite;
							break;
					}
				}
				var response = client.PutObject(request);
				
			}
		}

		public List<string> GetBucketList(string accessId, string secretAccessKey)
		{
			throw new NotImplementedException();
		}


	}
}
