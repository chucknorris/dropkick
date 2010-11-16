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
namespace dropkick.Configuration.Dsl.Iis
{
    using DeploymentModel;
    using Tasks;
    using Tasks.Iis;

    public class IisProtoTask :
        BaseProtoTask,
        IisSiteOptions,
        IisVirtualDirectoryOptions
    {
        public IisProtoTask(string websiteName)
        {
            WebsiteName = websiteName;
        }

        public bool ShouldCreate { get; set; }
        public string WebsiteName { get; set; }
        public string VdirPath { get; set; }
        public IisVersion Version { get; set; }
        public string PathOnServer { get; set; }
        protected string AppPoolName { get; set; }
        public bool ClassicPipelineRequested { get; set; }
        public string ManagedRuntimeVersion { get; set; }
        public string PathForWebsite { get; set; }
        public int PortForWebsite { get; set; }

        public IisVirtualDirectoryOptions VirtualDirectory(string name)
        {
            VdirPath = ReplaceTokens(name);
            return this;
        }

        public IisVirtualDirectoryOptions SetPathTo(string path)
        {
            PathOnServer = ReplaceTokens(path);
            return this;
        }

        public IisVirtualDirectoryOptions UseClassicPipeline()
        {
            ClassicPipelineRequested = true;
            return this;
        }

        public IisVirtualDirectoryOptions SetAppPoolTo(string appPoolName)
        {
            AppPoolName = ReplaceTokens(appPoolName);
            return this;
        }

        public IisVirtualDirectoryOptions SetRuntimeToV2()
        {
            ManagedRuntimeVersion = Tasks.Iis.ManagedRuntimeVersion.V2;
            return this;
        }

        public IisVirtualDirectoryOptions SetRuntimeToV4()
        {
            ManagedRuntimeVersion = Tasks.Iis.ManagedRuntimeVersion.V4;
            return this;
        }

        public void CreateIfItDoesntExist()
        {
            ShouldCreate = true;
        }

        public override void RegisterRealTasks(PhysicalServer s)
        {
            if (Version == IisVersion.Six)
            {
                s.AddTask(new Iis6Task
                              {
                                  PathOnServer = PathOnServer,
                                  ServerName = s.Name,
                                  VdirPath = VdirPath,
                                  WebsiteName = WebsiteName,
                                  AppPoolName = AppPoolName
                              });
                return;
            }
            s.AddTask(new Iis7Task
                          {
                              PathOnServer = PathOnServer,
                              ServerName = s.Name,
                              VdirPath = VdirPath,
                              WebsiteName = WebsiteName,
                              AppPoolName = AppPoolName,
                              UseClassicPipeline = ClassicPipelineRequested,
                              ManagedRuntimeVersion = this.ManagedRuntimeVersion,
                              PathForWebsite = this.PathForWebsite,
                              PortForWebsite = this.PortForWebsite
                          });
        }

    }
}