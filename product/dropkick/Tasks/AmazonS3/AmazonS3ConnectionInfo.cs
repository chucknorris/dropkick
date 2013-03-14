using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dropkick.Tasks.AmazonS3
{
	public class AmazonS3ConnectionInfo
	{
		public string BucketName { get; set; }
		public string AccessId { get; set; }
		public string SecretAccessKey { get; set; }

		public string GetDescription()
		{
			return BucketName;
		}
	}
}
