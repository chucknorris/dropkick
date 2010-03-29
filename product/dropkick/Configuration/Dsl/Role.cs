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
    using DeploymentModel;

    public interface Role
    {
        string Name { get; }
        void ConfigureServer(DeploymentServer server);
    }

    public class ServerRole :
        Role,
        DeploymentInspectorSite
    {
        readonly ProtoServer _protoServer = new PrototypicalServer();

        public ServerRole(string name)
        {
            Name = name;
        }

        #region DeploymentInspectorSite Members

        public void InspectWith(DeploymentInspector inspector)
        {
            inspector.Inspect(this, () => _protoServer.InspectWith(inspector));
        }

        #endregion

        #region Role Members

        public string Name { get; set; }

        public void ConfigureServer(DeploymentServer server)
        {
            _protoServer.MapTo(server);
        }

        #endregion

        public static ServerRole GetRole(Role input)
        {
            var result = input as ServerRole;
            if (result == null)
                throw new ArgumentException(string.Format("The role is not valid for this deployment"), "input");

            return result;
        }

        public void BindAction(Action<ProtoServer> action)
        {
            action(_protoServer);
        }
    }
}