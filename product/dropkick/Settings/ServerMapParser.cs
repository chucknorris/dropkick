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
namespace dropkick.Settings
{
    using System.Collections.Generic;
    using System.IO;
    using Engine;
    using Magnum.CommandLineParser;
    using Magnum.Configuration;

    public class ServerMapParser
    {
        readonly ICommandLineParser _parser = new MonadicCommandLineParser();

        public RoleToServerMap Parse(FileInfo file)
        {
            var binder = ConfigurationBinderFactory.New(c =>
            {
                //c.AddJsonFile(file.FullName);
                var content = File.ReadAllText(file.FullName);
                c.AddJson(content);
            });


            var d = binder.GetAll();

            var result = new RoleToServerMap();



            foreach (var kvp in d)
            {
                result.AddMap(kvp.Key, kvp.Value.ToString());
            }
                
           

            return result;
        }
    }
}