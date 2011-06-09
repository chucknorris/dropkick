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
namespace dropkick.DeploymentModel
{
    public interface PhysicalServer
    {
        string Name { get; }
        bool IsLocal { get; }
        void AddTask(Task task);
        /// <summary>
        /// This will convert a path string to the physical path (local server deploy will get a local address).
        /// If you need to force the path to be local always, you should call dropkick.FileSystem.Path.GetPhysicalPath(server,path,TRUE)
        /// </summary>
        /// <param name="path">The path on the server</param>
        /// <returns>A physical path, will be a local path if the deploy run is on the same server it is deploying to</returns>
        string MapPath(string path);
    }
}