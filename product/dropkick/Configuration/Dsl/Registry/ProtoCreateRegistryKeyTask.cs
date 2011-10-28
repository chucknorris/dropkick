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
		private readonly IList<ProtoCreateRegistryKeyTask> _subKeys = new List<ProtoCreateRegistryKeyTask>();
		private readonly IList<RegistryValue> _values = new List<RegistryValue>();

		public ProtoCreateRegistryKeyTask(RegistryHive hive, string name)
		{
			Guard.AgainstEmpty(name);
			_hive = hive;
			_name = ReplaceTokens(name);
		}

		/// <summary>
		/// Initializes a new instance of the ProtoCreateRegistryKeyTask class as a sub-key of the given parent key.
		/// </summary>
		private ProtoCreateRegistryKeyTask(ProtoCreateRegistryKeyTask parentKey, string subkey)
		{
			_hive = parentKey._hive;
			_name = System.IO.Path.Combine(parentKey._name, ReplaceTokens(subkey));
		}

		public RegistryKeyOptions CreateRegistryKey(string subKey)
		{
			var task = new ProtoCreateRegistryKeyTask(this, subKey);
			_subKeys.Add(task);
			return task;
		}

		public RegistryKeyOptions CreateRegistryKey(string subKey, Action<RegistryKeyOptions> options)
		{
			var task = CreateRegistryKey(subKey);
			if (options != null)
				options(task);
			return task;
		}

		public override void RegisterRealTasks(PhysicalServer server)
		{
			server.AddTask(new CreateRegistryKeyTask(server.Name, _hive, _name));
			_subKeys.Each(subKey => subKey.RegisterRealTasks(server));
			_values.Each(value =>
			             server.AddTask(new CreateRegistryValueTask(server.Name, _hive, _name, value.Name, value.ValueType,
			                                                    value.Value)));
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
