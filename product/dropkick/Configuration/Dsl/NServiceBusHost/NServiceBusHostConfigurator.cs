// Copyright 2007-2010 The Apache Software Foundation.
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace dropkick.Configuration.Dsl.NServiceBusHost
{
	using DeploymentModel;
	using FileSystem;
	using Tasks;
	using Tasks.NServiceBusHost;

	public class NServiceBusHostConfigurator :
		BaseProtoTask,
		NServiceBusHostOptions
	{
		readonly Path _path;
		string _serviceName;
		string _displayName;
		string _description;
		string _instanceName;
		string _location;
		string _exeName;
		string _password;
		string _username;

		public NServiceBusHostConfigurator(Path path)
		{
			_path = path;
		}

		public void ExeName(string name)
		{
			_exeName = name;
		}

		public void Instance(string name)
		{
			_instanceName = name;
		}

		public void LocatedAt(string location)
		{
			_location = location;
		}

		public void PassCredentials(string username, string password)
		{
			_username = username;
			_password = password;
		}

		public void ServiceName(string name)
		{
			_serviceName = name;
		}

		public void ServiceDisplayName(string name)
		{
			_displayName = name;
		}

		public void ServiceDescription(string description)
		{
			_description = description;
		}

		public override void RegisterRealTasks(PhysicalServer site)
		{
			var location = _path.GetPhysicalPath(site, _location, true);
			if (site.IsLocal)
			{
				site.AddTask(new LocalNServiceBusHostTask(_exeName, location, _instanceName, _username, _password, _serviceName, _displayName, _description));
			}
			else
			{
				site.AddTask(new RemoteNServiceBusHostTask(_exeName, location, _instanceName, site, _username, _password, _serviceName, _displayName, _description));
			}
		}
	}
}