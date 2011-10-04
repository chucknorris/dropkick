using System;
using Microsoft.Win32;

namespace dropkick.Configuration.Dsl.Registry
{
	public interface RegistryKeyOptions
	{
		RegistryKeyOptions CreateRegistryKey(string name);
		RegistryKeyOptions CreateRegistryKey(string name, Action<RegistryKeyOptions> options);
		void CreateValue(string name, RegistryValueKind valueType, object value);
	}
}
