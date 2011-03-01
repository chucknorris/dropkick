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
namespace dropkick.Tasks
{
    using System.Threading;
    using DeploymentModel;

    public abstract class BaseTask :
        Task
    {
        public abstract string Name { get; }
        public abstract DeploymentResult VerifyCanRun();
        public abstract DeploymentResult Execute();
        

        protected void VerifyInAdministratorRole(DeploymentResult result)
        {
            if (Thread.CurrentPrincipal.IsInRole("Administrator"))
            {
                result.AddAlert("You are not in the 'Administrator' role. You will not be able to start/stop services");
            }
            else
            {
                result.AddGood("You are in the 'Administrator' role");
            }
        }

        public void LogFineGrain(string format, params object[] args)
        {
            Logging.Fine(format, args);
        }

        public void LogCoarseGrain(string format, params object[] args)
        {
            Logging.Coarse(format, args);
        }
    }
}