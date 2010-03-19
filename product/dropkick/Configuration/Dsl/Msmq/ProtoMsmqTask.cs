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
namespace dropkick.Configuration.Dsl.Msmq
{
    using System;
    using DeploymentModel;
    using Tasks;
    using Tasks.Msmq;

    public class ProtoMsmqTask :
        BaseTask,
        MsmqOptions,
        QueueOptions
    {
        bool _createIfItDoesNotExist;
        string _queueName;

        #region MsmqOptions Members

        public QueueOptions PrivateQueueNamed(string name)
        {
            _queueName = name;
            return this;
        }

        #endregion

        #region QueueOptions Members

        public void CreateIfItDoesntExist()
        {
            _createIfItDoesNotExist = true;
        }

        #endregion

        public override Action<TaskSite> RegisterTasks()
        {
            return s =>
                   {
                       s.AddTask(new MsmqTask()
                                     {
                                         QueueName = _queueName,
                                         ServerName = s.Name
                                     });
                   };
        }
    }
}