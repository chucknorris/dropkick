using dropkick.Configuration.Dsl.Registry;
using Microsoft.Win32;
using NUnit.Framework;

namespace dropkick.tests
{
	[Category("Registry")]
	[TestFixture()]
	public class When_dispaying_a_RegistryHive_as_a_string
	{
		[Test]
		[TestCase(RegistryHive.ClassesRoot, "HKEY_CLASSES_ROOT")]
		[TestCase(RegistryHive.CurrentConfig, "HKEY_CURRENT_CONFIG")]
		[TestCase(RegistryHive.CurrentUser, "HKEY_CURRENT_USER")]
		[TestCase(RegistryHive.LocalMachine, "HKEY_LOCAL_MACHINE")]
		[TestCase(RegistryHive.Users, "HKEY_USERS")]
		public void Then_RegistryHive_should_display_user_friendly_string(RegistryHive hive, string expectedString)
		{
			Assert.AreEqual(expectedString, hive.AsRegistryHiveString());
		}
	}

	[Category("Registry")]
	[TestFixture()]
	public class When_dispaying_a_RegistryValueKind_as_a_string
	{
		[Test]
		[TestCase(RegistryValueKind.DWord, "REG_DWORD")]
		[TestCase(RegistryValueKind.MultiString, "REG_MULTI_SZ")]
		[TestCase(RegistryValueKind.String, "REG_SZ")]
		[TestCase(RegistryValueKind.ExpandString, "REG_EXPAND_SZ")]
		[TestCase(RegistryValueKind.QWord, "REG_QWORD")]
		public void Then_RegistryValueKind_should_display_user_friendly_string(RegistryValueKind valueKind, string expectedString)
		{
			Assert.AreEqual(expectedString, valueKind.AsRegistryValueTypeString());
		}
	}
}
