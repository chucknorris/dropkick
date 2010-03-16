namespace dropkick.Configuration.Dsl
{
    using System;
    using DeploymentModel;

    public interface Role
    {
        string Name { get; }
        void ConfigureServer(DeploymentServer server);
    }

    public class ServerRole :
        Role,
        DeploymentInspectorSite
    {
        Action<Server> _serverConfiguration;
        Server _server = new PrototypicalServer();

        public ServerRole(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

        public void InspectWith(DeploymentInspector inspector)
        {
            inspector.Inspect(this, () => _server.InspectWith(inspector));
        }

        public static ServerRole GetRole(Role input)
        {
            ServerRole result = input as ServerRole;
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