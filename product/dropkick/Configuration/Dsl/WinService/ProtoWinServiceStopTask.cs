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
    using DeploymentModel;
    using Tasks;
    using Tasks.WinService;

    public class ProtoWinServiceStopTask :
        BaseProtoTask
    {
        readonly string _serviceName;
        readonly string _wmiUserName;
        readonly string _wmiPassword;

        public ProtoWinServiceStopTask(string serviceName, string wmiUserName=null, string wmiPassword=null)
        {
            _serviceName = ReplaceTokens(serviceName);
            if (!string.IsNullOrEmpty(wmiUserName))
            {
                _wmiUserName = ReplaceTokens(wmiUserName);
            }
            if (!string.IsNullOrEmpty(wmiPassword))
            {
                _wmiPassword = ReplaceTokens(wmiPassword);
            }
        }

        public override void RegisterRealTasks(PhysicalServer s)
        {
            s.AddTask(new WinServiceStopTask(s.Name, _serviceName, _wmiUserName, _wmiPassword));
        }
    }
}