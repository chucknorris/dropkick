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
    public interface XmlPokeOptions
    {
       /// <summary>
       /// set the value at the given xPath; items will be replaced only if present in the source xml
       /// </summary>
       /// <param name="xPath"></param>
       /// <param name="value"></param>
       /// <returns></returns>
       XmlPokeOptions Set(string xPath, string value);
       /// <summary>
       /// set the value, or create new nodes according to the xPath; items will be replace or added if not present in the source xml
       /// </summary>
       /// <param name="xPath"></param>
       /// <param name="value"></param>
       /// <returns></returns>
       XmlPokeOptions SetOrInsert(string xPath, string value);

       /// <summary>
       /// set the value, or create new nodes according to the xPath; items will be replace or added if not present in the source xml;
       /// if shouldBeFirst is set to true, the inserted item will be inserted before the first node.
       /// </summary>
       /// <param name="xPath"></param>
       /// <param name="value"></param>
       /// <param name="shouldBeFirst"></param>
       /// <returns></returns>
       XmlPokeOptions SetOrInsert(string xPath, string value, bool shouldBeFirst);
    }
}