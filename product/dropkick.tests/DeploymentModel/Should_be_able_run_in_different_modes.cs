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
namespace dropkick.tests.DeploymentModel
{
    using dropkick.DeploymentModel;
    using dropkick.Engine;
    using NUnit.Framework;

    [TestFixture]
    public class Should_be_able_run_in_different_modes
    {
        [Test]
        public void Execute()
        {
            bool verifyRan = false;
            bool executeRan = false;
            bool traceRan = false;

            var detail = new DeploymentDetail(() =>
            {
                traceRan = true;
                return "trace";
            }, () =>
            {
                verifyRan = true;
                var r = new DeploymentResult();
                r.AddGood("test:v");
                return r;
            }, () =>
            {
                executeRan = true;
                var r = new DeploymentResult();
                r.AddGood("test:e");
                return r;
            },
            ()=> new DeploymentResult());


            var web = new DeploymentRole("WEB");
            web.AddServer("BILL");
            web.ForEachServerMapped(s => s.AddDetail(detail));

            var plan = new DeploymentPlan();
            plan.AddRole(web);


            var args = new DeploymentArguments
                       {
                           Role = "WEB",
                           Command = DeploymentCommands.Execute
                       };

            plan.Execute();

            Assert.IsTrue(traceRan, "trace");
            Assert.IsTrue(verifyRan, "verify");
            Assert.IsTrue(executeRan, "execute");
        }

        [Test]
        public void Trace()
        {
            bool verifyRan = false;
            bool executeRan = false;
            bool traceRan = false;

            var detail = new DeploymentDetail(() =>
            {
                traceRan = true;
                return "trace";
            }, () =>
            {
                verifyRan = true;
                return new DeploymentResult();
            }, () =>
            {
                executeRan = true;
                return new DeploymentResult();
            }, ()=>new DeploymentResult());


            var web = new DeploymentRole("WEB");
            web.AddServer(new DeploymentServer("bob"));
            web.ForEachServerMapped(s => s.AddDetail(detail));

            var plan = new DeploymentPlan();
            plan.AddRole(web);


            var args = new DeploymentArguments
                       {
                           Role = "WEB",
                           Command = DeploymentCommands.Trace
                       };

            plan.Trace();

            Assert.IsTrue(traceRan);
            Assert.IsFalse(verifyRan);
            Assert.IsFalse(executeRan);
        }

        [Test]
        public void Verify()
        {
            bool verifyRan = false;
            bool executeRan = false;
            bool traceRan = false;

            var detail = new DeploymentDetail(() =>
            {
                traceRan = true;
                return "trace";
            }, () =>
            {
                verifyRan = true;
                return new DeploymentResult();
            }, () =>
            {
                executeRan = true;
                return new DeploymentResult();
            },()=>new DeploymentResult());


            var web = new DeploymentRole("WEB");
            web.AddServer("bob");
            web.ForEachServerMapped(s => s.AddDetail(detail));

            var plan = new DeploymentPlan();
            plan.AddRole(web);


            var args = new DeploymentArguments
                       {
                           Role = "WEB",
                           Command = DeploymentCommands.Verify
                       };

            plan.Verify();

            Assert.IsTrue(traceRan);
            Assert.IsTrue(verifyRan);
            Assert.IsFalse(executeRan);
        }
    }
}