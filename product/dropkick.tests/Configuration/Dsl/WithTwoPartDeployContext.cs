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
namespace dropkick.tests.Configuration.Dsl
{
    using dropkick.Configuration.Dsl;
    using dropkick.Engine;
    using NUnit.Framework;
    using TestObjects;

    [TestFixture]
    public abstract class WithTwoPartDeployContext
    {
        public TwoRoleDeploy Deployment { get; private set; }
        public DropkickDeploymentInspector Inspector { get; private set; }
        public RoleToServerMap Map { get; private set; }

        [TestFixtureSetUp]
        public void EstablishContext()
        {
            Deployment = new TwoRoleDeploy();
            Inspector = new DropkickDeploymentInspector();
            Map = new RoleToServerMap();
            Map.AddMap("Web", "SrvTopeka09");
            Map.AddMap("Web", "SrvTopeka19");
            Map.AddMap("Db", "SrvTopeka02");

            BecauseOf();
        }

        public abstract void BecauseOf();
    }
}