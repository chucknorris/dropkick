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
namespace dropkick.Configuration.Dsl.WinService
{
    using System;
    using DeploymentModel;
    using Tasks;
    using Tasks.WinService;

    public class ProtoWinServiceTask :
        WinServiceOptions
    {
        readonly Server _server;
        readonly string _serviceName;

        public ProtoWinServiceTask(Server server, string serviceName)
        {
            _server = server;
            _serviceName = serviceName;
        }

        #region WinServiceOptions Members

        public WinServiceOptions Do(Action<Server> registerAdditionalActions)
        {
            _server.RegisterTask(new ProtoWinServiceStopTask(_serviceName));

            //child task
            registerAdditionalActions(_server);


            _server.RegisterTask(new ProtoWinServiceStartTask(_serviceName));

            return this;
        }

        public void Start()
        {
            _server.RegisterTask(new ProtoWinServiceStartTask(_serviceName));
        }

        public void Stop()
        {
            _server.RegisterTask(new ProtoWinServiceStopTask(_serviceName));
        }

        #endregion
    }

    public class ProtoWinServiceStopTask :
        BaseTask
    {
        readonly string _serviceName;

        public ProtoWinServiceStopTask(string serviceName)
        {
            _serviceName = serviceName;
        }

        public override void RegisterTasks(TaskSite s)
        {
            s.AddTask(new WinServiceStopTask(s.Name, _serviceName));
        }
    }

    public class ProtoWinServiceStartTask :
        BaseTask
    {
        readonly string _serviceName;

        public ProtoWinServiceStartTask(string serviceName)
        {
            _serviceName = serviceName;
        }

        public override void RegisterTasks(TaskSite s)
        {
            s.AddTask(new WinServiceStartTask(s.Name, _serviceName));
        }
    }
}