using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dropkick.Tasks.AmazonS3
{
	public enum AmazonAcl
	{
		Private,
		PublicRead,
		PublicReadWrite,
		AuthenticatedRead,
		BucketOwnerRead,
		BucketOwnerFullControl
	};

	public interface IAmazonService
	{
		void UploadFile(string accessId, string secretAccessKey, string localFilePath, string targetFilePath, string bucketName, AmazonAcl? acl);
		List<string> GetBucketList(string accessId, string secretAccessKey);
		bool CheckBucketWriteAccess(string accessId, string secretAccessKey, string bucketName);
	}
}
