namespace dropkick.Tasks.WinService
{
    using DeploymentModel;
    using Wmi;

    public class WinServiceCreateTask :
        BaseServiceTask
    {
        public WinServiceCreateTask(string machineName, string serviceName)
            : base(machineName, serviceName)
        {
        }

        public override string Name
        {
            get { return "Installing service '{0}' on '{1}'".FormatWith(ServiceName, MachineName); }
        }

        public override DeploymentResult VerifyCanRun()
        {
            var result = new DeploymentResult();

            return result;
        }

        public override DeploymentResult Execute()
        {
            var result = new DeploymentResult();

            var returnCode = WmiService.Create(MachineName, ServiceName, ServiceDescription, ServiceLocation, StartMode, UserName, Password, Dependencies);

            return result;
        }

        public string[] Dependencies { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        public ServiceStartMode StartMode { get; set; }
        public string ServiceLocation { get; set; }
        public string ServiceDescription { get; set; }
    }
}