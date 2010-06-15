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
namespace dropkick.Tasks
{
    using System.Collections.Generic;
    using DeploymentModel;

    public class NestedTask :
        Task
    {
        readonly IList<Task> _tasks = new List<Task>();

        #region Task Members

        public string Name
        {
            get { return "NESTED TASK"; }
        }

        public DeploymentResult VerifyCanRun()
        {
            var result = new DeploymentResult();
            foreach (var task in _tasks)
            {
                DeploymentResult r = task.VerifyCanRun();
                result = result.MergedWith(r);
            }
            return result;
        }

        public DeploymentResult Execute()
        {
            var result = new DeploymentResult();
            foreach (var task in _tasks)
            {
                DeploymentResult r = task.Execute();
                result = result.MergedWith(r);
            }
            return result;
        }

        #endregion

        public void AddTask(Task task)
        {
            _tasks.Add(task);
        }
    }
}