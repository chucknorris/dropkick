using dropkick.Tasks.Registry;
using Microsoft.Win32;
using NUnit.Framework;

namespace dropkick.tests.Registry
{
	[TestFixture]
	[Category("Registry")]
	public class CreateRegistryKeyTaskTest
	{
		[Test]
		[Explicit]
		public void CanCreateRegistryKey()
		{
			const string REGISTRY_KEY = @"Software\DropKickTest";

			var sut = new CreateRegistryKeyTask("localhost", RegistryHive.CurrentUser, REGISTRY_KEY);
			var result = sut.Execute();
			
			Assert.IsFalse(result.ContainsError(), "DeploymentResult contains error: {0}", result.ResultsToList());
			RegistryAssert.KeyExists(RegistryHive.CurrentUser, REGISTRY_KEY);
			deleteRegistryKey(RegistryHive.CurrentUser, REGISTRY_KEY);
		}

		private void deleteRegistryKey(RegistryHive hive, string key)
		{
			using (var reg = RegistryKey.OpenBaseKey(hive, RegistryView.Default))
			{
				reg.DeleteSubKey(key);
			}
		}
	}
}
