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
    using dropkick.Configuration;
    using dropkick.Configuration.Dsl.Files;
    using dropkick.DeploymentModel;
    using FileSystem;
    using NUnit.Framework;

    [TestFixture]
    public class CopyFilesProtoWithSettings
    {
        ProtoCopyFileTask _proto;
        DeploymentServer _serv;

        [SetUp]
        public void Bob()
        {
            var settings = new TestSettings();
            settings.Bob = "dru";
            settings.Environment = "PROD";
            HUB.Settings = settings;
            _proto = new ProtoCopyFileTask(new DotNetPath(), "C:\\from\\here");
            _proto.ToDirectory("C:\\to\\there\\{{Environment}}");
            _serv = new DeploymentServer("bob");

            _proto.RegisterRealTasks(_serv);
        }

        [Test]
        public void Bill()
        {
            //need a real test
        }

        public class TestSettings :
            DropkickConfiguration
        {
            public string Bob { get; set; }
        }
    }
}