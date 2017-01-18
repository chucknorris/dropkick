using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace dropkick.Tasks.AmazonS3
{
	public class RestAmazonService : IAmazonService
	{
		public string BeginMultiPartUpload(string bucketName, string accessId, string secretAccessKey)
		{
			throw new NotImplementedException();
			//string url = string.Format("https://{0}.s3.amazonaws.com", bucketName);
			//var request = HttpWebRequest.Create("
		}


		public void UploadFile(string accessId, string secretAccessKey, string testFilePath, string targetFilePath, string bucketName, AmazonAcl? acl)
		{
			if(false)
			{
				//PUT https://s3.amazonaws.com/DropkickTestBucket/cf42a642-fb70-430c-b1f8-10f57fda4c68.txt HTTP/1.1
				//User-Agent: S3 Browser3-7-7
				//Authorization: AWS AKIAJNK54HGEVXB64SPA:On/fCt0RhOzN3P/AzSwljeuh8ro=
				//x-amz-date: Wed, 13 Mar 2013 21:26:37 GMT
				//x-amz-meta-cb-modifiedtime: Wed, 13 Mar 2013 21:21:34 GMT
				//Content-Type: text/plain
				//Host: s3.amazonaws.com
				//Content-Length: 20


				//test
				//test
				//test
				var request = (HttpWebRequest)WebRequest.Create("http://s3.amazonaws.com/DropkickTestBucket/cf42a642-fb70-430c-b1f8-10f57fda4c68.txt");
				request.Method = "PUT";

				WebHeaderCollection headers = (request as HttpWebRequest).Headers;
				request.UserAgent = "S3 Browser3-7-7";
				headers.Add("Authorization", "AWS AKIAJNK54HGEVXB64SPA:On/fCt0RhOzN3P/AzSwljeuh8ro=");
				headers.Add("x-amz-date", "Wed, 13 Mar 2013 21:26:37 GMT");
				headers.Add("x-amz-meta-cb-modifiedtime", "Wed, 13 Mar 2013 21:21:34 GMT");
				request.ContentType = "text/plain";
				request.ContentLength = 18;
				using(StreamWriter writer = new StreamWriter(request.GetRequestStream()))
				{
					writer.WriteLine("test");
					writer.WriteLine("test");
					writer.WriteLine("test");
				}
				using(var response = request.GetResponse())
				{
					Console.Write(new StreamReader(response.GetResponseStream()).ReadToEnd());
				}
				//Host: s3.amazonaws.com
			}
			else 
			{
				var fi = new FileInfo(testFilePath);
				var request = BuildRequest(accessId, secretAccessKey, "PUT", bucketName, targetFilePath);
				WebHeaderCollection headers = (request as HttpWebRequest).Headers;
				if (acl.HasValue)
				{
					string aclString = GetAclString(acl.Value);
					headers.Add("x-amz-acl", aclString);
				}
				headers.Add("x-amz-meta-cb-modifiedtime", fi.LastWriteTimeUtc.ToString("ddd, dd MMM yyyy HH:mm:ss ") + "GMT");
				request.ContentLength = fi.Length;
				request.ContentType = "text/plain";
				bool done = false;
				StringBuilder sb = new StringBuilder();
				sb.AppendLine("PUT");
				sb.AppendLine(request.ContentType);
				sb.AppendLine("x-amz-date:" + headers["x-amz-date"]);
				sb.AppendLine("x-amz-meta-cb-modifiedtime:" + headers["x-amz-meta-cb-modifiedtime"]);
				sb.Append("/" + bucketName + "/" + targetFilePath);
				Debug.WriteLine(sb.ToString());
				string authValue = GetAuthorizationHeader(accessId, secretAccessKey, sb.ToString());
				Debug.WriteLine(authValue);
				headers["Authorization"] = authValue;
				using(var fileStream = File.OpenRead(testFilePath))
				using(var requestStream = request.GetRequestStream())
				{
					WriteTo(fileStream, requestStream);
					requestStream.Flush();
				}

				using(var response = request.GetResponse())
				{
					Console.Write(new StreamReader(response.GetResponseStream()).ReadToEnd());
				}
			}
		}

		private void WriteTo(Stream sourceStream, Stream targetStream)
		{
			byte[] buffer = new byte[0x10000];
			int n;
			while ((n = sourceStream.Read(buffer, 0, buffer.Length)) != 0)
				targetStream.Write(buffer, 0, n);
		}

		private string GetAclString(AmazonAcl acl)
		{
			switch(acl)
			{
				case AmazonAcl.Private:
					return "private";
				case AmazonAcl.PublicRead:
					return "public-read";
				case AmazonAcl.PublicReadWrite:
					return "public-read-write";
				case AmazonAcl.AuthenticatedRead:
					return "authenticated-read";
				case AmazonAcl.BucketOwnerRead:
					return "bucket-owner-read";
				case AmazonAcl.BucketOwnerFullControl:
					return "bucket-owner-full-control";
				default:
					return null;
			}
		}

		public List<string> GetBucketList(string accessId, string secretAccessKey)
		{
			//Borrowed from: http://it.toolbox.com/blogs/daniel-at-work/simple-c-example-of-using-amazon-s3-27750

			HttpWebRequest request = BuildRequest(accessId, secretAccessKey, "GET");
			
			using(var response = request.GetResponse())
			using(var stream = response.GetResponseStream())
			{
				XDocument xdoc = XDocument.Load(stream);
				var bucketNames = (from b in xdoc.Descendants()
								where b.Name.LocalName == "Bucket"
								select b.Elements().Single(e=>e.Name.LocalName == "Name").Value).ToList();
				return bucketNames;
			}
		}

		private HttpWebRequest BuildRequest(string accessId, string secretAccessKey, string verb, string bucketName=null, string targetPath=null)
		{
			// here is the basic Http Web Request
			string url = "http://s3.amazonaws.com";
			if(!string.IsNullOrEmpty(bucketName))
			{
				url = url + "/" + bucketName;
			}
			if(!string.IsNullOrEmpty(targetPath))
			{
				if(targetPath.StartsWith("/"))
				{
					url += targetPath;
				}
				else 
				{
					url += "/" + targetPath;
				}
			}
			var request = WebRequest.Create(url) as HttpWebRequest;
			request.Method = verb;

			WebHeaderCollection headers = (request as HttpWebRequest).Headers;

			// the canonical string combines the request's data
			// with the current time
			string httpDate = DateTime.UtcNow.ToString("ddd, dd MMM yyyy HH:mm:ss ") + "GMT"; ;
			headers.Add("x-amz-date", httpDate);

			// our request is very simple, so we can hard-code the string
			string canonicalString = "GET\n\n\n\nx-amz-date:" + httpDate + "\n/";


			string headerValue = GetAuthorizationHeader(accessId, secretAccessKey, canonicalString);
			// finally, this is the Authorization header.
			headers.Add("Authorization", headerValue);

			return request;
		}

		private static string GetAuthorizationHeader(string accessId, string secretAccessKey, string canonicalString)
		{
			// now encode the canonical string
			Encoding ae = new UTF8Encoding();

			// create a hashing object
			HMACSHA1 signature = new HMACSHA1();
			// secretId is the hash key
			signature.Key = ae.GetBytes(secretAccessKey);
			byte[] bytes = ae.GetBytes(canonicalString);
			byte[] moreBytes = signature.ComputeHash(bytes);
			// convert the hash byte array into a base64 encoding
			string encodedCanonical = Convert.ToBase64String(moreBytes);

			return "AWS " + accessId + ":" + encodedCanonical;
		}

		public bool CheckBucketWriteAccess(string accessId, string secretAccessKey, string bucketName)
		{
			throw new NotImplementedException();
		}
	}
}
