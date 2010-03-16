//// Copyright 2007-2008 The Apache Software Foundation.
//// 
//// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
//// this file except in compliance with the License. You may obtain a copy of the 
//// License at 
//// 
////     http://www.apache.org/licenses/LICENSE-2.0 
//// 
//// Unless required by applicable law or agreed to in writing, software distributed 
//// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
//// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
//// specific language governing permissions and limitations under the License.
//namespace dropkick.Configuration.Dsl.Iis
//{
//    using System;
//    using Tasks;
//
//    public static class IisVersionSelector
//    {
//        public static IisSiteOptions SelectTheCorrectConfig(Server server, string websiteName)
//        {
//            //how to go from taskbuilder -> task -> detail?
//            server.RegisterTask(new NoteProtoTask("IIS Version Detection Used"));
//
//            if (Environment.OSVersion.Version.Major == 1)
//            {
//                server.RegisterTask(new NoteProtoTask("IIS Version was automatically set to IIS6"));
//                return new Iis6TaskCfg(server, websiteName);
//            }
//            else if (Environment.OSVersion.Version.Major == 6)
//            {
//                server.RegisterTask(new NoteProtoTask("IIS Version was automatically set to IIS7"));
//                return new Iis7TaskCfg(server, websiteName);
//            }
//            else
//            {
//                throw new Exception("Can't figure out which version of IIS to run");
//            }
//        }
//    }
//}