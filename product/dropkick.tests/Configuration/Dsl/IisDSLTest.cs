using Moq;
using NUnit.Framework;
using dropkick.Configuration.Dsl;
using dropkick.Configuration.Dsl.Iis;
using dropkick.DeploymentModel;
using dropkick.Tasks.Iis;

namespace dropkick.tests.Configuration.Dsl
{
    // This fixture was originally setup to test for default bindings being added to the task
    // but I decided it was better to make no assumptions about bindings if none were specified.
    // I left the fixture in here as an example of how to mock the prototask -> realtask process.
    [Category("Iis7ProtoTask")]
    public class When_creating_an_iis_site_without_specifying_bindings : TinySpec
    {
        [Fact]
        public void It_should_not_setup_any_binding()
        {
            Assert.IsNull(_iisTask.Bindings);
        }

        public override void Context()
        {
            var mockProtoServer = new Mock<ProtoServer>();
            mockProtoServer.Setup(x => x.RegisterProtoTask(It.IsAny<ProtoTask>()))
                .Callback<ProtoTask>(t => _protoTask = t);
            var mockPhysServer = new Mock<PhysicalServer>();
            mockPhysServer.Setup(x => x.AddTask(It.IsAny<Iis7Task>()))
                .Callback<Task>(t => _iisTask = (Iis7Task)t);
            mockPhysServer.Setup(x => x.Name).Returns("localhost");

            _protoServer = mockProtoServer.Object;
            _physicalServer = mockPhysServer.Object;
        }

        public override void Because()
        {
            _protoServer.Iis7Site("test");
            _protoTask.RegisterRealTasks(_physicalServer);
        }

        ProtoTask _protoTask;
        ProtoServer _protoServer;
        Iis7Task _iisTask;
        PhysicalServer _physicalServer;
    }
}
