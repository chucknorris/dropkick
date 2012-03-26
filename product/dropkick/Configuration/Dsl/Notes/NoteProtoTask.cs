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
namespace dropkick.Configuration.Dsl.Notes
{
    using DeploymentModel;
    using Tasks;

    public class NoteProtoTask :
        BaseProtoTask
    {
        public NoteProtoTask(string messageFormat, params object[] args) : this(DeploymentItemStatus.Note, string.Format(messageFormat, args)) { }
        public NoteProtoTask(string message) : this(DeploymentItemStatus.Note, message) { }
        public NoteProtoTask(DeploymentItemStatus status, string messageFormat, params object[] args) : this(status, string.Format(messageFormat, args)) { }
        public NoteProtoTask(DeploymentItemStatus status, string message)
        {
            Message = ReplaceTokens(message);
            Status = status;
        }

        public string Message { get; private set; }
        public DeploymentItemStatus Status { get; private set; }
       
        public override void RegisterRealTasks(PhysicalServer s)
        {
           s.AddTask(new NoteTask(Message, Status));
        }
    }
}