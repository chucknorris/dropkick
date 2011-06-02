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
using System.Collections.Generic;
using dropkick.Tasks.Security;

namespace dropkick.Configuration.Dsl.Security.ACL
{
    public class PathSecurityConfiguration :
        FileSecurityConfig
    {
        readonly string _path;
        readonly ProtoServer _server;

        public PathSecurityConfiguration(ProtoServer server, string path)
        {
            _server = server;
            _path = path;
        }

        /// <summary>
        /// Removes all users/groups who are not inherited or in the preserve list (defined in options). 
        /// </summary>
        public ClearAclOptions Clear()
        {
            var proto = new ProtoPathClearAclsTask(_path);
            _server.RegisterProtoTask(proto);

            return proto;
        }

        /// <summary>
        /// Removes ACL inheritance from a folder. When used in conjunction with Clear(), you can really lock down a folder.
        /// </summary>
        public void RemoveInheritance()
        {
            var proto = new ProtoPathRemoveAclInheritanceTask(_path);
            _server.RegisterProtoTask(proto);
        }

        public void GrantRead(string group)
        {
            var proto = new ProtoPathGrantReadTask(_path, group);
            _server.RegisterProtoTask(proto);
        }

        public void GrantReadWrite(string group)
        {
            var proto = new ProtoPathGrantReadWriteTask(_path, group);
            _server.RegisterProtoTask(proto);
        }

        public void Grant(Rights readWrite, string group)
        {
            switch(readWrite)
            {
                case Rights.ReadWrite:
                    {
                        GrantReadWrite(group);
                        break;
                    }
                default:
                    {
                        GrantRead(group);
                        break;
                    }
            }
        }
    }
}