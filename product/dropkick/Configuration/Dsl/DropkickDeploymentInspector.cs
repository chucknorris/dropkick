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
    using DeploymentModel;
    using Engine;
    using Magnum.Reflection;

    public class DropkickDeploymentInspector :
        ReflectiveVisitorBase<DropkickDeploymentInspector>,
        DeploymentInspector
    {
        readonly DeploymentPlan _plan = new DeploymentPlan();
        DeploymentRole _currentRole; //TODO: seems hackish
        RoleToServerMap _serverMappings;

        public DropkickDeploymentInspector() :
            base("Look")
        {
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
            _currentRole = _plan.AddRole(role.Name);

            foreach (var serverName in _serverMappings.GetServers(role.Name))
            {
                _currentRole.AddServer(serverName);
            }

            return true;
        }

        public bool Look(ProtoServer protoServer)
        {
            //TODO: implement
            return true;
        }

        public bool Look(ProtoTask protoTask)
        {
            //TODO: hackish
            _currentRole.ForEachServer(server =>
            {
                protoTask.RegisterTasks(server);
            });

            return true;
        }

        public DeploymentPlan GetPlan(Deployment deployment, RoleToServerMap serverMappings)
        {
            _serverMappings = serverMappings;

            deployment.InspectWith(this);

            return _plan;
        }
    }
}