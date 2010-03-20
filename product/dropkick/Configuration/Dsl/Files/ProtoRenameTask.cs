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
    using DeploymentModel;
    using Tasks;

    public class ProtoRenameTask :
        BaseTask,
        RenameOptions
    {
        string _target;
        string _to;

        public ProtoRenameTask(string target)
        {
            _target = target;
        }

        #region RenameOptions Members

        public void Rename(string name)
        {
            _to = name;
        }

        #endregion

        public override void RegisterTasks(TaskSite s)
        {
            throw new NotImplementedException();
        }
    }
}