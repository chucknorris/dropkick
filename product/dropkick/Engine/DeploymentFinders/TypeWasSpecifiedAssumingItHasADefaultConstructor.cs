namespace dropkick.Engine.DeploymentFinders
{
    using System;
    using Configuration.Dsl;
    using log4net;

    public class TypeWasSpecifiedAssumingItHasADefaultConstructor :
        DeploymentFinder
    {
        static readonly ILog _log = LogManager.GetLogger(typeof(TypeWasSpecifiedAssumingItHasADefaultConstructor));

        public Deployment Find(string typeName)
        {
            _log.DebugFormat("TYPE: '{0}'", typeName);

            var type = Type.GetType(typeName);
            return Find(type);
        }

        public Deployment Find(Type type)
        {
            return (Deployment)Activator.CreateInstance(type);
        }
    }
}