using dropkick.Configuration.Dsl.Registry;
using dropkick.DeploymentModel;
using Microsoft.Win32;

namespace dropkick.Tasks.Registry
{
	public class CreateRegistryValueTask : BaseRegistryTask
	{
		private string _valueName;
		private RegistryValueKind _valueType;
		private object _value;

		public CreateRegistryValueTask(string serverName, RegistryHive hive, string key, string valueName, RegistryValueKind valueType, object value)
		{
			ServerName = serverName;
			Hive = hive;
			Key = key;
			_valueName = valueName;
			_valueType = valueType;
			_value = value;
		}

		public override string Name
		{
			get { return string.Format(@"Create Registry value '{0}'", GetRegistryKeyDisplayString(Hive, Key, _valueName, _valueType)); }
		}

		public override DeploymentResult VerifyCanRun()
		{
			var result = new DeploymentResult();

			VerifyRegistryHivePermissions(result);
			verifyKeyExists(result);

			result.AddGood(Name);

			return result;
		}

		public override DeploymentResult Execute()
		{
			var result = new DeploymentResult();

			LogCoarseGrain("Opening or creating registry key '{0}' on '{1}'", GetRegistryKeyDisplayString(Hive, Key), ServerName);
			using (var regHive = OpenHive())
			using (var regKey = regHive.OpenSubKey(Key, true) ?? regHive.CreateSubKey(Key))
			{
				LogCoarseGrain("Setting registry value '{0}' of type '{1}'", _valueName, _valueType.AsRegistryValueTypeString());
				regKey.SetValue(_valueName, _value, _valueType);
			}

			result.AddGood(Name);
			return result;
		}

		private void verifyKeyExists(DeploymentResult result)
		{
			using (var regHive = OpenHive())
				using (var regKey = regHive.OpenSubKey(Key))
				{
					if (regKey == null)
						result.AddAlert(@"Registry Key '{0}' does not exist and will need to be created.", GetRegistryKeyDisplayString(Hive, Key));
				}
		}
	}
}
