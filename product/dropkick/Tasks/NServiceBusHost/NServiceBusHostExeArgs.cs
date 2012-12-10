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
namespace dropkick.Tasks.NServiceBusHost
{
    using Prompting;

    public class NServiceBusHostExeArgs
    {
        readonly string _args;

        public NServiceBusHostExeArgs(string exeName,
                                      string instanceName,
                                      string username,
                                      string password,
                                      string serviceName,
                                      string displayName,
                                      string description,
                                      string profiles)
        {
            _args = "/install";

            if (!string.IsNullOrEmpty(instanceName))
                _args += " /instance:\"{0}\"".FormatWith(instanceName);

            if (username != null && password != null)
                _args += " /userName:\"{0}\" /password:\"{1}\"".FormatWith(username, password);

            if (!string.IsNullOrEmpty(serviceName))
                _args += " /serviceName:\"{0}\"".FormatWith(serviceName);

            if (!string.IsNullOrEmpty(displayName))
                _args += " /displayName:\"{0}\"".FormatWith(displayName);

            if (!string.IsNullOrEmpty(description))
                _args += " /description:\"{0}\"".FormatWith(description);

            if (!string.IsNullOrEmpty(profiles))
                _args += " {0}".FormatWith(profiles);
        }

        public override string ToString()
        {
            return _args;
        }
    }
}