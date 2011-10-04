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
using System;

namespace dropkick.Configuration.Dsl.NetworkShare
{
    using DeploymentModel;
    using Tasks;
    using Tasks.NetworkShare;

    public class ProtoFolderShareTask :
        BaseProtoTask,
        FolderShareOptions
    {
		readonly ExistingOptions _existingShareOptions = new ExistingOptions();
		readonly string _shareName;
        string _pointingTo;

    	public ProtoFolderShareTask(string shareName)
        {
            _shareName = ReplaceTokens(shareName);
        }

        public FolderShareOptions PointingTo(string path)
        {
            _pointingTo = ReplaceTokens(path);
            return this;
        }

        public override void RegisterRealTasks(PhysicalServer s)
        {
            s.AddTask(new FolderShareTask
                          {
                              PointingTo = _pointingTo,
                              Server = s.Name,
                              ShareName = _shareName,
							  DeleteAndRecreate = _existingShareOptions.deleteAndRecreate
                          });
        }

    	public ExistingShareOptions IfExists
    	{
			get { return _existingShareOptions; }
    	}

		private class ExistingOptions : ExistingShareOptions
		{
			internal bool deleteAndRecreate;

			public void DeleteAndRecreate()
			{
				deleteAndRecreate = true;
			}
		}
    }
}