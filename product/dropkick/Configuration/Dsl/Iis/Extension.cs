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
namespace dropkick.Configuration.Dsl.Iis
{
    using FileSystem;

    public static class Extension //ProtoTaskBuilder
    {
        public static IisSiteOptions Iis6Site(this ProtoServer protoServer, string websiteName)
        {
            var task = new IisProtoTask(websiteName, new DotNetPath())
                       {
                           Version = IisVersion.Six
                       };
            protoServer.RegisterProtoTask(task);
            return task;
        }

        public static IisSiteOptions Iis7Site(this ProtoServer protoServer, string websiteName)
        {
            var task = new IisProtoTask(websiteName, new DotNetPath())
                           {
                               Version = IisVersion.Seven,
                       };
            protoServer.RegisterProtoTask(task);
            return task;
        }

        public static IisSiteOptions Iis7Site(this ProtoServer protoServer, string websiteName, string pathForWebsite, int port)
        {
            var task = new IisProtoTask(websiteName,new DotNetPath())
                           {
                               Version = IisVersion.Seven,
                               PathForWebsite = pathForWebsite,
                               PortForWebsite = port
                       };
            protoServer.RegisterProtoTask(task);
            return task;
        }
    }
}