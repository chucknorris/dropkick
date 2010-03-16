namespace dropkick.Tasks.Msmq
{
    using System;
    using System.Messaging;
    using System.Threading;
    using Configuration.Dsl;
    using DeploymentModel;
    using Dsl.Msmq;


    public class MsmqTask :
        Task
    {
        private bool _createIfNoExst;
        private string _queueName;
        private string _serverName;

        public string QueueName
        {
            get { return _queueName; }
            set { _queueName = value; }
        }

        public string QueuePath
        {
            get { return new QueueAddress(string.Format("msmq://{0}/{1}", ServerName, QueueName)).FormatName; }
        }

        public string ServerName
        {
            get { return _serverName; }
            set { _serverName = value.Equals(".") ? Environment.MachineName : value; }
        }



        public string Name
        {
            get { return string.Format("MsmqTask for server '{0}' and private queue named '{1}'", _serverName, _queueName); }
        }

        public DeploymentResult VerifyCanRun()
        {
            var result = new DeploymentResult();

            VerifyInAdministratorRole(result);

            if (Environment.MachineName.Equals(_serverName))
            {
                //if(MessageQueue.Exists(path))
                //{
                //    result.AddGood("'{0}' does exist");
                //}
                //else
                //{
                //    result.AddAlert(string.Format("'{0}' doesn't exist and will be created", _queueName));
                //}
                result.AddAlert("I can't check queue exstance yet");
            }
            else
            {
                result.AddAlert(string.Format("Cannot check for queue '{0}' on server '{1}' while on server '{2}'",
                                              _queueName, _serverName, Environment.MachineName));
            }


            return result;
        }

        public DeploymentResult Execute()
        {
            if (_serverName == Environment.MachineName)
            {
                MessageQueue.Create(QueuePath);
            }

            return new DeploymentResult();
        }


        private void VerifyInAdministratorRole(DeploymentResult result)
        {
            if (Thread.CurrentPrincipal.IsInRole("Administrator"))
            {
                result.AddAlert("You are not in the Administrator role");
            }
            else
            {
                result.AddGood("You are in the Administrator role");
            }
        }

        public void CreateIfItDoesNotExist()
        {
            _createIfNoExst = true;
        }
    }
}