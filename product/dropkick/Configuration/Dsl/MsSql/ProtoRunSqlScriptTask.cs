namespace dropkick.Configuration.Dsl.MsSql
{
    using System;
    using DeploymentModel;
    using Tasks;
    using Tasks.MsSql;

    public class ProtoRunSqlScriptTask :
        BaseTask
    {
        readonly string _databaseName;

        public ProtoRunSqlScriptTask(string databaseName)
        {
            _databaseName = databaseName;
        }

        public string ScriptToRun { get; set; }

        public override Action<TaskSite> RegisterTasks()
        {
            return s =>
                   {
                       s.AddTask(new RunSqlScriptTask(s.Name, _databaseName)
                                     {
                                         ScriptToRun = this.ScriptToRun
                                     });
                   };
        }
    }
}