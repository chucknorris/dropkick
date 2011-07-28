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
    using System;
    using System.IO;
    using Magnum.Configuration;

    public class SettingsParser
    {
        public T Parse<T>(FileInfo file, string commandLine, string environment) where T : new()
        {
            return (T)Parse(typeof (T), file, commandLine, environment);
        }

        public object Parse(Type t, FileInfo file, string commandLine, string environment)
        {
            var binder = ConfigurationBinderFactory.New(c =>
            {
                //c.AddJsonFile("global.conf");
                //c.AddJsonFile("{0}.conf".FormatWith(environment));
                //c.AddJsonFile(file.FullName);
                var content = File.ReadAllText(file.FullName);
                c.AddJson(content);
                c.AddCommandLine(commandLine);
            });

            
            object result = binder.Bind(t);

            return result;
        }
    }
}