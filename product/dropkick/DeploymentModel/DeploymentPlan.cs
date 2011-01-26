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

        public void Execute()
        {
            Ex(d =>
            {
                var o = d.Verify();
                if (o.ContainsError())
                {
                    //stop. report verify error.
                    return;
                }

                var oo = d.Execute();

                DisplayResults(oo);
            });
        }
        public void Verify()
        {
            Ex(d => d.Verify());
        }
        public void Trace()
        {
            Ex(d =>
            {
                var o = d.Trace();
                DisplayResults(o);
            });
        }

        void Ex(Action<DeploymentDetail> action)
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
                    _log.InfoFormat("[{0,-5}] {1}", result.Status, result.Message);
            }
        }

    }
}