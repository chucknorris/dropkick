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
namespace dropkick.tests.Tasks.Msmq
{
    using System;
    using dropkick.DeploymentModel;
    using dropkick.Tasks.Msmq;
    using NUnit.Framework;

    [TestFixture]
    [Category("Integration")]
    public class MsmqTest
    {
        [Test]
        public void Execute()
        {
            var ps = new DeploymentServer(Environment.MachineName);
            var t = new MsmqTask(ps, "dk_test");
            t.Execute();
        }

        [Test]
        public void Verify()
        {
            var ps = new DeploymentServer(Environment.MachineName);
            var t = new MsmqTask(ps, "dk_test");
            DeploymentResult r = t.VerifyCanRun();
        }
    }
}