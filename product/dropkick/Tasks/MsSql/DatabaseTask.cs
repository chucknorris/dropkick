using System;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.Management.Smo;
using dropkick.DeploymentModel;

namespace dropkick.Tasks.MsSql
{
    public class DatabaseTask : Task
    {
        public string Name
        {            
            get
            {
                var sb = new StringBuilder();
                sb.AppendFormat("On the server: {0}, database: {1}", DbServer, DbName);
                if (CreateIfDoesntExists) sb.Append(", create if doesn't exists");
                if (CreateBackup) sb.AppendFormat(", backup to {0}", BackupPath);
                if (RunScriptFile) sb.AppendFormat(", run script {0}", ScriptFile);
                return sb.ToString();
            }
        }

        public DeploymentResult VerifyCanRun()
        {
            var result = new DeploymentResult();
            if (String.IsNullOrWhiteSpace(DbServer))
            {
                result.AddAlert("DbServer is not specified.");
                return result;
            }
            if (String.IsNullOrWhiteSpace(DbName))
            {
                result.AddAlert("DbServer is not specified.");
                return result;
            }
            try
            {
                var server = new Server(DbServer);
                bool dbExists = GetDb(server) != null;
                if (!CreateIfDoesntExists && !dbExists)
                    result.AddAlert(String.Format("DB {0} doesn't exists.", DbName));
                if (CreateIfDoesntExists && !dbExists)
                    result.AddGood(String.Format("DB {0} doesn't exists. It will be created.", DbName));
                if (RunScriptFile && !File.Exists(ScriptFile))
                    result.AddAlert(String.Format("Script file {0} doesn't exists", ScriptFile));

            }
            catch(Exception e)
            {
                result.AddError("Can not run task", e);
            }
            return result;
        }

        private Database GetDb(Server server)
        {
            return server.Databases.Cast<Database>().FirstOrDefault(i => String.Compare(i.Name, DbName) == 0);
        }

        public DeploymentResult Execute()
        {
            var result = new DeploymentResult();
            var server = new Server(DbServer);
            Database db = GetDb(server);
            if (db == null && CreateIfDoesntExists)
            {
                result.AddGood("New DB {0} created.", DbName);
                db = CreateNewDb(server);
            }

            if (db == null)
            {
                throw new Exception(String.Format("DB {0} doesn't exists on server {1}", DbName, DbServer));
            }

            if (RunScriptFile)
            {
                string cmd = File.ReadAllText(ScriptFile);
                db.ExecuteNonQuery(cmd);
                result.AddGood("Script file {0} executed", ScriptFile);
            }

            //TODO: backup

            return result;
        }

        private Database CreateNewDb(Server server)
        {
            var db = new Database(server, DbName);
            server.Databases.Add(db);
            db.Create();
            return db;
        }

        public string DbName { get; set; }
        public string DbServer { get; set; }
        public bool CreateIfDoesntExists { get; set; }
        public bool CreateBackup { get; set; }
        public string BackupPath { get; set; }
        public string ScriptFile { get; set; }
        public bool RunScriptFile { get; set; }
    }
}