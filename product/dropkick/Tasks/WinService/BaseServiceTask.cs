namespace dropkick.Tasks.WinService
{
    using System.Threading;
    using Configuration.Dsl;
    using DeploymentModel;


    public abstract class BaseServiceTask :
        Task
    {
        protected BaseServiceTask(string machineName, string serviceName)
        {
            MachineName = machineName;
            ServiceName = serviceName;
        }

        public string MachineName { get; set; }
        public string ServiceName { get; set; }

        public abstract string Name { get; }

        public abstract DeploymentResult VerifyCanRun();
        public abstract DeploymentResult Execute();



        protected void VerifyInAdministratorRole(DeploymentResult result)
        {
            if (Thread.CurrentPrincipal.IsInRole("Administrator"))
            {
                result.AddAlert("You are not in the 'Administrator' role. You will not be able to start/stop services");
            }
            else
            {
                result.AddGood("You are in the 'Administrator' role");
            }
        }
    }
}