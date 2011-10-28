using System;
using Microsoft.Win32;

namespace dropkick.Configuration.Dsl.Registry
{
	public static class Extension
	{
		public static RegistryKeyOptions CreateRegistryKey(this ProtoServer protoServer, RegistryHive hive, string name)
		{
			var proto = new ProtoCreateRegistryKeyTask(hive, name);
			protoServer.RegisterProtoTask(proto);
			return proto;
		}

		public static RegistryKeyOptions CreateRegistryKey(this ProtoServer protoServer, RegistryHive hive, string name, Action<RegistryKeyOptions> options)
		{
			var proto = CreateRegistryKey(protoServer, hive, name);
			if (options != null)
				options(proto);
			return proto;
		}
	}
}
