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
    using System.Linq.Expressions;
    using System.Reflection;
    using Exceptions;

    public interface Deployment :
        DeploymentInspectorSite
    {
        void Initialize(object settings);
        bool HardPrompt { get; }
        IEnumerable<string> Roles { get; }
    }

    public class Deployment<Inheritor, SETTINGS> :
        Deployment
        where Inheritor : Deployment<Inheritor, SETTINGS>, new()
        where SETTINGS : DropkickConfiguration
    {
        readonly Dictionary<string, ServerRole> _roles = new Dictionary<string, ServerRole>();
        Action<SETTINGS> _definition;
        public SETTINGS Settings { get; private set; }


        #region Deployment Members

        //this has replaced the static constructor
        public void Initialize(object settings)
        {
            InitializeParts();
            VerifyDeploymentConfiguration();

            Settings = (SETTINGS)settings;
            HUB.Settings = settings;
            _definition(Settings);
        }

        public string Environment { get; set; }

        public void InspectWith(DeploymentInspector inspector)
        {
            inspector.Inspect(this, () =>
            {
                foreach (ServerRole role in _roles.Values)
                {
                    role.InspectWith(inspector);
                }
            });
        }

        #endregion

        public IEnumerable<string> Roles
        {
            get { return _roles.Keys; }
        }

        public bool HardPrompt { get; private set; }

        void InitializeParts()
        {
            Type machineType = typeof(Inheritor);

            foreach (PropertyInfo propertyInfo in machineType.GetProperties(BindingFlags.Static | BindingFlags.Public))
            {
                if (IsNotARole(propertyInfo)) continue;

                ServerRole role = SetPropertyValue(propertyInfo, x => new ServerRole(x.Name));

                _roles.Add(role.Name, role);
            }
        }

        void VerifyDeploymentConfiguration()
        {
            if (_roles.Count == 0) throw new DeploymentConfigurationException("A deployment must have at least one role to be valid.");
        }

        //initial setup to be called in the static constructor
        protected void Define(Action<SETTINGS> definition)
        {
            _definition = definition;
        }

        //needs to be renamed
        protected void DeploymentStepsFor(Role inputRole, Action<ProtoServer> action)
        {
            var role = ServerRole.GetRole(inputRole);
            Settings.Role = role.Name;
            role.BindAction(action);
        }

        static TValue SetPropertyValue<TValue>(PropertyInfo propertyInfo, Func<PropertyInfo, TValue> getValue)
        {
            var value = Expression.Parameter(typeof(TValue), "value");
            var action = Expression.Lambda<Action<TValue>>(Expression.Call(propertyInfo.GetSetMethod(), value), new[] { value }).Compile();

            TValue propertyValue = getValue(propertyInfo);
            action(propertyValue);

            return propertyValue;
        }

        static bool IsNotARole(PropertyInfo propertyInfo)
        {
            return !(propertyInfo.PropertyType == typeof(ServerRole) || propertyInfo.PropertyType == typeof(Role));
        }


        public ICollection<ServerRole> GetRoles
        {
            get
            {
                return _roles.Values;
            }
        }

        protected void RequireHardConfirmation()
        {
            HardPrompt = true;
        }
    }
}