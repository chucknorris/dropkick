using Microsoft.Win32;
using NUnit.Framework;

namespace dropkick.tests.Registry
{
	public static class RegistryAssert
	{
		public static void KeyExists(RegistryHive hive, string key)
		{
			using (var reg = RegistryKey.OpenBaseKey(hive, RegistryView.Default))
				using (var subKey = reg.OpenSubKey(key))
				{
					Assert.IsNotNull(subKey);
				}
		}
	}
}
