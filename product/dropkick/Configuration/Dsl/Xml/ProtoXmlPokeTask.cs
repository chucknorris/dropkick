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
namespace dropkick.Configuration.Dsl.Xml
{
    using System.Collections.Generic;
    using DeploymentModel;
    using FileSystem;
    using Tasks;
    using Tasks.Xml;

    public class ProtoXmlPokeTask :
        BaseProtoTask,
        XmlPokeOptions
    {
        private readonly string _filePath;
        private IDictionary<string, string> _items = new Dictionary<string, string>();

        public ProtoXmlPokeTask(string filePath)
        {
            _filePath = ReplaceTokens(filePath);
        }

        public XmlPokeOptions Set(string xPath, string value)
        {
            if (_items.ContainsKey(xPath))
            {
                _items[xPath] = value;
            } else
            {
                _items.Add(xPath,value);
            }

            return this;
        }

        public override void RegisterRealTasks(PhysicalServer site)
        {
            string filePath = site.MapPath(_filePath);

            var o = new XmlPokeTask(filePath, _items, new DotNetPath());
            site.AddTask(o);
        }
    }
}