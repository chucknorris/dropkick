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
    using Settings;

    public static class Runner
    {
        static readonly ILog _coarseLog = LogManager.GetLogger("dropkick.coarsegrain");

        static readonly SettingsParser _parser = new SettingsParser();
        static readonly ServerMapParser _serverParser = new ServerMapParser();
        static readonly MultipleFinder _finder = new MultipleFinder();

        public static void Deploy(string commandLine)
        {
            if (!_coarseLog.IsDebugEnabled) { Console.WriteLine("Sad Emo Otter says \"DEBUG LOGGING IS OFF - THIS ISN'T GOING TO BE FUN :(\""); }

            try
            {
                _coarseLog.Info("****************************************************");
                _coarseLog.Info("DropkicK");
                _coarseLog.Info("****************************************************");
                _coarseLog.Info("");

                DeploymentArguments newArgs = DeploymentCommandLineParser.Parse(commandLine);
                bool silent = newArgs.Silent;
                bool argumentsVerified = true;

                _coarseLog.Info("^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^");
                _coarseLog.InfoFormat("Command:     {0}", newArgs.Command);
                _coarseLog.InfoFormat("Environment: {0}", newArgs.Environment);
                _coarseLog.InfoFormat("Role:        {0}", newArgs.Role);

                ////////// File Checks
                if (!VerifyPathToServerMapsFile(newArgs.PathToServerMapsFile))
                {
                    if (argumentsVerified) argumentsVerified = false;
                }
                if (!VerifyPathToSettingsFile(newArgs.PathToSettingsFile))
                {
                    if (argumentsVerified) argumentsVerified = false;
                }
                ////////////////////

                //////// DEPLOYMENT STUFF
                FindResult findResult = _finder.Find(newArgs.Deployment);
                Deployment deployment = findResult.Deployment;
                _coarseLog.InfoFormat("Deployment Method: '{0}'", findResult.MethodOfFinding);
                _coarseLog.InfoFormat("Deployment Found:  '{0}'", findResult.Deployment.GetType().Name);

                if (deployment.GetType().Equals(typeof(NullDeployment)))
                {
                    _coarseLog.Fatal("Couldn't find a deployment to run.");
                    if (argumentsVerified) argumentsVerified = false;
                }
                ////////

                if (!argumentsVerified)
                {
                    Environment.Exit(1);
                }

                RoleToServerMap maps = _serverParser.Parse(new FileInfo(newArgs.PathToServerMapsFile));
                newArgs.ServerMappings.Merge(maps);
                DisplayServerMappingsForEnvironment(newArgs.ServerMappings);

                _coarseLog.Info("");
                _coarseLog.Info("^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^");

                if (deployment.HardPrompt)
                {

                    if (!silent)
                    {
                        bool wrong = true;
                        do
                        {
                            _coarseLog.Info("  Please type the environment name '{0}' to continue.".FormatWith(newArgs.Environment));
                            var environment = Console.ReadLine();
                            if (environment.EqualsIgnoreCase(newArgs.Environment))
                            {
                                wrong = false;
                            }
                        } while (wrong);
                    }
                    else
                    {
                        _coarseLog.Fatal("Cannot use hard prompting when in silent mode.");
                        Environment.Exit(1);
                    }
                }
                else
                {
                    if (!silent)
                    {
                        _coarseLog.Info("Please review the settings above and when you are ready,");
                        _coarseLog.Info("  Press 'ctrl+c' to cancel.");
                        _coarseLog.Info("  Press enter to kick it out there");
                        Console.ReadKey(true);
                    }
                }

                /////// how to clean this up - below 
                Type settingsType = deployment.GetType().BaseType.GetGenericArguments()[1];

                var settings = (DropkickConfiguration)_parser.Parse(settingsType, new FileInfo(newArgs.PathToSettingsFile), commandLine,newArgs.Environment);

                settings.Environment = newArgs.Environment;
                deployment.Initialize(settings);

                DeploymentPlanDispatcher.KickItOutThereAlready(deployment, newArgs);
            }
            catch (Exception ex)
            {
                _coarseLog.Debug(commandLine);
                _coarseLog.Error(ex);
                Environment.Exit(1);
            }
        }

        static void DisplayServerMappingsForEnvironment(RoleToServerMap mappings)
        {
            _coarseLog.Debug("");
            _coarseLog.Info("Server Mappings");
            foreach (var role in mappings.Roles())
            {
                _coarseLog.InfoFormat("  '{0}'", role);

                foreach (var server in mappings.GetServers(role))
                {
                    _coarseLog.InfoFormat("    '{0}'", server.Name);
                }
            }
        }

        static bool VerifyPathToServerMapsFile(string pathToFile)
        {
            if (File.Exists(pathToFile))
            {
                _coarseLog.Debug("");
                _coarseLog.InfoFormat("Server Maps:   '{0}' - Looks Good!", pathToFile);
                return true;
            }

            _coarseLog.FatalFormat("Server Maps:   '{0}' - NOT FOUND", pathToFile);
            return false;
        }

        static bool VerifyPathToSettingsFile(string pathToSettingsFile)
        {
            if (File.Exists(pathToSettingsFile))
            {
                _coarseLog.Debug("");
                _coarseLog.InfoFormat("Settings Path: '{0}' - Looks Good!", pathToSettingsFile);
                return true;
            }

            _coarseLog.FatalFormat("Settings Path: '{0}' - NOT FOUND", pathToSettingsFile);
            return false;
        }
    }
}