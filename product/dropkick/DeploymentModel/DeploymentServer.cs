namespace dropkick.DeploymentModel
{
    using System;
    using System.Collections.Generic;
    using Configuration.Dsl;

    public class DeploymentServer :
        TaskSite
    {
        //because tasks need to be customized per server
        readonly IList<DeploymentDetail> _details;

        public DeploymentServer(string name)
        {
            Name = name;
            _details = new List<DeploymentDetail>();
        }

        public string Name { get; private set; }


        public void AddDetail(DeploymentDetail detail)
        {
            _details.Add(detail);
        }


        public void ForEachDetail(Action<DeploymentDetail> detailAction)
        {
            foreach (var detail in _details)
            {
                detailAction(detail);
            }
        }

        public bool IsLocal
        {
            get
            {
                return System.Environment.MachineName == Name;
            }
        }

        public int DetailCount
        {
            get { return _details.Count; }
        }


        public void AddTask(Task task)
        {
            _details.Add(task.ToDetail(this));
        }

        public void AddTask(ProtoTask task)
        {
            task.RegisterTasks()(this);
        }
    }
}