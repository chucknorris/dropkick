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

    public class CreateQueueTask :
        BaseTask
    {
        readonly string _queueName;
        readonly bool _transactional;

        public CreateQueueTask(string queueName, bool transactional)
        {
            _queueName = queueName;
            _transactional = transactional;
        }

        public override string Name
        {
            get { return "Create queue"; }
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

            MessageQueue q = MessageQueue.Create(_queueName, _transactional);
            result.AddGood("Created the queue");

            return result;
        }
    }
}