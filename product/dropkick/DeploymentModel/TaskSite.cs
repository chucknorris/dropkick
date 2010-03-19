namespace dropkick.DeploymentModel
{
    public interface TaskSite
    {
        void AddTask(Task task);
        string Name { get; }
        bool IsLocal { get; }
    }
}