using System;
using dropkick.Configuration.Dsl.Registry;
using dropkick.DeploymentModel;
using Microsoft.Win32;

namespace dropkick.Tasks.Registry
{
	public class CreateRegistryKeyTask : BaseRegistryTask
	{
		public CreateRegistryKeyTask(string serverName, RegistryHive hive, string key)
		{
			ServerName = serverName;
			Hive = hive;
			Key = key;
		}

		public override string Name
		{
			get
			{
				return string.Format(@"Create Registry Key '{0}'", GetRegistryKeyDisplayString(Hive, Key)); 
			}
		}

		public override DeploymentResult VerifyCanRun()
		{
			var result = new DeploymentResult();

			VerifyRegistryHivePermissions(result);

			result.AddGood(Name);

			return result;
		}

		public override DeploymentResult Execute()
		{
			var result = new DeploymentResult();

			LogCoarseGrain("Opening registry hive '{0}' on '{1}'", Hive.AsRegistryHiveString(), ServerName);
			using (var regHive = RegistryKey.OpenRemoteBaseKey(Hive, ServerName))
				using (var regKey = regHive.CreateSubKey(Key))
				{
					if (regKey == null)
						throw new ApplicationException(String.Format("Creation of registry key '{0}' in hive '{1}' on '{2}' failed. ", Key, Hive, ServerName));
				}

			result.AddGood(Name);
			return result;
		}
	}
}
