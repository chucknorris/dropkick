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
namespace dropkick.Configuration.Dsl.Security.ACL
{
    public interface FileSecurityConfig
    {
        /// <summary>
        /// Removes all users/groups who are not inherited or in the preserve list (defined in options). 
        /// </summary>
        ClearAclOptions Clear();

        /// <summary>
        /// Removes ACL inheritance from a folder. When used in conjunction with Clear(), you can really lock down a folder.
        /// </summary>
        void RemoveInheritance();
        void GrantRead(string group);
        void GrantReadWrite(string group);

        /// <summary>
        /// Don't forget the DK config file supports enums
        /// </summary>
        void Grant(Rights readWrite, string group);
    }
}