// Copyright 2007-2010 The Apache Software Foundation.
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
namespace dropkick.Engine.DeploymentFinders
{
    using System;
    using Configuration.Dsl;
    using log4net;
    using Magnum.Reflection;

    public class TypeWasSpecifiedAssumingItHasADefaultConstructor :
        DeploymentFinder
    {
        static readonly ILog _log = LogManager.GetLogger(typeof (TypeWasSpecifiedAssumingItHasADefaultConstructor));

        public Deployment Find(string typeName)
        {
            _log.DebugFormat("TYPE: '{0}'", typeName);

            Type type = Type.GetType(typeName);
            return Find(type);
        }

        public Deployment Find(Type type)
        {
            return (Deployment) Activator.CreateInstance(type);
        }

        public string Name
        {
            get { return "A Direct Type was Specified"; }
        }
    }
}