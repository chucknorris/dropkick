namespace dropkick.Configuration.Dsl
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;

    public interface Deployment :
        DeploymentInspectorSite
    {
    }

    public class Deployment<Inheritor, CONFIG> :
        Deployment
        where Inheritor : Deployment<Inheritor, CONFIG>, new()
    {
        static readonly Dictionary<string, Role<Inheritor, CONFIG>> _roles = new Dictionary<string, Role<Inheritor,CONFIG>>();

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

                Role<Inheritor,CONFIG> role = SetPropertyValue(propertyInfo, x => new Role<Inheritor,CONFIG>(x.Name));

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
            Role<Inheritor, CONFIG> role = Role<Inheritor,CONFIG>.GetRole(inputRole);
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
            return !(propertyInfo.PropertyType == typeof(Role<Inheritor, CONFIG>) || propertyInfo.PropertyType == typeof(Role));
        }

        public void InspectWith(DeploymentInspector inspector)
        {
            inspector.Inspect(this, () =>
            {
                foreach (Role<Inheritor, CONFIG> role in _roles.Values)
                {
                    role.InspectWith(inspector);
                }
            });
        }

        public static CONFIG Configuration { get; private set; }
    }
}