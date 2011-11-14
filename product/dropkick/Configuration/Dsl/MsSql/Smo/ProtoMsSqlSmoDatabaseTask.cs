// Copyright 2007-2008 The Apache Software Foundation.
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
// specific language governing permissions and limitations under the License.

using System;

namespace dropkick.Configuration.Dsl.MsSql.Smo
{
    using System.Collections.Generic;
    using System.Linq;
    using DeploymentModel;
    using Tasks;
    using Tasks.MsSql.Smo;

    public class ProtoMsSqlSmoDatabaseTask : BaseProtoTask, DbServerOptions
    {
        private readonly List<DbOptionsImpl> _databases = new List<DbOptionsImpl>();
        private string _dbServer;

        public DbServerOptions DatabaseServer(string name)
        {
            _dbServer = name;
            return this;
        }

        public DbOptions Database(string name, OpeningOptions options = OpeningOptions.FailIfDoesntExists)
        {
            var dbOptions = new DbOptionsImpl(_dbServer, name, options);
            _databases.Add(dbOptions);
            return dbOptions;
        }

        public override void RegisterRealTasks(PhysicalServer server)
        {
            foreach (var task in _databases.Select(i => CreateTask(i, server)))
            {
                server.AddTask(task);
            }
        }

        private Task CreateTask(DbOptionsImpl options, PhysicalServer server)
        {
            var task = new DatabaseTask();
            task.OpeningOptions = options.OpeningOptions;
            task.DbServer = options.DbServer;
            task.DbName = options.DbName;
            task.ScriptFiles = options.ScriptFiles.Select(server.MapPath).ToList();
            task.CreateScriptFiles = options.CreateScriptFiles.Select(server.MapPath).ToList();
            task.DropDb = options.DropDb;
            task.SettingsAppliedForNewDb = options.DbSettings;
            return task;
        }

        private class DbOptionsImpl : DbOptions
        {
            public DbOptionsImpl(string dbServer, string dbName, OpeningOptions options)
            {
                DbServer = dbServer;
                DbName = dbName;
                OpeningOptions = options;
                ScriptFiles = new List<string>();
                CreateScriptFiles = new List<string>();
            }

            public DbSettings DbSettings { get; set; }
            public string DbServer { get; set; }
            public OpeningOptions OpeningOptions { get; set; }
            public List<string> ScriptFiles { get; set; }
            public bool DropDb { get; set; }
            public string DbName { get; set; }

            public void RunScriptFile(string scriptFile)
            {
                ScriptFiles.Add(scriptFile);
            }

            public void RunCreateScriptFile(string scriptFile)
            {
                CreateScriptFiles.Add(scriptFile);
            }

            public List<string> CreateScriptFiles { get; set; }

            public void Drop()
            {
                DropDb = true;
            }

            public void WithSettings(Action<DbSettings> updateSettings)
            {                
                if (DbSettings == null)
                {
                    DbSettings = new DbSettings();
                }
                updateSettings(DbSettings);
            }
        }
    }

}