using System;
using System.Messaging;
using System.Security.Principal;
using dropkick.Configuration.Dsl.Msmq;
using dropkick.DeploymentModel;
using dropkick.Tasks.Security.Msmq;
using NUnit.Framework;

namespace dropkick.tests.Tasks.Security.Msmq
{
    public class MsmqGrantReadWriteSpecs
    {
        public abstract class MsmqGrantReadWriteSpecsBase : TinySpec
        {
            protected PhysicalServer server;
            protected string user;
            protected QueueAddress address;
            protected DeploymentResult result;


            public override void Context()
            {
                server = new DeploymentServer("localhost");
                user = WindowsIdentity.GetCurrent().Name;

                var ub = new UriBuilder("msmq", Environment.MachineName)
                {
                    Path = "dk_test"
                };
                address = new QueueAddress(ub.Uri);
            }

        }

        [ConcernFor("MSMQ Tasks")]
        [Category("Integration")]
        public class when_granting_read_write_to_a_local_queue_for_a_user : MsmqGrantReadWriteSpecsBase
        {
            protected LocalMsmqGrantReadWriteTask task;

            public override void Context()
            {
                base.Context();

                if (MessageQueue.Exists(address.LocalName))
                {
                    MessageQueue.Delete(address.LocalName);
                }

                MessageQueue.Create(address.LocalName);

                task = new LocalMsmqGrantReadWriteTask(address, user);
            }

            public override void Because()
            {
                result = task.Execute();
            }


            [Fact]
            public void should_complete_successfully()
            {
                //no code issues
            }

            [Fact]
            public void should_return_a_result()
            {
                Assert.IsNotNull(result);
            }

            [Fact]
            public void should_not_contain_any_errors()
            {
                Assert.IsFalse(result.ContainsError(), "Errors occured during permission setting.{0}{1}".FormatWith(Environment.NewLine, result.ToString()));
            }
        }

        [ConcernFor("MSMQ Tasks")]
        [Category("Integration")]
        public class when_granting_read_write_to_a_local_queue_for_a_group : MsmqGrantReadWriteSpecsBase
        {
            protected LocalMsmqGrantReadWriteTask task;

            public override void Context()
            {
                base.Context();

                if (MessageQueue.Exists(address.LocalName))
                {
                    MessageQueue.Delete(address.LocalName);
                }

                MessageQueue.Create(address.LocalName);

                task = new LocalMsmqGrantReadWriteTask(address, @"Everyone");
            }

            public override void Because()
            {
                result = task.Execute();
            }


            [Fact]
            public void should_complete_successfully()
            {
                //no code issues
            }

            [Fact]
            public void should_return_a_result()
            {
                Assert.IsNotNull(result);
            }

            [Fact]
            public void should_not_contain_any_errors()
            {
                Assert.IsFalse(result.ContainsError(), "Errors occured during permission setting.{0}{1}".FormatWith(Environment.NewLine, result.ToString()));
            }
        }

        [ConcernFor("MSMQ Tasks")]
        [Category("Integration")]
        public class when_granting_read_write_permissions_to_a_remote_queue : MsmqGrantReadWriteSpecsBase
        {
            protected RemoteMsmqGrantReadWriteTask task;

            public override void Context()
            {
                base.Context();

                server = new DeploymentServer("127.0.0.1");

                var ub = new UriBuilder("msmq", "127.0.0.1")
                {
                    Path = "dk_test_remote"
                };
                address = new QueueAddress(ub.Uri);

                task = new RemoteMsmqGrantReadWriteTask(server, address, user);
            }

            public override void Because()
            {
                result = task.Execute();
            }

            [Fact, Explicit]
            public void should_complete_successfully()
            {
                //no code issues
            }

            [Fact, Explicit]
            public void should_return_a_result()
            {
                Assert.IsNotNull(result);
            }

            //NOTE:Remote MSMQ Granting read/write doesn't appear to work
            [Fact, Explicit]
            public void should_not_contain_any_errors()
            {
                Assert.IsFalse(result.ContainsError(), "Errors occured during permission setting.{0}{1}".FormatWith(Environment.NewLine, result.ToString()));
            }
        }
    }
}