// Copyright 2007-2008 The Apache Software Foundation.
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
namespace dropkick.Engine
{
    using System;
    using System.Collections.Generic;
    using DeploymentModel;
    using Magnum.Collections;

    public class RoleToServerMap
    {
        readonly MultiDictionary<string, DeploymentServer> _mappings;

        public RoleToServerMap()
        {
            _mappings = new MultiDictionary<string, DeploymentServer>(false, StringComparer.InvariantCultureIgnoreCase);
        }

        public void AddMap(string roleName, string serverName)
        {
            _mappings.Add(roleName, new DeploymentServer(serverName));
        }

        public ICollection<DeploymentServer> GetServers(string roleName)
        {
            return _mappings[roleName];
        }

        public void	 Merge(RoleToServerMap map)
        {
            foreach (var mapping in map._mappings)
            {
                foreach (var server in mapping.Value)
                {
                    _mappings[mapping.Key].Add(server);
                }
            }
        }

        public ICollection<string> Roles()
        {
            return _mappings.Keys;
        }
    }
}