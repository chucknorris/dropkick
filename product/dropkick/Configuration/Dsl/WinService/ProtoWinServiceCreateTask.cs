namespace dropkick.Configuration.Dsl.WinService
{
    using System;
    using System.Collections.Generic;
    using DeploymentModel;
    using Tasks;
    using Tasks.WinService;
    using Wmi;

    public class ProtoWinServiceCreateTask :
        BaseTask,
        WinServiceCreateOptions
    {
        readonly string _serviceName;
        List<string> _dependencies = new List<string>();
        string _userName;
        string _password;
        string _description;
        string _installPath;
        ServiceStartMode _startMode;

        public ProtoWinServiceCreateTask(string privateName)
        {
            _serviceName = privateName;
        }

        public WinServiceCreateOptions WithDescription(string description)
        {
            _description = description;
            return this;
        }

        public WinServiceCreateOptions WithServicePath(string path)
        {
            _installPath = path;
            return this;
        }

        public WinServiceCreateOptions WithStartMode(ServiceStartMode mode)
        {
            _startMode = mode;
            return this;
        }

        public override void RegisterRealTasks(PhysicalServer site)
        {
            site.AddTask(new WinServiceCreateTask(site.Name, _serviceName)
                             {
                                 Dependencies = _dependencies.ToArray(),
                                 //UserName = _userName,
                                 //Password = _password,
                                 //ServiceDescription =  _description, no place to put this currently
                                 ServiceLocation =  _installPath,
                                 StartMode =  _startMode
                             });
        }
    }

    public interface WinServiceCreateOptions
    {
        WinServiceCreateOptions WithDescription(string description);
        WinServiceCreateOptions WithServicePath(string path);
        WinServiceCreateOptions WithStartMode(ServiceStartMode mode);
    }
}