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
namespace dropkick.Tasks.Security.Msmq
{
    using System.Messaging;
    using DeploymentModel;

    public class MsmqGrantWriteTask :
        BaseTask
    {
        readonly string _group;
        readonly string _queueName;

        public MsmqGrantWriteTask(string @group, string queueName)
        {
            _group = group;
            _queueName = queueName;
        }

        public override string Name
        {
            get { return "Grant write to '{0}'".FormatWith(_group); }
        }

        public override DeploymentResult VerifyCanRun()
        {
            var result = new DeploymentResult();

            VerifyInAdministratorRole(result);

            return result;
        }

        public override DeploymentResult Execute()
        {
            var result = new DeploymentResult();
            var q = new MessageQueue(_queueName);
            //do stuff
            return result;
        }
    }
}