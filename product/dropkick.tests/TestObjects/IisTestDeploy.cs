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
namespace dropkick.tests.TestObjects
{
    using dropkick.Configuration.Dsl;
    using dropkick.Configuration.Dsl.Iis;

    public class IisTestDeploy :
        Deployment<IisTestDeploy, SampleConfiguration>
    {
        public IisTestDeploy()
        {
            Define(settings => DeploymentStepsFor(Web, server =>
            {
                //TODO: make this a prompt
                server.Iis7Site("Default Web Site", @"D:\IntNet", 80)
                    .VirtualDirectory("dk_test");

                server.Iis7Site("Default Web Site")
                    .VirtualDirectory("fp")
                    .SetAppPoolTo("appPoolName", pool=>
                    {
                        pool.Enable32BitAppOnWin64();
                        pool.UseClassicPipeline();
                    })
                    .SetPathTo(@"~\websites\my_web");
                //     '\\server' + \websites\my_web
            }));
        }

        public static Role Web { get; set; }
    }
}