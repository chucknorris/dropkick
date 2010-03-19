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
namespace dropkick.Configuration.Dsl.NetworkShare
{
    using System;
    using DeploymentModel;
    using Tasks;
    using Tasks.NetworkShare;

    public class ProtoFolderShareTask :
        BaseTask,
        FolderShareOptions
    {
        bool _createIfNotExist;
        string _pointingTo;
        readonly string _shareName;

        public ProtoFolderShareTask(string shareName)
        {
            _shareName = shareName;
        }

        #region FolderShareOptions Members

        public FolderShareOptions PointingTo(string path)
        {
            _pointingTo = path;
            return this;
        }

        public void CreateIfNotExist()
        {
            _createIfNotExist = true;
        }

        #endregion

        public override Action<TaskSite> RegisterTasks()
        {
            return s =>
                   {
                       s.AddTask(new FolderShareTask
                                     {
                                         PointingTo = _pointingTo,
                                         Server = s.Name,
                                         ShareName = _shareName
                                     });
                   };
        }
    }
}