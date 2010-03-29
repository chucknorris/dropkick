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
namespace dropkick.Configuration.Dsl
{
    using System.Collections.Generic;
    using DeploymentModel;

    public class PrototypicalServer :
        ProtoServer
    {
        readonly IList<ProtoTask> _tasks = new List<ProtoTask>();

        #region Server Members

        public void InspectWith(DeploymentInspector inspector)
        {
            inspector.Inspect(this, () =>
            {
                foreach (ProtoTask task in _tasks)
                {
                    task.InspectWith(inspector);
                }
            });
        }

        public void MapTo(DeploymentServer server)
        {
            foreach (var task in _tasks)
            {
                task.RegisterRealTasks(server);
            }
        }

        public void RegisterProtoTask(ProtoTask protoTask)
        {
            _tasks.Add(protoTask);
        }

        #endregion
    }
}