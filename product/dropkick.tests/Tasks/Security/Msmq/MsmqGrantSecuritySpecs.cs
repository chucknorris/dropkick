using System;
using System.Messaging;
using System.Security.Principal;
using dropkick.Configuration.Dsl.Msmq;
using dropkick.DeploymentModel;
using dropkick.Tasks.Security.Msmq;
using NUnit.Framework;

namespace dropkick.tests.Tasks.Security.Msmq
{
    public class MsmqGrantSecuritySpecs
    {
        public abstract class MsmqGrantSecuritySpecsBase : TinySpec
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

            public MessageQueue RemoveLocalQueueIfExistsAndCreate(string queueName)
            {
                 if (MessageQueue.Exists(queueName))
                {
                    MessageQueue.Delete(queueName);
                }

               return MessageQueue.Create(queueName);
            }

        }

        [ConcernFor("MSMQ Tasks")]
        [Category("Integration")]
        public class when_granting_read_to_a_local_queue_for_a_user : MsmqGrantSecuritySpecsBase
        {
            protected LocalMsmqGrantReadTask task;

            public override void Context()
            {
                base.Context();
                RemoveLocalQueueIfExistsAndCreate(address.LocalName);
                
                task = new LocalMsmqGrantReadTask(address, user);
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
        public class when_granting_write_to_a_queue_for_a_user : MsmqGrantSecuritySpecsBase
        {
            protected MsmqGrantWriteTask task;

            public override void Context()
            {
                base.Context();
                RemoveLocalQueueIfExistsAndCreate(address.LocalName);

                task = new MsmqGrantWriteTask(address, user);
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
        public class when_granting_read_write_to_a_local_queue_for_a_user : MsmqGrantSecuritySpecsBase
        {
            protected LocalMsmqGrantReadWriteTask task;

            public override void Context()
            {
                base.Context();
                RemoveLocalQueueIfExistsAndCreate(address.LocalName);

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
        public class when_granting_read_write_to_a_local_queue_for_a_group : MsmqGrantSecuritySpecsBase
        {
            protected LocalMsmqGrantReadWriteTask task;

            public override void Context()
            {
                base.Context();
                RemoveLocalQueueIfExistsAndCreate(address.LocalName);

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
        [Category("Integration"),Explicit]
        public class when_granting_read_write_permissions_to_a_remote_queue : MsmqGrantSecuritySpecsBase
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