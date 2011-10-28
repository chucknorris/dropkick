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
using Microsoft.Web.Administration;

namespace dropkick.Configuration.Dsl.Iis
{
    using System;

    public interface IisVirtualDirectoryOptions
    {
        IisVirtualDirectoryOptions SetPathTo(string path);
        IisVirtualDirectoryOptions SetAppPoolTo(string appPoolName);
        IisVirtualDirectoryOptions SetAppPoolTo(string appPoolName, Action<AppPoolOptions> action);
    }

    public interface AppPoolOptions
    {
        void Enable32BitAppOnWin64();
        void UseClassicPipeline();
        void SetRuntimeToV4();
    	void SetProcessModelIdentity(string username, string password);
    	void SetProcessModelIdentity(ProcessModelIdentity identity);
    }
}