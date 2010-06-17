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
        private bool _createIfNoExist;
        private string _serverName;
        public bool PrivateQueue;

        public string QueueName { get; set; }

        public string QueuePath
        {
            get { return @"{0}\{1}{2}".FormatWith(ServerName, (PrivateQueue ? @"Private$\" : string.Empty), QueueName); }
        }

        public string ServerName
        {
            get { return _serverName; }
            set { _serverName = value.Equals(".") ? Environment.MachineName : value; }
        }

        public string Name
        {
            get { return string.Format("MsmqTask for server '{0}' and private queue named '{1}'", ServerName, QueueName); }
        }

        public DeploymentResult VerifyCanRun()
        {
            var result = new DeploymentResult();

            VerifyInAdministratorRole(result);

            if (Environment.MachineName.EqualsIgnoreCase(ServerName))
            {
                //if (MessageQueue.Exists(QueuePath))
                //{
                //    result.AddGood("'{0}' does exist");
                //}
                //else
                //{
                //    result.AddAlert(string.Format("'{0}' doesn't exist and will be created.", QueuePath));
                //}
                result.AddAlert("I can't check queue existance yet");
            }
            else
            {
                result.AddAlert(string.Format("Cannot check for queue '{0}' on server '{1}' while on server '{2}'",
                                              QueueName, ServerName, Environment.MachineName));
            }


            return result;
        }

        public DeploymentResult Execute()
        {
            var result = new DeploymentResult();

            if (!ServerName.EqualsIgnoreCase(Environment.MachineName))
                result.AddError("Cannot create a private queue on the remote machine '{0}' while on '{1}'.".FormatWith(ServerName, Environment.MachineName));
            else
            {
                if (!MessageQueue.Exists(QueuePath))
                {
                    result.AddAlert("'{0}' does not exist and will be created.".FormatWith(QueuePath));
                    MessageQueue.Create(QueuePath);
                    result.AddGood("Created queue '{0}'".FormatWith(QueuePath));
                }
                else
                    result.AddGood("'{0}' already exists.".FormatWith(QueuePath));
            }

            return result;
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
            _createIfNoExist = true;
        }
    }
}