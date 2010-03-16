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
namespace dropkick.Configuration.Dsl.Iis
{
    using System.IO;
    using DeploymentModel;
    using Tasks;
    using Tasks.Iis;

    public class IisProtoTask :
        BaseTask,
        IisSiteOptions,
        IisVirtualDirectoryOptions
    {
        readonly Server _server;

        public IisProtoTask(Server server, string websiteName)
        {
            _server = server;
            _server.RegisterTask(this);
            WebsiteName = websiteName;
        }

        public bool ShouldCreate { get; protected set; }
        public string WebsiteName { get; set; }
        public string VdirPath { get; set; }
        public IisVersion Version { get; set; }
        public DirectoryInfo PathOnServer { get; set; }

        #region IisSiteOptions Members

        public IisVirtualDirectoryOptions VirtualDirectory(string name)
        {
            VdirPath = name;
            return this;
        }

        #endregion

        #region IisVirtualDirectoryOptions Members

        public void CreateIfItDoesntExist()
        {
            ShouldCreate = true;
        }

        #endregion

        public override Task ConstructTasksForServer(DeploymentServer server)
        {
            if (Version == IisVersion.Six)
            {
                return new Iis6Task()
                       {
                           PathOnServer = PathOnServer,
                           ServerName = server.Name,
                           VdirPath = VdirPath,
                           WebsiteName = WebsiteName
                       };
            }

            return new Iis7Task()
                   {
                       PathOnServer = PathOnServer,
                       ServerName = server.Name,
                       VdirPath = VdirPath,
                       WebsiteName = WebsiteName
                   };
        }
    }
}