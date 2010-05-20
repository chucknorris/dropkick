namespace dropkick.Configuration.Dsl.Security
{
    using DeploymentModel;
    using Tasks;
    using Tasks.Security;

    public class ProtoLogOnAsAServiceTask :
        BaseTask
    {
        readonly string _userAccount;

        public ProtoLogOnAsAServiceTask(string userAccount)
        {
            _userAccount = userAccount;
        }

        public override void RegisterRealTasks(PhysicalServer site)
        {
            site.AddTask(new LogOnAsAServiceTask(site.Name, _userAccount));
        }
    }
}