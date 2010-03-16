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

    public class CommandDeploymentEst :
        VerificationContext<CommandTestDeploy>
    {
    }

    public class MsmqDeploymentEst :
        VerificationContext<MsmqTestDeploy>
    {
    }

    public class MsSqlDeploymentEst :
        VerificationContext<MsSqlTestDeploy>
    {
    }

    public class TestDeploymentEst :
        VerificationContext<TestDeployment>
    {
    }

    public class WinService :
        VerificationContext<WinServiceTestDeploy>
    {
    }

    [TestFixture]
    public abstract class VerificationContext<T> where T : Deployment, new()
    {
        public Deployment Deployment { get; set; }

        DeploymentArguments _verifyArguments;

        [TestFixtureSetUp]
        public void EstablishContext()
        {
            Deployment = new T();
            _verifyArguments = new DeploymentArguments
            {
                Deployment = GetType().Assembly.FullName,
                Environment = "TEST",
                Command = DeploymentCommands.Verify,
                Role = "Web"
            };

            _verifyArguments.ServerMappings.AddMap("Web", "SrvTopeka02");

            DeploymentPlanDispatcher.KickItOutThereAlready(Deployment, _verifyArguments);
        }

        [Test]
        public void RunIt()
        {
            
        }
    }
}