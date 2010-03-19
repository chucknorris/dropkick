namespace dropkick.Tasks.Files
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DeploymentModel;

    public class MultiCopyTask :
        Task
    {
        private IList<CopyTask> _tasks = new List<CopyTask>();

        public string Name
        {
            get { return "MultiCopy"; }
        }

        public DeploymentResult VerifyCanRun()
        {
            return _tasks.Select(x => x.VerifyCanRun()).Aggregate((l, r) => l.MergedWith(r));
        }

        public DeploymentResult Execute()
        {
            return _tasks.Select(x => x.Execute()).Aggregate((l, r) => l.MergedWith(r));
        }

        public void Add(CopyTask task)
        {
            _tasks.Add(task);
        }
    }
}