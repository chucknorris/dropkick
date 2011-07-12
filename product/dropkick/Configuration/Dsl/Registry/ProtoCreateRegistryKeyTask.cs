using System;
using System.Collections.Generic;
using dropkick.DeploymentModel;
using dropkick.Tasks;
using dropkick.Tasks.Registry;
using Magnum;
using Magnum.Extensions;
using Microsoft.Win32;

namespace dropkick.Configuration.Dsl.Registry
{
	public class ProtoCreateRegistryKeyTask : BaseProtoTask, RegistryKeyOptions
	{
		private readonly RegistryHive _hive;
		private readonly string _name;
		private readonly IList<string> _subKeys = new List<string>();
		private readonly IList<RegistryValue> _values = new List<RegistryValue>();

		public ProtoCreateRegistryKeyTask(RegistryHive hive, string name)
		{
			Guard.AgainstEmpty(name);
			_hive = hive;
			_name = ReplaceTokens(name);
		}

		public override void RegisterRealTasks(PhysicalServer server)
		{
			server.AddTask(new CreateRegistryKeyTask(server.Name, _hive, _name));
			_subKeys.Each(subKey =>
			              server.AddTask(new CreateRegistryKeyTask(server.Name, _hive, System.IO.Path.Combine(_name, subKey))));
			_values.Each(value =>
			             server.AddTask(new CreateRegistryValueTask(server.Name, _hive, _name, value.Name, value.ValueType,
			                                                    value.Value)));
		}

		public RegistryKeyOptions CreateRegistryKey(string name)
		{
			_subKeys.Add(name);
			return this;
		}

		public RegistryKeyOptions CreateRegistryKey(string name, Action<RegistryKeyOptions> options)
		{
			_subKeys.Add(name);
			if (options != null)
				options(this);
			return this;
		}

		public void CreateValue(string name, RegistryValueKind valueType, object value)
		{
			_values.Add(new RegistryValue(name, valueType, value));
		}

		public class RegistryValue
		{
			public RegistryValue(string name, RegistryValueKind valueType, object value)
			{
				Name = name;
				ValueType = valueType;
				Value = value;
			}

			public string Name { get; private set; }
			public RegistryValueKind ValueType { get; private set; }
			public object Value { get; private set; }
		}
	}
}
