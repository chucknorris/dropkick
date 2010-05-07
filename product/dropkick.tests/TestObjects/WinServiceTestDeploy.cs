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
namespace dropkick.tests.TestObjects
{
    using dropkick.Configuration.Dsl;
    using dropkick.Configuration.Dsl.Msmq;
    using dropkick.Configuration.Dsl.WinService;
    using Wmi;

    public class WinServiceTestDeploy :
        Deployment<WinServiceTestDeploy, object>
    {
        public WinServiceTestDeploy()
        {
            //this is just a means to check the nested closure would work, not that one would actually do this
            Define((settings, environment) =>
                   DeploymentStepsFor(Web, server =>
                                           {
                                               server.WinService("MSMQ").Do(s => s.Msmq().PrivateQueueNamed("dru"));

                                               server.WinService("FHLB").Delete();
                                               server.WinService("FHLB").Create()
                                                   .WithDescription("")
                                                   .WithServicePath("E:\\myservice")
                                                   .WithStartMode(ServiceStartMode.Automatic);
                                           }));
        }

        public static Role Web { get; set; }
        public static Role File { get; set; }
    }
}