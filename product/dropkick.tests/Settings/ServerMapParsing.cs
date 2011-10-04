using System.IO;
using System.Linq;
using dropkick.Settings;
using NUnit.Framework;

namespace dropkick.tests.Settings
{
	[TestFixture]
	public class ServerMapParsing
	{
		[Test]
		public void ParsesMultipleServersInOneRole()
		{
			File.WriteAllText(".\\test.map", "{Role1: ['server1', 'server2']}");

			var p = new ServerMapParser();
			var map = p.Parse(new FileInfo(".\\test.map"));

			var servers = map.GetServers("Role1");
			Assert.IsTrue(servers.Any(s => s.Name == "server1"), "Element 'server1' was not found in the collection");
			Assert.IsTrue(servers.Any(s => s.Name == "server2"), "Element 'server2' was not found in the collection");
			Assert.AreEqual(2, servers.Count);
		}
	}
}
