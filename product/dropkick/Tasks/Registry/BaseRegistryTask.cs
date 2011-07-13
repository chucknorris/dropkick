using System.Text;
using dropkick.Configuration.Dsl.Registry;
using dropkick.DeploymentModel;
using Microsoft.Win32;

namespace dropkick.Tasks.Registry
{
	public abstract class BaseRegistryTask : BaseTask
	{
		protected RegistryHive Hive { get; set; }
		protected string Key { get; set; }
		protected string ServerName { get; set; }
		protected RegistryView RegistryView { get; set; }

		protected static string GetRegistryKeyDisplayString(RegistryHive hive, string key,
			string valueName = null, RegistryValueKind valueKind = RegistryValueKind.None)
		{
			var result = new StringBuilder();
			result.AppendFormat(@"{0}\{1}", hive.AsRegistryHiveString(), key);
			if (string.IsNullOrEmpty(valueName))
				return result.ToString();

			result.AppendFormat(@"\{0}", valueName);
			if (valueKind == RegistryValueKind.None)
				return result.ToString();

			result.AppendFormat(@":{0}", valueKind.AsRegistryValueTypeString());
			return result.ToString();
		}

		protected void VerifyRegistryHivePermissions(DeploymentResult result)
		{
			if (Hive == RegistryHive.CurrentUser)
			{
				result.AddNote("Elevated permissions not required for Registry Hive '{0}'", Hive.AsRegistryHiveString());
				return;
			}
		}

		protected RegistryKey OpenHive()
		{
			return RegistryKey.OpenRemoteBaseKey(Hive, ServerName, RegistryView);
		}
	}
}
