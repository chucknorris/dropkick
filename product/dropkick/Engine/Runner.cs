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
    using System;
    using System.IO;
    using Configuration;
    using Configuration.Dsl;
    using DeploymentFinders;
    using log4net;
    using Magnum.Reflection;
    using Settings;

    public static class Runner
    {
        static readonly ILog _log = LogManager.GetLogger(typeof (Runner));
        static readonly SettingsParser _parser = new SettingsParser();
        static readonly ServerMapParser _serverParser = new ServerMapParser();
        static readonly MultipleFinder _finder = new MultipleFinder();

        public static void Deploy(string commandLine)
        {
            try
            {
                DeploymentArguments newArgs = DeploymentCommandLineParser.Parse(commandLine);

                if (!File.Exists(newArgs.PathToServerMapsFile))
                {
                    _log.FatalFormat("Cannot find the server maps for the environment '{0}' at '{1}'", newArgs.Environment, newArgs.PathToServerMapsFile);
                    return;
                }

                var maps = _serverParser.Parse(new FileInfo(newArgs.PathToServerMapsFile));
                newArgs.ServerMappings.Merge(maps);

                _log.Info("*******SETTINGS*******");
                _log.InfoFormat("Command: {0}", newArgs.Command);
                _log.InfoFormat("Deployment: {0}", newArgs.Deployment);
                _log.InfoFormat("Environment: {0}", newArgs.Environment);
                _log.InfoFormat("Role: {0}", newArgs.Role);
                DisplayServerMappingsForEnvironment(newArgs.ServerMappings);
                VerifyPathToSettingsFile(newArgs.PathToSettingsFile);
                _log.Info("*******SETTINGS*******");

                Console.WriteLine("Press enter to kick it out there");
                Console.ReadKey(true);

                Deployment deployment = _finder.Find(newArgs.Deployment);
                Type settingsType = deployment.GetType().BaseType.GetGenericArguments()[1];

                var settings = (DropkickConfiguration)_parser.Parse(settingsType, new FileInfo(newArgs.PathToSettingsFile), commandLine,
                              newArgs.Environment);

                settings.Environment = newArgs.Environment;
                deployment.Initialize(settings);

                DeploymentPlanDispatcher.KickItOutThereAlready(deployment, newArgs);
            }
            catch (Exception ex)
            {
                _log.Debug(commandLine);
                _log.Error(ex);
            }
        }

        static void DisplayServerMappingsForEnvironment(RoleToServerMap mappings)
        {
            _log.Info("Server Mappings");
            foreach (var role in mappings.Roles())
            {
                _log.InfoFormat("  '{0}'", role);

                foreach (var server in mappings.GetServers(role))
                {
                    _log.InfoFormat("    '{0}'", server.Name);
                }
            }
        }

        static void VerifyPathToSettingsFile(string pathToSettingsFile)
        {
            if (File.Exists(pathToSettingsFile))
                _log.InfoFormat("Settings Path: {0}", pathToSettingsFile);
            else
                _log.ErrorFormat("Settings Path: {0}", pathToSettingsFile);
        }
    }
}