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
namespace dropkick.Configuration.Dsl.Files
{
    using System;
    using System.Collections.Generic;
    using DeploymentModel;
    using Tasks;
    using Tasks.Files;

    public class ProtoCopyTask :
        BaseTask,
        CopyOptions,
        FromOptions
    {
        readonly Server _server;
        string _to;
        Action<FileActions> _followOn;
        readonly IList<string> _froms = new List<string>();
        readonly IList<ProtoRenameTask> _renameTasks = new List<ProtoRenameTask>();

        public ProtoCopyTask(Server server)
        {
            _server = server;
        }

        public RenameOptions Include(string path)
        {
            _froms.Add(path);
            var rn = new ProtoRenameTask(path);
            _renameTasks.Add(rn);
            return rn;
        }

        public CopyOptions From(string sourcePath)
        {
            _froms.Add(sourcePath);
            return this;
        }

        public CopyOptions To(string destinationPath)
        {
            _to = destinationPath;
            return this;
        }

        public void With(Action<FileActions> copyAction)
        {
            _followOn = copyAction;
            copyAction(new SomeFileActions(_server));
        }


        public override Action<TaskSite> RegisterTasks()
        {
            var mt = new MultiCopyTask();

            foreach (var f in _froms)
            {
                var o = new CopyTask(f, _to);
                mt.Add(o);
            }

            //TODO: Add renames in here
            return s =>
                   {
                       s.AddTask(mt);
                   };
        }
    }
}