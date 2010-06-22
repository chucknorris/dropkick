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
namespace dropkick.Tasks.Msmq
{
    using System;
    using System.Messaging;
    using DeploymentModel;
    using Dsl.Msmq;

    public class MsmqTask :
        BaseTask
    {
        PhysicalServer _server;

        public MsmqTask(PhysicalServer server, QueueAddress address)
        {
            _server = server;
            Address = address;
        }
        public MsmqTask(PhysicalServer server, string queueName)
        {
            _server = server;
            var ub = new UriBuilder("msmq", server.Name) { Path = queueName };
            Address = new QueueAddress(ub.Uri);
        }
        public QueueAddress Address { get; set; }


        public override string Name
        {
            get { return string.Format("MsmqTask for server '{0}' and private queue named '{1}'", _server.Name, Address); }
        }

        public override DeploymentResult VerifyCanRun()
        {
            var result = new DeploymentResult();

            VerifyInAdministratorRole(result);

            if (_server.IsLocal)
            {
                if (MessageQueue.Exists(Address.LocalName))
                {
                    result.AddGood("'{0}' does exist");
                }
                else
                {
                    result.AddAlert(string.Format("'{0}' doesn't exist and will be created.", Address.ActualUri));
                }
            }
            else
            {
                result.AddAlert(string.Format("Cannot check for queue '{0}' on server '{1}' while on server '{2}'",
                                              Address, _server.Name, Environment.MachineName));
            }


            return result;
        }

        public override DeploymentResult Execute()
        {
            var result = new DeploymentResult();

            if (_server.IsLocal)
                ProcessLocalQueue(result);
            else
                ProcessRemoteQueue(result);

            return result;
        }

        void ProcessRemoteQueue(DeploymentResult result)
        {
            var message = "Cannot create a private queue on the remote machine '{0}' while on '{1}'."
                .FormatWith(_server.Name, Environment.MachineName);

            result.AddError(message);
        }

        void ProcessLocalQueue(DeploymentResult result)
        {
            if (!MessageQueue.Exists(Address.LocalName))
            {
                result.AddAlert("'{0}' does not exist and will be created.".FormatWith(Address.FormatName));
                MessageQueue.Create(Address.LocalName);
                result.AddGood("Created queue '{0}'".FormatWith(Address.FormatName));
            }
            else
            {
                result.AddGood("'{0}' already exists.".FormatWith(Address.FormatName));
            }
        }
    }
}