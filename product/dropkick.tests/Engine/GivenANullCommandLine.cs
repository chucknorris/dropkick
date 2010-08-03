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
namespace dropkick.tests.Engine
{
    using dropkick.Engine;
    using NUnit.Framework;

    [TestFixture]
    public class GivenANullCommandLine
    {
        const string _null_commandline = "";
        [Test]
        public void Default_role_should_be_ALL()
        {
            var ea = DeploymentCommandLineParser.Parse(_null_commandline);

            Assert.AreEqual("ALL", ea.Role);
        }

        [Test]
        public void Default_should_be_trace()
        {
            var ea = DeploymentCommandLineParser.Parse(_null_commandline);

            Assert.AreEqual(DeploymentCommands.Trace, ea.Command);
        }


    }

    [TestFixture]
    public class GivenACompleteCommandLine
    {


        const string _arguments = "verify -environment:staging -deployment:MyStuff.dll -part:WEB";

        [Test]
        public void Should_parse_out_execute()
        {
            var arguments = "execute";
            var ea = DeploymentCommandLineParser.Parse(arguments);

            Assert.AreEqual(ea.Command, DeploymentCommands.Execute);
        }

        [Test]
        public void Should_handle_dashes_and_slashes()
        {
            var arguments = "verify /environment:staging -deployment:MyStuff.dll /part:WEB";
            var ea = DeploymentCommandLineParser.Parse(arguments);

            Assert.AreEqual("staging", ea.Environment);
            Assert.AreEqual("MyStuff.dll", ea.Deployment);
            Assert.AreEqual(DeploymentCommands.Verify, ea.Command);
            Assert.AreEqual("WEB", ea.Role);
        }

        [Test]
        public void Should_parse_out_assembly()
        {
            var ea = DeploymentCommandLineParser.Parse(_arguments);

            Assert.AreEqual("MyStuff.dll", ea.Deployment);
        }

        [Test]
        public void Should_parse_out_Environment()
        {
            var ea = DeploymentCommandLineParser.Parse(_arguments);
            Assert.AreEqual("staging", ea.Environment);
        }


        [Test]
        public void Should_parse_out_parts()
        {
            var ea = DeploymentCommandLineParser.Parse(_arguments);

            Assert.AreEqual(ea.Role, "WEB");
        }

        [Test]
        public void Should_parse_out_verify()
        {
            var arguments = "verify";
            var ea = DeploymentCommandLineParser.Parse(arguments);

            Assert.AreEqual(ea.Command, DeploymentCommands.Verify);
        }
    }
}