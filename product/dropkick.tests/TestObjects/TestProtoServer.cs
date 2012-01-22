using System;
using dropkick.Configuration.Dsl;
using dropkick.DeploymentModel;

namespace dropkick.tests.TestObjects
{
    public class TestProtoServer : ProtoServer
    {
        public ProtoTask ProtoTask { get; private set; }

        void ProtoServer.MapTo(DeploymentServer server)
        {
            throw new NotImplementedException();
        }

        void ProtoServer.RegisterProtoTask(ProtoTask protoTask)
        {
            ProtoTask = protoTask;
        }

        void DeploymentInspectorSite.InspectWith(DeploymentInspector inspector)
        {
            throw new NotImplementedException();
        }
    }
}