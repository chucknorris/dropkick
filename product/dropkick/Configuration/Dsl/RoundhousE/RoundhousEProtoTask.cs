namespace dropkick.Configuration.Dsl.RoundhousE
{
    using System;
    using DeploymentModel;
    using Tasks;
    using Tasks.CommandLine;

    public class RoundhousEProtoTask :
        BaseTask,
        RoundhousEOptions
    {
        public string EnvironmentName { get; set; }
        public string InstanceName { get; set; }
        public string DatabaseName { get; set; }
        public string SqlFilesLocation { get; set; }
        public bool UseSimpleRestoreMode { get; set; }
        public string DatabaseType { get; set; }
        public RoundhousEProtoTask()
        {
            SqlFilesLocation = ".\\database";
            InstanceName = ".";
        }

        public RoundhousEOptions Environment(string name)
        {
            EnvironmentName = name;
            return this;
        }

        public RoundhousEOptions OnInstance(string name)
        {
            InstanceName = name;
            return this;
        }

        public RoundhousEOptions OnDatabase(string name)
        {
            DatabaseName = name;
            return this;
        }

        public RoundhousEOptions UseMsSqlServer2005()
        {
            DatabaseType = "2005";
            return this;
        }

        public RoundhousEOptions WithRecoveryMode(string type)
        {
            UseSimpleRestoreMode = true;
            return this;
        }

        public override void RegisterRealTasks(PhysicalServer site)
        {
            var rhtask = new LocalCommandLineTask(".\\roundhouse\\rh.exe");
            
            rhtask.Args = "/d={0} /f={1} /s={2} /env={3} /dt={4}".FormatWith(DatabaseName, SqlFilesLocation, site.Name, EnvironmentName, DatabaseType);
            if (UseSimpleRestoreMode)
                rhtask.Args += " /simple";

            site.AddTask(rhtask);
        }
    }
}