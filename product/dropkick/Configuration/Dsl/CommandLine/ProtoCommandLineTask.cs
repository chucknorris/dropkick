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
namespace dropkick.Configuration.Dsl.CommandLine
{
    using System;
    using DeploymentModel;
    using FileSystem;
    using Tasks;
    using Tasks.CommandLine;

    public class ProtoCommandLineTask :
        BaseProtoTask,
        CommandLineOptions
    {
        readonly string _command = "";
        string _args = "";
        string _path = Environment.CurrentDirectory;
        string _workingDirectory = Environment.CurrentDirectory;

        public ProtoCommandLineTask(string command)
        {
            _command = command;
        }

        public CommandLineOptions Args(string args)
        {
            _args = ReplaceTokens(args);
            return this;
        }

        public CommandLineOptions ExecutableIsLocatedAt(string path)
        {
            _path = ReplaceTokens(path);
            return this;
        }

        public CommandLineOptions WorkingDirectory(string path)
        {
            _workingDirectory = ReplaceTokens(path);
            return this;
        }

        public override void RegisterRealTasks(PhysicalServer s)
        {
            if (s.IsLocal)
            {
                s.AddTask(new LocalCommandLineTask(new DotNetPath(), _command)
                              {
                                  Args = _args,
                                  ExecutableIsLocatedAt = _path,
                                  WorkingDirectory = _workingDirectory
                              });
                return;
            }

            s.AddTask(new RemoteCommandLineTask(_command)
                          {
                              Args = _args,
                              ExecutableIsLocatedAt = _path,
                              WorkingDirectory = _workingDirectory,
                              Machine = s.Name
                          });
        }
    }
}
