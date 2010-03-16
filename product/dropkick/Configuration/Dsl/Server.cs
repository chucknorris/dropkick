namespace dropkick.Configuration.Dsl
{
    using System;
    using System.Collections.Generic;
    using DeploymentModel;
    using Tasks.Msmq;

    public interface Server :
        DeploymentInspectorSite
    {
        void MapTo(DeploymentServer server);
        void RegisterTask(ProtoTask protoTask);
    }

    public class PrototypicalServer :
        Server
    {
        IList<ProtoTask> _tasks = new List<ProtoTask>();

        public void InspectWith(DeploymentInspector inspector)
        {
            inspector.Inspect(this, ()=>
            {
                foreach (ProtoTask task in _tasks)
                {
                    task.InspectWith(inspector);
                }
            });
        }

        //TODO: YUCK
        public void MapTo(DeploymentServer server)
        {
            foreach (var task in _tasks)
            {
                server.AddDetail(task.ConstructTasksForServer(server).ToDetail(server));
            }
        }

        public void RegisterTask(ProtoTask protoTask)
        {
            _tasks.Add(protoTask);
        }
    }
}