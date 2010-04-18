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

    public class ProtoWinServiceTask :
        WinServiceOptions
    {
        readonly ProtoServer _protoServer;
        readonly string _serviceName;

        public ProtoWinServiceTask(ProtoServer protoServer, string serviceName)
        {
            _protoServer = protoServer;
            _serviceName = serviceName;
        }


        public WinServiceOptions Do(Action<ProtoServer> registerAdditionalActions)
        {
            _protoServer.RegisterProtoTask(new ProtoWinServiceStopTask(_serviceName));

            //child task
            registerAdditionalActions(_protoServer);

            
            _protoServer.RegisterProtoTask(new ProtoWinServiceStartTask(_serviceName));

            return this;
        }

        public void Start()
        {
            _protoServer.RegisterProtoTask(new ProtoWinServiceStartTask(_serviceName));
        }

        public void Stop()
        {
            _protoServer.RegisterProtoTask(new ProtoWinServiceStopTask(_serviceName));
        }

        public WinServiceCreateOptions Create()
        {
            var proto = new ProtoWinServiceCreateTask(_serviceName);
            _protoServer.RegisterProtoTask(proto);
            return proto;
        }

        public void Delete()
        {
            _protoServer.RegisterProtoTask(new ProtoWinServiceDeleteTask(_serviceName));
        }
    }
}