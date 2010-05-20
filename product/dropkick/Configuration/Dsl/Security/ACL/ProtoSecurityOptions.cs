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
namespace dropkick.Configuration.Dsl.Security
{
    using System;

    public class ProtoSecurityOptions :
        SecurityOptions
    {
        readonly ProtoServer _server;

        public ProtoSecurityOptions(ProtoServer server)
        {
            _server = server;
        }

        #region SecurityOptions Members

        public void LocalPolicy(Action<LocalPolicyConfig> func)
        {
            LocalPolicyConfig lpc = new ProtoLocalPolicy(_server);
            func(lpc);
        }

        public void ForPath(string path, Action<FileSecurityConfig> action)
        {
            var psc = new PathSecurityConfiguration(_server, path);
            action(psc);
        }

        public void ForQueue(string queue, Action<QueueSecurityConfig> action)
        {
            throw new NotImplementedException();
        }

        public void CreateALoginFor(string account)
        {
            throw new NotImplementedException();
        }

        public void ForDatabase(string database, Action<DatabaseSecurity> action)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}