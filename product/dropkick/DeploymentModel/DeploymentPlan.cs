namespace dropkick.DeploymentModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using log4net;

    public class DeploymentPlan
    {
        readonly IList<DeploymentRole> _roles = new List<DeploymentRole>();
        static readonly ILog _log = LogManager.GetLogger(typeof(DeploymentPlan));

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

        public DeploymentResult Execute()
        {
            return Ex(d =>
            {
                var o = d.Verify();
                if (o.ContainsError())
                {
                    //stop. report verify error.
                    return o;
                }

                var oo = d.Execute();

                DisplayResults(oo);

                return oo;
            });
        }
        public DeploymentResult Verify()
        {
            return Ex(d => d.Verify());
        }
        public DeploymentResult Trace()
        {
            return Ex(d => new DeploymentResult());
        }

        DeploymentResult Ex(Func<DeploymentDetail, DeploymentResult> action)
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
                        var r = action(d);
                        result.MergedWith(r);
                        foreach (var item in r.Results)
                        {
                            result.Add(item);
                        }
                    });
                });
            }

            return result;
        }

        public DeploymentRole GetRole(string name)
        {
            return _roles.Where(r => r.Name == name).First();
        }

        static void DisplayResults(DeploymentResult results)
        {
            foreach (var result in results)
            {
                if (result.Status == DeploymentItemStatus.Error)
                    _log.ErrorFormat("[{0,-5}] {1}", result.Status, result.Message);

                if (result.Status == DeploymentItemStatus.Alert)
                    _log.WarnFormat("[{0,-5}] {1}", result.Status, result.Message);

                if (result.Status == DeploymentItemStatus.Good)
                    _log.InfoFormat("[{0,-5}] {1}", result.Status, result.Message);

                if (result.Status == DeploymentItemStatus.Note)
                    _log.DebugFormat("[{0,-5}] {1}", result.Status, result.Message);
            }
        }

    }
}