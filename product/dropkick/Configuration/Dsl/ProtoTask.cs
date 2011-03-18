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
    using DeploymentModel;

    public interface ProtoTask :
        DeploymentInspectorSite
    {
        void RegisterRealTasks(PhysicalServer server);
    }

    public static class TaskHelpers
    {
        public static DeploymentDetail ToDetail(this Task task, DeploymentServer server)
        {
            var d = new DeploymentDetail(() => task.Name, task.VerifyCanRun, task.Execute, ()=>Tracer(task) );

            return d;
        }

        public static DeploymentResult Tracer(Task task)
        {
            var r = new DeploymentResult();
            r.AddNote(task.Name);
            return r;
        }
    }
}