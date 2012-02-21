namespace dropkick.DeploymentModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using log4net;

    public class DeploymentPlan
    {
        readonly IList<DeploymentRole> _roles = new List<DeploymentRole>();

        public string Name { get; set; }

        public int RoleCount
        {
            get { return _roles.Count; }
        }

        public DeploymentRole AddRole(string name)
        {
            var role = new DeploymentRole(name);
            _roles.Add(role);
            return role;
        }
        public void AddRole(DeploymentRole role)
        {
            _roles.Add(role);
        }

        public DeploymentResult Trace()
        {
            var deploymentResult = new DeploymentResult();
            Ex(d =>
            {
                var result = d.Trace();
                DisplayResults(result);
                deploymentResult.MergedWith(result);
            });

            return deploymentResult;
        }

        public DeploymentResult Verify()
        {
            var deploymentResult =new DeploymentResult();
            Ex(d =>
            {
                var result = d.Verify();
                DisplayResults(result);
                deploymentResult.MergedWith(result);
            });

            return deploymentResult;
        }

        public DeploymentResult Execute()
        {
            var deploymentResult = new DeploymentResult();

            Ex(d =>
            {
                var o = d.Verify();
                deploymentResult.MergedWith(o);
                if (o.ContainsError())
                {
                    //display errors!
                    DisplayResults(o);
                    //stop. report verify error.
                    return;
                }

                var result = d.Execute();
                DisplayResults(result);
                deploymentResult.MergedWith(result);
            });

            return deploymentResult;
        }

        DeploymentResult Ex(Action<DeploymentDetail> action)
        {
            var result = new DeploymentResult();
            result.AddNote(Name);

            foreach (var role in _roles)
            {
                result.AddNote(role.Name);

                role.ForEachServerMapped(s =>
                {
                    result.AddNote(s.Name);
                    s.ForEachDetail(d =>
                    {
                        result.AddNote(d.Name);
                        action(d);
                    });
                });
            }

            return result;
        }

        public DeploymentRole GetRole(string name)
        {
            return _roles.Where(r => r.Name == name).First();
        }

        //todo: keep this logger in mind
        static void DisplayResults(DeploymentResult results)
        {
            foreach (var result in results)
            {
                if (result.Status == DeploymentItemStatus.Error) Logging.Coarse(LogLevel.Error,"[{0,-5}] {1}", result.Status, result.Message);
                if (result.Status == DeploymentItemStatus.Alert) Logging.Coarse(LogLevel.Warn, "[{0,-5}] {1}", result.Status, result.Message);
                if (result.Status == DeploymentItemStatus.Good) Logging.Coarse(LogLevel.Info, "[{0,-5}] {1}", result.Status, result.Message);
                if (result.Status == DeploymentItemStatus.Note) Logging.Fine(LogLevel.Info, "[{0,-5}] {1}", result.Status, result.Message);
                if (result.Status == DeploymentItemStatus.Verbose) Logging.Fine(LogLevel.Debug, "[{0,-5}] {1}", result.Status, result.Message);
            }
        }

    }
}