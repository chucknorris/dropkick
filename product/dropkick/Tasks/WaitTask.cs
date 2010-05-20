namespace dropkick.Tasks
{
    using System;
    using System.Threading;
    using DeploymentModel;

    public class WaitTask :
        Task
    {
        readonly TimeSpan _waitTime;

        public WaitTask(TimeSpan waitTime)
        {
            _waitTime = waitTime;
        }

        public string Name
        {
            get { return "Wait: {0} seconds".FormatWith(_waitTime.TotalSeconds); }
        }

        public DeploymentResult VerifyCanRun()
        {
            return new DeploymentResult();
        }

        public DeploymentResult Execute()
        {
            Thread.Sleep(_waitTime);
            var result = new DeploymentResult();
            result.AddGood("Waited for '{0}' seconds", _waitTime.TotalSeconds.ToString());
            return result;
        }
    }
}