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
namespace dropkick.Configuration.Dsl
{
    using System;
    using System.Collections.Generic;
    using DeploymentModel;
    using Engine;
    using Magnum.Reflection;
    using Magnum.Extensions;

    public class DropkickDeploymentInspector :
        ReflectiveVisitorBase<DropkickDeploymentInspector>,
        DeploymentInspector
    {
        readonly DeploymentPlan _plan = new DeploymentPlan();
        DeploymentRole _currentRole; //TODO: seems hackish
        readonly RoleToServerMap _serverMappings;
        readonly HashSet<string> _rolesOfInterest = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);


        public DropkickDeploymentInspector(RoleToServerMap maps) :
            base("Look")
        {
            _serverMappings = maps;
        }

        #region DeploymentInspector Members

        public void Inspect(object obj)
        {
            base.Visit(obj);
        }

        public void Inspect(object obj, ExposeMoreInspectionSites additionalInspections)
        {
            base.Visit(obj, () =>
            {
                additionalInspections();
                return true;
            });
        }

        #endregion

        public bool Look(Deployment deployment)
        {
            _plan.Name = deployment.GetType().Name;
            return true;
        }

        public bool Look(Role role)
        {
            if(ShouldNotProcessRole(role.Name))
                return false;

            _currentRole = _plan.AddRole(role.Name);

            foreach (var serverName in _serverMappings.GetServers(role.Name))
            {
                _currentRole.AddServer(serverName);
            }

            return true;
        }

        public bool Look(ProtoServer protoServer)
        {
            return true;
        }

        public bool Look(ProtoTask protoTask)
        {
            //TODO: hackish
            _currentRole.ForEachServerMapped(server =>
            {
                protoTask.RegisterRealTasks(server);
            });

            return true;
        }

        public DeploymentPlan GetPlan(Deployment deployment)
        {
            deployment.InspectWith(this);

            return _plan;
        }

        bool ShouldNotProcessRole(string role)
        {
            if(_rolesOfInterest.Count == 0) return false;

            return !_rolesOfInterest.Contains(role);
        }

        public void RolesToGet(params string[] roles)
        {
            roles.Each(r => _rolesOfInterest.Add(r));
        }
    }
}