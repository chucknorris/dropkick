namespace dropkick.Configuration.Dsl.WinService
{
    using DeploymentModel;
    using Tasks;
    using Tasks.WinService;

    public class ProtoWinServiceDeleteTask :
        BaseTask
    {
        readonly string _serviceName;

        public ProtoWinServiceDeleteTask(string serviceName)
        {
            _serviceName = serviceName;
        }

        public override void RegisterRealTasks(PhysicalServer site)
        {
            site.AddTask(new WinServiceDeleteTask(site.Name, _serviceName));
        }
    }
}