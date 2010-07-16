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
// specific language governing permissions and limitations under the License.
namespace dropkick.Configuration.Dsl.RoundhousE
{
    using DeploymentModel;
    using Tasks;
    using Tasks.CommandLine;

    public class RoundhousEProtoTask :
        BaseProtoTask,
        RoundhousEOptions
    {
        public RoundhousEProtoTask()
        {
            SqlFilesLocation = ".\\database";
            InstanceName = ".";
        }

        public string EnvironmentName { get; private set; }
        public string InstanceName { get; private set; }
        public string DatabaseName { get; private set; }
        public string SqlFilesLocation { get; private set; }
        public bool UseSimpleRestoreMode { get; private set; }
        public string DatabaseType { get; private set; }

        public RoundhousEOptions Environment(string name)
        {
            EnvironmentName = ReplaceTokens(name);
            return this;
        }

        public RoundhousEOptions OnInstance(string name)
        {
            InstanceName = ReplaceTokens(name);
            return this;
        }

        public RoundhousEOptions OnDatabase(string name)
        {
            DatabaseName = ReplaceTokens(name);
            return this;
        }

        public RoundhousEOptions UseMsSqlServer2005()
        {
            DatabaseType = "2005";
            return this;
        }

        public RoundhousEOptions UseMsSqlServer2008()
        {
            DatabaseType = "2008";
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

            rhtask.Args = "/d={0} /f={1} /s={2} /env={3} /dt={4}".FormatWith(DatabaseName, SqlFilesLocation, site.Name,
                                                                             EnvironmentName, DatabaseType);
            if (UseSimpleRestoreMode)
                rhtask.Args += " /simple";

            site.AddTask(rhtask);
        }
    }
}