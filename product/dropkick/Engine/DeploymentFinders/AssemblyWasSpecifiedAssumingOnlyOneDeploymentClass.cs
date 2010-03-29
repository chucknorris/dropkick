namespace dropkick.Engine.DeploymentFinders
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Configuration.Dsl;
    using log4net;

    public class AssemblyWasSpecifiedAssumingOnlyOneDeploymentClass :
        DeploymentFinder
    {
        static readonly ILog _log = LogManager.GetLogger(typeof(AssemblyWasSpecifiedAssumingOnlyOneDeploymentClass));

        #region DeploymentFinder Members

        public Deployment Find(string assemblyName)
        {
            //check that it is an assembly

            var path = FindFile(assemblyName);

            Assembly asm = Assembly.LoadFile(path);
            IEnumerable<Type> tt = asm.GetTypes().Where(t => typeof(Deployment).IsAssignableFrom(t));

            return new TypeWasSpecifiedAssumingItHasADefaultConstructor().Find(tt.First());
        }

        string FindFile(string file)
        {
            var p = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, file);
            _log.DebugFormat("Looking for deployment dll '{0}' at '{1}'", file, p);
            
            if(!File.Exists(p))
                throw new FileNotFoundException("Couldn't Find File",p);
            
            return p;
        }

        #endregion
    }
}