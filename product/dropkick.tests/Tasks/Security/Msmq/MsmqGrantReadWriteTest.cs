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
namespace dropkick.tests.Tasks.Security.Msmq
{
    using System;
    using System.Messaging;
    using dropkick.Configuration.Dsl.Msmq;
    using dropkick.DeploymentModel;
    using dropkick.Tasks.Security.Msmq;
    using NUnit.Framework;

    [TestFixture]
    [Category("Integration")]
    public class MsmqLocalGrantReadWriteTest
    {
        QueueAddress _address;

        [TestFixtureSetUp]
        public void Setup()
        {
            var ub = new UriBuilder("msmq", Environment.MachineName) {Path = "dk_test"};
            _address = new QueueAddress(ub.Uri);

            if (MessageQueue.Exists(_address.LocalName))
                MessageQueue.Delete(_address.LocalName);

            MessageQueue.Create(_address.LocalName);
        }

        [Test]
        public void Execute()
        {
            var t = new LocalMsmqGrantReadWriteTask(_address, @"Everyone");
            DeploymentResult r = t.Execute();

            Assert.IsFalse(r.ContainsError(), "Errors occured during permission setting.");
        }
    }

    [TestFixture]
    [Category("Integration")]
    public class MsmqRemoteGrantReadWriteTest
    {
        QueueAddress _address;

        [TestFixtureSetUp]
        public void Setup()
        {
            var ub = new UriBuilder("msmq", "SrvTestWeb01") {Path = "dk_test"};
            _address = new QueueAddress(ub.Uri);

//            if (MessageQueue.Exists(_address.LocalName))
//                MessageQueue.Delete(_address.LocalName);
//
//            MessageQueue.Create(_address.LocalName);
        }

        [Test]
        public void Execute()
        {
            var t = new LocalMsmqGrantReadWriteTask(_address, @"TEST\ReynoldsR");
            DeploymentResult r = t.Execute();

            Assert.IsFalse(r.ContainsError(), "Errors occured during permission setting.");
        }
    }
}