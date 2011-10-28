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

using System.Linq;

namespace dropkick.tests.Tasks.Msmq
{
    using System;
    using System.Messaging;
    using dropkick.Configuration.Dsl.Msmq;
    using dropkick.DeploymentModel;
    using dropkick.Tasks.Msmq;
    using NUnit.Framework;

    [TestFixture]
    [Category("Integration")]
    public class MsmqTest
    {
        [Test][Explicit]
        public void ExecuteLocal()
        {
            var ps = new DeploymentServer(Environment.MachineName);
            var ub = new UriBuilder("msmq", ps.Name) { Path = "dk_test2" };
            var address = new QueueAddress(ub.Uri);

            if (MessageQueue.Exists(address.LocalName))
                MessageQueue.Delete(address.LocalName);

            var t = new CreateLocalMsmqQueueTask(ps, address);
            var r = t.Execute();

            Assert.IsFalse(r.ContainsError(), "Errors occured during MSMQ create execution.");

        }

        [Test][Explicit]
        public void VerifyLocal()
        {
            var ps = new DeploymentServer(Environment.MachineName);
            var ub = new UriBuilder("msmq", ps.Name) { Path = "dk_test" };
            var t = new CreateLocalMsmqQueueTask(ps, new QueueAddress(ub.Uri));
            var r = t.VerifyCanRun();

            Assert.IsFalse(r.ContainsError(), "Errors occured during MSMQ create verification.");
        }

    }

    [TestFixture]
    [Category("Integration")]
    public class TransactionalLocalMsmqTest
    {
        [Test]
        [Explicit]
        public void ExecuteLocal()
        {
            var ps = new DeploymentServer(Environment.MachineName);
            var ub = new UriBuilder("msmq", ps.Name) { Path = "dk_test2" };
            var address = new QueueAddress(ub.Uri);

            if (MessageQueue.Exists(address.LocalName))
                MessageQueue.Delete(address.LocalName);

            var t = new CreateLocalMsmqQueueTask(ps, address, transactional:true);
            var r = t.Execute();

            Assert.IsFalse(r.ContainsError(), "Errors occured during MSMQ create execution.");

            var queue =
                MessageQueue.GetPrivateQueuesByMachine(Environment.MachineName).SingleOrDefault(
                    x => address.LocalName.EndsWith(x.QueueName));
            Assert.IsNotNull(queue, "Transactional queue was not created.");
            Assert.IsTrue(queue.Transactional, "Queue was created but is not transactional.");
        }
    }

    [TestFixture]
    [Category("Integration")]
    public class RemoteCreateMsmqTest
    {
        [Test][Explicit]
        public void Execute()
        {
            var ps = new DeploymentServer("srvtestweb01");
            var ub = new UriBuilder("msmq", ps.Name) { Path = "dk_test" };
            var address = new QueueAddress(ub.Uri);

            //delete the remote queue

            var t = new CreateRemoteMsmqQueueTask(ps, address);
            var r = t.Execute();

            Assert.IsFalse(r.ContainsError(), "Errors occured during MSMQ create execution.");

        }

        [Test][Explicit]
        public void Verify()
        {
            var ps = new DeploymentServer("srvutilbuild");
            var ub = new UriBuilder("msmq", ps.Name) { Path = "dk_test" };
            var t = new CreateLocalMsmqQueueTask(ps, new QueueAddress(ub.Uri));
            var r = t.VerifyCanRun();

            Assert.IsFalse(r.ContainsError(), "Errors occured during MSMQ create verification.");
        }
    }
}