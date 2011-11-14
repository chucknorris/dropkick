// Copyright 2007-2010 The Apache Software Foundation.
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.using System;

using System;

namespace dropkick.Tasks.MsSql.Smo
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Microsoft.SqlServer.Management.Smo;
    using DeploymentModel;

    public class DatabaseTask : Task
    {
        public DatabaseTask()
        {
            ScriptFiles = new List<string>();
            CreateScriptFiles = new List<string>();
        }

        public string Name
        {            
            get
            {
                var sb = new StringBuilder();
                sb.AppendFormat("On the server: {0}, database: {1}", DbServer, DbName);
                if (OpeningOptions == OpeningOptions.FailIfDoesntExists) sb.Append(", fail if doesn't exists");
                if (OpeningOptions == OpeningOptions.CreateIfDoesntExists) sb.Append(", create if doesn't exists");
                if (OpeningOptions == OpeningOptions.RecreateIfExists) sb.Append(", recreate if exists");
                foreach (var file in ScriptFiles)
                {
                    sb.AppendFormat(", run script {0}", Path.GetFileName(file));
                }
                return sb.ToString();
            }
        }

        private void CheckScriptFiles(IEnumerable<string> files, DeploymentResult result, DeploymentItemStatus errorStatus)
        {
            foreach (var file in files)
            {
                if (!File.Exists(file))
                {
                    result.Add(new DeploymentItem(errorStatus, String.Format("Script file '{0}' not found.", file)));
                }
                else
                {
                    result.AddGood("Script file '{0}' found.");
                }
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

                bool crateIfDoesntExists = (OpeningOptions & OpeningOptions.CreateIfDoesntExists) == OpeningOptions.CreateIfDoesntExists;
                bool recreateIfExists = (OpeningOptions & OpeningOptions.RecreateIfExists) == OpeningOptions.RecreateIfExists;

                bool dbExists = GetDb(server) != null;
                if (dbExists == false)
                {
                    if (crateIfDoesntExists)
                    {
                        result.AddGood(String.Format("DB {0} doesn't exists. It will be created.", DbName));
                        CheckScriptFiles(CreateScriptFiles, result, DeploymentItemStatus.Alert);
                    }
                    else
                        result.AddAlert(String.Format("DB {0} doesn't exists.", DbName));
                }
                else
                {
                    if (recreateIfExists)
                        result.AddGood(String.Format("DB {0} exists. It will be recreated.", DbName));
                    else
                        result.AddGood(String.Format("DB {0} exists.", DbName));
                }

                CheckScriptFiles(ScriptFiles, result, DeploymentItemStatus.Alert);                
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
            
            bool crateIfDoesntExists = (OpeningOptions & OpeningOptions.CreateIfDoesntExists) == OpeningOptions.CreateIfDoesntExists;
            bool recreateIfExists = (OpeningOptions & OpeningOptions.RecreateIfExists) == OpeningOptions.RecreateIfExists;

            if (db == null && crateIfDoesntExists)
            {
                result.AddGood("New DB {0} created.", DbName);
                db = CreateNewDb(server);
            }

            if (db == null)
            {
                throw new Exception(String.Format("DB {0} doesn't exists on server {1}", DbName, DbServer));
            }

            if (recreateIfExists)
            {
                db.Drop();
                db = CreateNewDb(server);
            }            

            ExecuteScriptFiles(ScriptFiles, db);

            if (DropDb)
            {
                db.Drop();
            }
            return result;
        }

        private void ExecuteScriptFiles(IEnumerable<string> files, Database db)
        {
            foreach (var file in files)
            {
                string cmd = File.ReadAllText(file);
                db.ExecuteNonQuery(cmd);
            }
        }

        private Database CreateNewDb(Server server)
        {
            var db = new Database(server, DbName);

            // apply options
            if (SettingsAppliedForNewDb != null)
            {
                db.Collation = SettingsAppliedForNewDb.Collation;
                db.RecoveryModel = SettingsAppliedForNewDb.RecoveryModel;
                db.CompatibilityLevel = SettingsAppliedForNewDb.CompatibilityLevel;
                db.AutoClose = SettingsAppliedForNewDb.AutoClose;
                db.AutoCreateStatisticsEnabled = SettingsAppliedForNewDb.AutoCreateStatisticsEnabled;
                db.AutoShrink = SettingsAppliedForNewDb.AutoShrink;
                db.AutoUpdateStatisticsEnabled = SettingsAppliedForNewDb.AutoUpdateStatisticsEnabled;
                db.AutoUpdateStatisticsAsync = SettingsAppliedForNewDb.AutoUpdateStatisticsAsync;
            }

            server.Databases.Add(db);
            db.Create();

            ExecuteScriptFiles(CreateScriptFiles, db);
            return db;
        }

        public string DbName { get; set; }
        public string DbServer { get; set; }
        public string BackupPath { get; set; }
        public OpeningOptions OpeningOptions { get; set; }
        public List<string> ScriptFiles { get; set; }
        public List<string> CreateScriptFiles { get; set; }
        public bool DropDb { get; set; }
        public DbSettings SettingsAppliedForNewDb { get; set; }
    }
}