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
namespace dropkick.Tasks
{
    using Configuration.Dsl;
    using DeploymentModel;
    using StringInterpolation;

    public abstract class BaseProtoTask :
        ProtoTask
    {
        readonly Interpolator _interpolator = new CaseInsensitiveInterpolator();

        public void InspectWith(DeploymentInspector inspector)
        {
            inspector.Inspect(this);
        }

        public abstract void RegisterRealTasks(PhysicalServer site);

        public string ReplaceTokens(string input)
        {
            return _interpolator.ReplaceTokens(HUB.Settings, input);
        }
    }
}