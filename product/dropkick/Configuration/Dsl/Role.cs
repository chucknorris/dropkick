namespace dropkick.Configuration.Dsl
{
    using System;
    using DeploymentModel;

    public interface Role
    {
        string Name { get; }
        void ConfigureServer(DeploymentServer server);
    }

    public class Role<T,C> :
        Role,
        DeploymentInspectorSite
        where T : Deployment<T,C>, new()
    {
        Action<Server> _serverConfiguration;
        Server _server = new PrototypicalServer();

        public Role(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

        public void InspectWith(DeploymentInspector inspector)
        {
            inspector.Inspect(this, () => _server.InspectWith(inspector));
        }

        public static Role<T,C> GetRole(Role input)
        {
            Role<T,C> result = input as Role<T,C>;
            if(result == null)
                throw new ArgumentException(string.Format("The part is not valid for this deployment"), "input");

            return result;
        }

        public void BindAction(Action<Server> action)
        {
            action(_server);
        }

        public void ConfigureServer(DeploymentServer server)
        {
            _server.MapTo(server);
        }
    }
}