namespace dropkick.Tasks.WinService
{
    using DeploymentModel;
    using Prompting;
    using Wmi;
    using Magnum.ObjectExtensions;

    public class WinServiceCreateTask :
        BaseServiceTask
    {
        //TODO: should be injected
        PromptService _prompt = new ConsolePromptService();

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

            if(UserName.IsNullOrEmpty())
                result.AddAlert("We are going to prompt for a username.");

            if(Password.IsNullOrEmpty())
                result.AddAlert("We are going to prompt for a password.");

            return result;
        }

        public override DeploymentResult Execute()
        {
            var result = new DeploymentResult();

            if (UserName.IsNullOrEmpty())
                UserName = _prompt.Prompt("Win Service '{0}' UserName".FormatWith(ServiceName));

            if (Password.IsNullOrEmpty())
                UserName = _prompt.Prompt("Win Service '{0}' Password".FormatWith(ServiceName));

            var returnCode = WmiService.Create(MachineName, ServiceName, ServiceName, ServiceLocation, StartMode, UserName, Password, Dependencies);

            return result;
        }

        public string[] Dependencies { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        public ServiceStartMode StartMode { get; set; }
        public string ServiceLocation { get; set; }
    }
}