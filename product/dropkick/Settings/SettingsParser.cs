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
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using Magnum.CommandLineParser;
    using Magnum.Reflection;

    public class SettingsParser
    {
        readonly ICommandLineParser _parser = new MonadicCommandLineParser();

        public T Parse<T>(string contents) where T : new()
        {
            var result = new T();

            IEnumerable<ICommandLineElement> po = _parser.Parse(contents);

            Set(typeof (T), result, po);

            return result;
        }

        public T Parse<T>(FileInfo file) where T : new()
        {
            return (T) Parse(typeof (T), file);
        }

        public object Parse(Type t, FileInfo file)
        {
            object result = FastActivator.Create(t);

            string contents = File.ReadAllText(file.FullName);
            IEnumerable<ICommandLineElement> po = _parser.Parse(contents);

            Set(t, result, po);

            return result;
        }

        void Set(Type type, object result, IEnumerable<ICommandLineElement> enumerable)
        {
            foreach (IDefinitionElement argument in enumerable)
            {
                PropertyInfo pi = type.GetProperty(argument.Key);
                var fp = new FastProperty(pi);
                fp.Set(result, Convert.ChangeType(argument.Value, pi.PropertyType));
            }
        }
    }
}