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
namespace dropkick.tests.Configuration.Dsl
{
    using dropkick.DeploymentModel;
    using NUnit.Framework;

    public class CheckingPlanBuildingLogic :
        WithTwoPartDeployContext
    {
        public DeploymentPlan Plan { get; set; }

        public override void BecauseOf()
        {
            Plan = Inspector.GetPlan(Deployment);
        }

        [Test]
        public void DbServerShouldBeNamedSrvTopeka02()
        {
            Assert.IsNotNull(Plan.GetRole("Db").GetServer("SrvTopeka02"));
        }

        [Test]
        public void DbServersShouldHaveOneTask()
        {
            Plan.GetRole("Db").ForEachServerMapped(s => Assert.AreEqual(1, s.DetailCount));
        }

        [Test]
        public void DbShouldHaveOneServer()
        {
            Assert.AreEqual(1, Plan.GetRole("Db").ServerCount);
        }

        [Test]
        public void PlanShouldHaveTwoRoles()
        {
            Assert.AreEqual(2, Plan.RoleCount, "Roles should be 2");
        }

        [Test]
        public void PlanShouldNotBeNull()
        {
            Assert.IsNotNull(Plan);
        }

        [Test]
        public void WebServersShouldHaveTwoTasksThatAreCopy()
        {
            Plan.GetRole("Web").ForEachServerMapped(s => Assert.AreEqual(2, s.DetailCount));
        }

        [Test]
        public void WebShouldHaveTwoServers()
        {
            Assert.AreEqual(2, Plan.GetRole("Web").ServerCount);
        }
    }
}