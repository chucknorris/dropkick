namespace dropkick.Configuration.Dsl.MsSql
{
    using DeploymentModel;
    using Tasks;
    using Tasks.MsSql;

    public class ProtoRunSqlScriptTask :
        BaseTask
    {
        string _databaseName;

        public ProtoRunSqlScriptTask(string databaseName)
        {
            _databaseName = databaseName;
        }

        public string ScriptToRun { get; set; }

        public override Task ConstructTasksForServer(DeploymentServer server)
        {
            return new RunSqlScriptTask(server.Name, _databaseName)
                   {
                       ScriptToRun = this.ScriptToRun
                   };
        }
    }
}