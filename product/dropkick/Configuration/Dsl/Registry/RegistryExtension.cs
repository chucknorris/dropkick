using Microsoft.Win32;

namespace dropkick.Configuration.Dsl.Registry
{
	public static class RegistryExtension
	{
		public static string AsRegistryHiveString(this RegistryHive hive)
		{
			switch (hive)
			{
				case RegistryHive.ClassesRoot:
					return "HKEY_CLASSES_ROOT";
				case RegistryHive.CurrentConfig:
					return "HKEY_CURRENT_CONFIG";
				case RegistryHive.CurrentUser:
					return "HKEY_CURRENT_USER";
				case RegistryHive.LocalMachine:
					return "HKEY_LOCAL_MACHINE";
				case RegistryHive.Users:
					return "HKEY_USERS";
				default:
					return hive.ToString();
			}
		}

		public static string AsRegistryValueTypeString(this RegistryValueKind valueKind)
		{
			switch (valueKind)
			{
				case RegistryValueKind.DWord:
					return "REG_DWORD";
				case RegistryValueKind.ExpandString:
					return "REG_EXPAND_SZ";
				case RegistryValueKind.MultiString:
					return "REG_MULTI_SZ";
				case RegistryValueKind.String:
					return "REG_SZ";
				case RegistryValueKind.QWord:
					return "REG_QWORD";
				default:
					return valueKind.ToString();
			}
		}
	}
}
