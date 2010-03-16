namespace dropkick.DeploymentModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

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

        public DeploymentResult Execute()
        {
            return Ex(d=>
            {
                var o = d.Verify();
                if(o.ContainsError())
                {
                    //stop. report verify error.
                    return o;
                }
                var oo = d.Execute();

                return o.MergedWith(oo);
            });
        }
        public DeploymentResult Verify()
        {
            return Ex(d=>d.Verify());
        }
        public DeploymentResult Trace()
        {
            return Ex(d=> new DeploymentResult());
        }

        DeploymentResult Ex(Func<DeploymentDetail, DeploymentResult> action)
        {
            Console.WriteLine(Name);
            var result = new DeploymentResult();

            foreach (var role in _roles)
            {
                Console.WriteLine("  {0}", role.Name);

                role.ForEachServer(s =>
                {
                    Console.WriteLine("    {0}", s.Name);
                    s.ForEachDetail(d =>
                    {
                        Console.WriteLine("      {0}", d.Name);
                        var r = action(d);
                        result.MergedWith(r);
                        foreach (var item in r.Results)
                        {
                            Console.WriteLine("      [{0}] {1}", item.Status, item.Message);
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
    }
}