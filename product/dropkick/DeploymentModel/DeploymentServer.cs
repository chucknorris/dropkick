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
namespace dropkick.DeploymentModel
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Configuration.Dsl;
    using FileSystem;

    [DebuggerDisplay("Deploy Server: {Name}")]
    public class DeploymentServer :
        PhysicalServer
    {
        //because tasks need to be customized per server
        readonly IList<DeploymentDetail> _details;

        public DeploymentServer(string name)
        {
            Name = name;
            _details = new List<DeploymentDetail>();
        }

        public int DetailCount
        {
            get { return _details.Count; }
        }

        #region PhysicalServer Members

        public string Name { get; private set; }

        public bool IsLocal
        {
            get
            {
                return Environment.MachineName.EqualsIgnoreCase(Name) 
                    || "localhost".EqualsIgnoreCase(Name)
                    //|| "127.0.0.1".EqualsIgnoreCase(Name)
                    ; 
            }
        }

        public string MapPath(string path)
        {
            return PathConverter.Convert(this, path);
        }

        public void AddTask(Task task)
        {
            _details.Add(task.ToDetail(this));
        }

        #endregion

        public void AddDetail(DeploymentDetail detail)
        {
            _details.Add(detail);
        }


        public void ForEachDetail(Action<DeploymentDetail> detailAction)
        {
            foreach (var detail in _details)
            {
                detailAction(detail);
            }
        }

        public void AddTask(ProtoTask task)
        {
            task.RegisterRealTasks(this);
        }
    }
}