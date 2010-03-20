namespace dropkick.Tasks
{
    using System;
    using Configuration.Dsl;
    using DeploymentModel;

    public abstract class BaseTask :
        ProtoTask
    {
        public void InspectWith(DeploymentInspector inspector)
        {
            inspector.Inspect(this);
        }

        public abstract void RegisterTasks(TaskSite site);
        
    }
}