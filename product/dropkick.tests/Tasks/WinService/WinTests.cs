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
namespace dropkick.tests.Tasks.WinService
{
    using System;
    using dropkick.DeploymentModel;
    using dropkick.Tasks.WinService;
    using NUnit.Framework;
    using Wmi;

    [TestFixture]
    public class WinTests
    {
        [Test]
        [Category("Integration")]
        public void Remote()
        {
            //TODO: friggin 2008 LUA-must run as admin
            var t = new WinServiceStopTask("SrvTestWeb01", "MSMQ");
            DeploymentResult o = t.VerifyCanRun();
            t.Execute();
            var t2 = new WinServiceStartTask("SrvTestWeb01", "MSMQ");
            t2.Execute();
        }

        [Test]
        [Category("Integration")]
        public void RemoteCreate()
        {
            var t = new WinServiceCreateTask("SrvTestWeb01", "FHLBank.Cue");
            t.ServiceLocation = "E:\\FHLBWinSvc\\Cue\\FHLBank.Cue.Host.exe";
            t.StartMode = ServiceStartMode.Automatic;

            DeploymentResult o = t.VerifyCanRun();
            t.Execute();
        }

        [Test]
        [Category("Integration")]
        public void RemoteDelete()
        {
            //TODO: friggin 2008 LUA-must run as admin
            var t = new WinServiceDeleteTask("SrvTestWeb01", "FHLBank.Cue");

            DeploymentResult o = t.VerifyCanRun();
            t.Execute();
        }

        [Test]
        [Category("Integration")]
        public void Start()
        {
            //TODO: friggin 2008 LUA-must run as admin
            var t = new WinServiceStopTask(Environment.MachineName, "MSMQ");
            DeploymentResult o = t.VerifyCanRun();
            t.Execute();
            var t2 = new WinServiceStartTask(Environment.MachineName, "MSMQ");
            t2.Execute();
        }
    }
}