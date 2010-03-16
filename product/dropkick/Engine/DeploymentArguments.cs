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
namespace dropkick.Engine
{
    public class DeploymentArguments
    {
        public DeploymentArguments()
        {
            Command = DeploymentCommands.Trace;
            Role = "ALL";
            ServerMappings = new RoleToServerMap();
            SettingsDirectory = ".\\settings";
            Deployment = "SEARCH";
        }

        public string Environment { get; set; }
        public string Role { get; set; }
        public string Deployment { get; set; }
        public DeploymentCommands Command { get; set; }
        public RoleToServerMap ServerMappings { get; private set; }
        public string SettingsDirectory { get; set;}
    }
}