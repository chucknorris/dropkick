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
    using System.Linq;
    using Path = FileSystem.Path;

    public class XmlPokeTask :
        BaseIoTask
    {

        private string _filePath;
        private readonly IDictionary<string, string> _replacementItems;
        private readonly IDictionary<string, Tuple<string, bool>> _replaceOrInsertItems;
        private readonly IDictionary<string, string> _namespacePrefixes;

        public XmlPokeTask(string filePath, IDictionary<string, string> replacementItems, Path path)
           : this(filePath, replacementItems, new Dictionary<string, Tuple<string, bool>>(), path, new Dictionary<string, string>()) {
        }
        public XmlPokeTask(string filePath, IDictionary<string, string> replacementItems, Path path, IDictionary<string, string> namespacePrefixes)
           : this(filePath, replacementItems, new Dictionary<string, Tuple<string, bool>>(), path, namespacePrefixes) {
        }
        public XmlPokeTask(string filePath, IDictionary<string, string> replacementItems, IDictionary<string, Tuple<string, bool>> replaceOrInsertItems, Path path)
           : this(filePath, replacementItems, replaceOrInsertItems, path, new Dictionary<string, string>()) {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="replacementItems">these items will be replaced only if present in the source xml</param>
        /// <param name="replaceOrInsertItems">these items will be replace or added if not present in the source xml</param>
        /// <param name="path"></param>
        /// <param name="namespacePrefixes"></param>
        public XmlPokeTask(string filePath, IDictionary<string, string> replacementItems, IDictionary<string, Tuple<string, bool>> replaceOrInsertItems, Path path, IDictionary<string, string> namespacePrefixes)
            : base(path)
        {
            _filePath = filePath;
            _replacementItems = replacementItems;
            _replaceOrInsertItems = replaceOrInsertItems;
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

            UpdateXmlFile(result, _filePath, _replacementItems, _replaceOrInsertItems, _namespacePrefixes);

            result.AddGood(Name);
            return result;
        }

        private void UpdateXmlFile(DeploymentResult result, string filePath, IDictionary<string, string> replacementItems, IDictionary<string, Tuple<string, bool>> insertItems, IDictionary<string, string> namespacePrefixes)
        {
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

            foreach(var item in insertItems.OrEmptyListIfNull()) {
               UpdateOrInsertValueInFile(result,document, xpathNavigator, item.Key, item.Value.Item1, item.Value.Item2, nsManager);
            } 

            LogFileChange("[xmlpoke] Completed changes to '{0}'.",filePath);
            
            document.Save(filePath);
        }

        private void UpdateOrInsertValueInFile(DeploymentResult result, XmlDocument document, XPathNavigator xpathNavigator, string xPath, string value,bool shouldBeFirst, XmlNamespaceManager nsManager)
        {
           try {
              var selected = xpathNavigator.Select(xPath, nsManager);
              if(selected.Count > 0) {
                 foreach(XPathNavigator navigator in selected) {
                    string original = navigator.Value;
                    navigator.SetValue(value);
                    LogFileChange("[xmlpoke] Updated '{0}' to '{1}'.", original, value);
                 }
              } else {
                 Set(document, xPath, value, shouldBeFirst, nsManager);
                 LogFileChange("[xmlpoke] Inserting '{0}' to xPath '{1}'.", value, xPath);
              }
           }
           catch(Exception exc) {
              //to make debuggin easier...
              throw new Exception(exc.Message + " xPath: '" + xPath + "'", exc);
           }
        }

       /// <summary>
        /// original is from stackoverflow: http://stackoverflow.com/a/3465832
       /// </summary>
       /// <param name="doc"></param>
       /// <param name="xpath"></param>
       /// <param name="value"></param>
       /// <param name="nsManager"></param>
        static void Set(XmlDocument doc, string xpath, string value, bool shouldBeFirst, XmlNamespaceManager nsManager) {
           if(doc == null)
              throw new ArgumentNullException("doc");
           if(string.IsNullOrEmpty(xpath))
              throw new ArgumentNullException("xpath");

           XmlNodeList nodes = doc.SelectNodes(xpath, nsManager);
           if(nodes.Count > 1) {
              throw new XPathException("Xpath '" + xpath + "' was found multiple times!");
           } else if(nodes.Count == 0) {
              createXPath(doc, xpath, nsManager, shouldBeFirst).InnerText = value;
           } else {
              nodes[0].InnerText = value;
           }
        }
       /// <summary>
       /// 
       /// </summary>
       /// <param name="doc"></param>
       /// <param name="xpath"></param>
       /// <param name="nsManager"></param>
        /// <param name="shouldBeFirst">insert it as the first child of the parent node. Reason: @lt;configSections@gt; element must be the first in a web.config...</param>
       /// <returns></returns>
        static XmlNode createXPath(XmlDocument doc, string xpath, XmlNamespaceManager nsManager, bool shouldBeFirst) {
           XmlNode node = doc;
           //mod: trim off all leading slashes
           foreach(string part in xpath.TrimStart('/', '\\').Split('/')) {
              XmlNodeList nodes = node.SelectNodes(part, nsManager);
              if(nodes.Count > 1)
                 throw new XPathException("Xpath '" + xpath + "' was not found multiple times!");
              else if(nodes.Count == 1) { node = nodes[0]; continue; }

              if(part.StartsWith("@")) {
                 var anode = doc.CreateAttribute(part.Substring(1));
                 node.Attributes.Append(anode);
                 node = anode;
              } else {
                 string elName, attrib = null;
                 if(part.Contains("[")) {
                    SplitOnce(part, "[", out elName, out attrib);
                    if(!attrib.EndsWith("]"))
                       throw new XPathException("Unsupported XPath (missing ]): " + part);
                    attrib = attrib.Substring(0, attrib.Length - 1);
                 } else
                    elName = part;

                 if(elName.Contains(':')) {
                    //mod:if the element contains a namespace, some special handling required...
                    var split = elName.Split(':');
                    string elementName = split[1];
                    string nsShort = split[0];
                    string nsLong = nsManager.LookupNamespace(nsShort);

                    XmlNode next = doc.CreateElement(elementName, nsLong);
                    if(shouldBeFirst) {
                       node.InsertBefore(next, node.FirstChild);
                    } else { node.AppendChild(next); }
                    node = next;
                 } else {
                    XmlNode next = doc.CreateElement(elName);
                    if(shouldBeFirst) {
                       node.InsertBefore(next, node.FirstChild);
                    } else { node.AppendChild(next); }
                    node = next;
                 }

                 if(attrib != null) {
                    if(!attrib.StartsWith("@"))
                       throw new XPathException("Unsupported XPath attrib (missing @): " + part);
                    string name, value;
                    SplitOnce(attrib.Substring(1), "='", out name, out value);
                    if(string.IsNullOrEmpty(value) || !value.EndsWith("'"))
                       throw new Exception("Unsupported XPath attrib: " + part);
                    value = value.Substring(0, value.Length - 1);
                    var anode = doc.CreateAttribute(name);
                    anode.Value = value;
                    node.Attributes.Append(anode);
                 }
              }
           }
           return node;
        }

        public static void SplitOnce(string value, string separator, out string part1, out string part2) {
           if(value != null) {
              int idx = value.IndexOf(separator);
              if(idx >= 0) {
                 part1 = value.Substring(0, idx);
                 part2 = value.Substring(idx + separator.Length);
              } else {
                 part1 = value;
                 part2 = null;
              }
           } else {
              part1 = "";
              part2 = null;
           }
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