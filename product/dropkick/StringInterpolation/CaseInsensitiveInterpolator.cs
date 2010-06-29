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
namespace dropkick.StringInterpolation
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using Magnum.Reflection;

    public class CaseInsensitiveInterpolator :
        Interpolator
    {
        readonly Regex _pattern = new Regex("{{(?<key>\\w+)}}");
        readonly Dictionary<Type, Dictionary<string, FastProperty>>  _properties = new Dictionary<Type, Dictionary<string, FastProperty>>();


        public string DoIt<SETTINGS>(SETTINGS settings, string input)
        {
            PrepareDictionary<SETTINGS>();
            string output = _pattern.Replace(input, m =>
            {
                string key = m.Groups["key"].Value;
                var pi = _properties[typeof(SETTINGS)][key];
                var value = (string) pi.Get(settings);
                return value;
            });

            return output;
        }

        void PrepareDictionary<T>()
        {
            if(!_properties.ContainsKey(typeof(T)))
            {
                var dict = new Dictionary<string, FastProperty>(StringComparer.InvariantCultureIgnoreCase);
                _properties.Add(typeof(T), dict);
                foreach (var propertyInfo in typeof(T).GetProperties())
                {
                    dict.Add(propertyInfo.Name, new FastProperty(propertyInfo));
                }
            }
        }
    }
}