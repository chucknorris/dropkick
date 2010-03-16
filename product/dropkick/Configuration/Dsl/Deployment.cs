namespace dropkick.Configuration.Dsl
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;
    using DeploymentModel;

    public interface Deployment :
        DeploymentInspectorSite
    {
    }

    public class Deployment<Inheritor> :
        Deployment
        where Inheritor : Deployment<Inheritor>, new()
    {
        static readonly Dictionary<string, Role<Inheritor>> _roles = new Dictionary<string, Role<Inheritor>>();

        static Deployment()
        {
            InitializeParts();
        }

        protected Deployment()
        {
            VerifyDeploymentConfiguration();
        }

        static void InitializeParts()
        {
            Type machineType = typeof(Inheritor);

            foreach(PropertyInfo propertyInfo in machineType.GetProperties(BindingFlags.Static | BindingFlags.Public))
            {
                if(IsNotARole(propertyInfo)) continue;

                Role<Inheritor> role = SetPropertyValue(propertyInfo, x => new Role<Inheritor>(x.Name));

                _roles.Add(role.Name, role);
            }
        }

        static void VerifyDeploymentConfiguration()
        {
            if(_roles.Count == 0)
                throw new DeploymentConfigurationException("A deployment must have at least one part to be valid.");
        }

        //initial setup to be called in the static constructor
        protected static void Define(Action definition)
        {
            definition();
        }

        //needs to be renamed
        protected static void DeploymentStepsFor(Role inputRole, Action<Server> action)
        {
            Role<Inheritor> role = Role<Inheritor>.GetRole(inputRole);
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
            return !(propertyInfo.PropertyType == typeof(Role<Inheritor>) || propertyInfo.PropertyType == typeof(Role));
        }

        public void InspectWith(DeploymentInspector inspector)
        {
            inspector.Inspect(this, () =>
            {
                foreach(Role<Inheritor> role in _roles.Values)
                {
                    role.InspectWith(inspector);
                }
            });
        }
    }
}