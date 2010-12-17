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
namespace dropkick.tests.Tasks.Iis
{
    using System;
    using dropkick.DeploymentModel;
    using dropkick.Tasks.Iis;
    using Microsoft.Web.Administration;
    using NUnit.Framework;

    [TestFixture]
    public class NewVirtualDirTest
    {
        [Test, Explicit]
        public void Create_A_VirtualDiretory()
        {
            var task = new Iis7Task
                           {
                               PathOnServer = "D:\\SomethingAwesome",
                               ServerName = "localhost",
                               VdirPath = "Victastic",
                               WebsiteName = "Default Web Site",
                               AppPoolName = "VICKERS",
                               //could be set on either website or vdir basis
                               ManagedRuntimeVersion = ManagedRuntimeVersion.V4,
                               Enable32BitAppOnWin64 = true
                           };
            DeploymentResult output = task.Execute();

            foreach (var item in output.Results)
            {
                Console.WriteLine(item.Message);
            }
        }

        [Test, Explicit]
        public void Create_A_VirtualDiretory_withClassic()
        {
            var task = new Iis7Task
                           {
                               PathOnServer = "D:\\SomethingAwesome",
                               ServerName = "localhost",
                               VdirPath = "Victastic",
                               WebsiteName = "SCOTT",
                               AppPoolName = "VICKERS",
                               //could be set on either website or vdir basis
                               UseClassicPipeline = true
                           };
            DeploymentResult output = task.Execute();

            foreach (var item in output.Results)
            {
                Console.WriteLine(item.Message);
            }
        }


        [Test, Explicit]
        public void Create_An_AppPool()
        {
            ServerManager iis = ServerManager.OpenRemote("SrvTestWeb01");
            iis.ApplicationPools.Add("MATTYB");

            iis.CommitChanges();
        }
    }
}