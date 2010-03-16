namespace dropkick.Configuration.Dsl
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;

    public interface Deployment :
        DeploymentInspectorSite
    {
        void Initialize(object settings);
    }

    public class Deployment<Inheritor, SETTINGS> :
        Deployment
        where Inheritor : Deployment<Inheritor, SETTINGS>, new()
    {
        readonly Dictionary<string, ServerRole> _roles = new Dictionary<string, ServerRole>();
        Action<SETTINGS> _definition;


        //this has replaced the static constructor
        public void Initialize(object settings)
        {
            InitializeParts();
            VerifyDeploymentConfiguration();

            Settings = (SETTINGS)settings;
            _definition(Settings);
        }

        void InitializeParts()
        {
            Type machineType = typeof(Inheritor);

            foreach(PropertyInfo propertyInfo in machineType.GetProperties(BindingFlags.Static | BindingFlags.Public))
            {
                if(IsNotARole(propertyInfo)) continue;

                ServerRole role = SetPropertyValue(propertyInfo, x => new ServerRole(x.Name));

                _roles.Add(role.Name, role);
            }
        }
        void VerifyDeploymentConfiguration()
        {
            if(_roles.Count == 0)
                throw new DeploymentConfigurationException("A deployment must have at least one role to be valid.");
        }

        //initial setup to be called in the static constructor
        protected void Define(Action<SETTINGS> definition)
        {
            _definition = definition;
        }

        //needs to be renamed
        protected void DeploymentStepsFor(Role inputRole, Action<Server> action)
        {
            var role = ServerRole.GetRole(inputRole);
            role.BindAction(action);
        }

        static TValue SetPropertyValue<TValue>(PropertyInfo propertyInfo, Func<PropertyInfo, TValue> getValue)
        {
            var value = Expression.Parameter(typeof(TValue), "value");
            var action = Expression.Lambda<Action<TValue>>(Expression.Call(propertyInfo.GetSetMethod(), value), new[] {value}).Compile();

            TValue propertyValue = getValue(propertyInfo);
            action(propertyValue);

            return propertyValue;
        }

        static bool IsNotARole(PropertyInfo propertyInfo)
        {
            return !(propertyInfo.PropertyType == typeof(ServerRole) || propertyInfo.PropertyType == typeof(Role));
        }

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

        public SETTINGS Settings { get; private set; }

    }
}