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
            
            Assembly asm = Assembly.LoadFile(FindFile(assemblyName));
            IEnumerable<Type> tt = asm.GetTypes().Where(t => typeof(Deployment).IsAssignableFrom(t));

            return new TypeWasSpecifiedAssumingItHasADefaultConstructor().Find(tt.First());
        }

        string FindFile(string file)
        {
            var p = Path.Combine(Environment.CurrentDirectory, file);
            if(!File.Exists(p)) throw new FileNotFoundException("Couldn't Find File",p);
            _log.DebugFormat("PATH: '{0}'", p);
            return p;
        }

        #endregion
    }
}