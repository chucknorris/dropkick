// Copyright 2007-2011 The Apache Software Foundation.
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
namespace dropkick.Configuration.Dsl.MsSrss
{
    using System.Collections.Generic;
    using DeploymentModel;
    using Tasks;
    using Tasks.MsSsrs;

    public class ProtoSsrsTask :
        BaseProtoTask,
        ReportOptions
    {
        string _publishTo;
        string _publishAllIn;
        readonly List<string> _publish;

        public ProtoSsrsTask()
        {
            _publish = new List<string>();
        }

        public void PublishTo(string address)
        {
            _publishTo = address;
        }

        public void PublishAllIn(string folder)
        {
            _publishAllIn = folder;
        }

        public void Publish(string name)
        {
            _publish.Add(name);
        }

        public override void RegisterRealTasks(PhysicalServer site)
        {
            var task = new PublishSsrsTask(_publishTo);
            task.AddReportsIn(_publishAllIn);
            task.AddReport(_publish);
            site.AddTask(task);
        }
    }
}