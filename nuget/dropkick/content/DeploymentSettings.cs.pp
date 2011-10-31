// ==============================================================================
// 
// ACuriousMind and FerventCoder Copyright © 2011 - Released under the Apache 2.0 License
// 
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
// ==============================================================================
using dropkick.Configuration;
using dropkick.Tasks.RoundhousE;
using dropkick.Wmi;

namespace $rootnamespace$
{
    public class DeploymentSettings : DropkickConfiguration
    {
        
        #region Properties

        //directories
        public string WebsitePath { get; set; }
        public string HostServicePath { get; set; }

        //service info
        public ServiceStartMode ServiceStartMode { get; set; }
        public string ServiceUserName { get; set; }
        public string ServiceUserPassword { get; set; }

        //web stuff
        public string WebUserName { get; set; }
        public string WebUserPassword { get; set; }
        public string VirtualDirectorySite { get; set; }
        public string VirtualDirectoryName { get; set; }
        
        //database stuff 
        public string DbName { get; set; }
        public string DbSqlFilesPath { get; set; }
        public string DbRestorePath { get; set; }
        public DatabaseRecoveryMode DbRecoveryMode { get; set; }
        public RoundhousEMode RoundhousEMode { get; set; }

        #endregion
    }
}