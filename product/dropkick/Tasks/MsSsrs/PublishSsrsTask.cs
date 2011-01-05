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
namespace dropkick.Tasks.MsSsrs
{
    using System;
    using System.Collections.Generic;
    using DeploymentModel;

    public class PublishSsrsTask :
        BaseTask
    {
        readonly string _publishTo;

        public PublishSsrsTask(string publishTo)
        {
            _publishTo = publishTo;
        }

        public override string Name
        {
            get { return "Publish SSRS"; }
        }

        public override DeploymentResult VerifyCanRun()
        {
            throw new NotImplementedException();
        }

        public override DeploymentResult Execute()
        {
            throw new NotImplementedException();
        }

        public void AddReportsIn(string folder)
        {
            
        }

        public void AddReport(List<string> reports)
        {
            
        }
    }
}