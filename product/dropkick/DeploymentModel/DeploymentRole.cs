namespace dropkick.DeploymentModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class DeploymentRole
    {
        readonly IList<DeploymentServer> _servers;

        public DeploymentRole(string name)
        {
            Name = name;
            _servers = new List<DeploymentServer>();
        }

        public string Name { get; private set; }

        public int ServerCount
        {
            get
            {
                return _servers.Count;
            }
        }

        public DeploymentServer GetServer(string name)
        {
            return _servers.Where(s => s.Name == name).First();
        }

        public void AddServer(DeploymentServer server)
        {
            _servers.Add(server);
        }


        public void ForEachServerMapped(Action<DeploymentServer> detailAction)
        {
            foreach (var server in _servers)
            {
                detailAction(server);
            }
        }

        public void AddServer(string name)
        {
            AddServer(new DeploymentServer(name));
        }
    }
}