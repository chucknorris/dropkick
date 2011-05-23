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
namespace dropkick.Engine
{
    using System.Collections.Generic;
    using System.Linq;
    using Magnum.CommandLineParser;

    public static class DeploymentCommandLineParser
    {
        //TODO: Switch to new Magnum Configuration stuff
        public static DeploymentArguments Parse(string commandline)
        {
            var args = new DeploymentArguments();
            Set(args, P(commandline));
            return args;
        }

        public static void Set(DeploymentArguments arguments, IEnumerable<ICommandLineElement> commandLineElements)
        {
            string command = ExtractCommandToRun(commandLineElements);
            arguments.Command = command.ToEnum<DeploymentCommands>();

            string deployment = commandLineElements.GetDefinition("deployment", "SEARCH");
            arguments.Deployment = deployment;

            string enviro = commandLineElements.GetDefinition("environment", "LOCAL");
            arguments.Environment = enviro;

            string config = commandLineElements.GetDefinition("settings", ".\\settings");
            arguments.SettingsDirectory = config;

            string roles = commandLineElements.GetDefinition("roles", "ALL");
            arguments.Role = roles;

            bool silent = commandLineElements.GetSwitch("silent");
            arguments.Silent = silent;
        }

        public static string ExtractCommandToRun(IEnumerable<ICommandLineElement> commandLineElements)
        {
            return commandLineElements.Where(x => x is IArgumentElement)
               .Select(x => (IArgumentElement)x)
               .DefaultIfEmpty(new ArgumentElement("trace"))
               .Select(x => x.Id)
               .SingleOrDefault();
        }

        public static IEnumerable<ICommandLineElement> P(string commandLine)
        {
            var parser = new MonadicCommandLineParser();

            return parser.Parse(commandLine);
        }
    }
}