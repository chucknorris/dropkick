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
namespace dropkick.Tasks.Xml
{
    using System;
    using System.Collections.Generic;
    using System.Xml;
    using System.Xml.XPath;
    using DeploymentModel;
    using Files;
    using Path = FileSystem.Path;

    public class XmlPokeTask :
        BaseIoTask
    {

        private string _filePath;
        private readonly IDictionary<string, string> _replacementItems;
        private readonly IDictionary<string, string> _namespacePrefixes; 

        public XmlPokeTask(string filePath, IDictionary<string, string> replacementItems, Path path)
            : this(filePath, replacementItems, path, new Dictionary<string, string>())
        {
        }

        public XmlPokeTask(string filePath, IDictionary<string, string> replacementItems, Path path, IDictionary<string, string> namespacePrefixes)
            : base(path)
        {
            _filePath = filePath;
            _replacementItems = replacementItems;
            _namespacePrefixes = namespacePrefixes;
        }

        public override string Name
        {
            get { return string.Format("Updated XML values in '{0}'", _filePath); }
        }

        public override DeploymentResult VerifyCanRun()
        {
            var result = new DeploymentResult();

            _filePath = _path.GetFullPath(_filePath);
            ValidateIsFile(result, _filePath);

            result.AddGood(Name);

            return result;
        }

        public override DeploymentResult Execute()
        {
            var result = new DeploymentResult();

            _filePath = _path.GetFullPath(_filePath);
            ValidateIsFile(result, _filePath);

            UpdateXmlFile(result, _filePath, _replacementItems, _namespacePrefixes);

            result.AddGood(Name);
            return result;
        }

        private void UpdateXmlFile(DeploymentResult result, string filePath, IDictionary<string, string> replacementItems, IDictionary<string, string> namespacePrefixes)
        {
            //XmlTextReader xmlReader = new XmlTextReader(_filePath);
            //XPathDocument xmlDoc = new XPathDocument(xmlReader);
            //XPathExpression expression = xpathNavigator.Compile(xPath);

            LogFileChange("[xmlpoke] Starting changes to '{0}'.", filePath);

            XmlDocument document = new XmlDocument();
            document.Load(filePath);
            var nsManager = new XmlNamespaceManager(document.NameTable);
            foreach (var prefix in namespacePrefixes)
            {
                nsManager.AddNamespace(prefix.Key, prefix.Value);
            }

            XPathNavigator xpathNavigator = document.CreateNavigator();
            foreach (KeyValuePair<string, string> item in replacementItems.OrEmptyListIfNull())
            {
                UpdateValueInFile(result, xpathNavigator, item.Key, item.Value, nsManager);
            } 

            LogFileChange("[xmlpoke] Completed changes to '{0}'.",filePath);
            
            document.Save(filePath);
        }

        private void UpdateValueInFile(DeploymentResult result, XPathNavigator xpathNavigator, string xPath, string value, XmlNamespaceManager nsManager)
        {
            foreach (XPathNavigator navigator in xpathNavigator.Select(xPath, nsManager))
            {
                string original = navigator.Value;
                navigator.SetValue(value);
                LogFileChange("[xmlpoke] Updated '{0}' to {1}'.",original,value);
            }
        }
    }
}