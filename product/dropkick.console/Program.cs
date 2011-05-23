// Copyright 2007-2008 The Apache Software Foundation.
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
namespace dropkick.console
{
    using System;
    using System.IO;
    using System.Linq;
    using Engine;
    using log4net;
    using log4net.Appender;
    using log4net.Config;
    using log4net.Layout;

    internal static class Program
    {
        static void Main(string[] args)
        {
            var logpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dk.log4net.xml");
            XmlConfigurator.Configure(new FileInfo(logpath));
            SetRunAppender();

            if (args.Contains("/help") || args.Contains("/?"))
            {
                var log = LogManager.GetLogger("dropkick");
                log.Info(GetHelpMessage());
                Environment.Exit(1);
            }

            Runner.Deploy(args.Aggregate("", (a, b) => a + " " + b).Trim());
        }

        private static string GetHelpMessage()
        {
            return string.Format("DropkicK Usage {0}" +
            "dk.exe [COMMAND] [ARGS]{0}" +
            "COMMAND {0}" +
            "   verify {0}" +
            "   execute {0}" +
            "   trace   (default) {0}" +
            "{0}" +
            "ARGS {0}" +
            "  /environment:local is the default - used to work with config files {0}" +
            "  /roles: all is the default {0}" +
            "  /settings: path to settings and servermaps {0}" +
            "  /deployment:  {0}" +
            "      Company.Project.Deployment.dll (an assembly) {0}" +
            "      Company.Project.Deployment.StandardDeploy (a class, lack of .dll) {0}" +
            "      (null) - if omitted, dk will search for a dll ending with 'Deployment' then pass that name in {0}" + 
            "  --silent - this switch allows you to run unattended installs {0}" 
            , Environment.NewLine);
        }

        static void SetRunAppender()
        {
            var log = LogManager.GetLogger("dropkick");
            var l = (log4net.Repository.Hierarchy.Logger)log.Logger;

            var layout = new PatternLayout
                             {
                                 ConversionPattern = "%message%newline"
                             };
            layout.ActivateOptions();

            var app = new FileAppender
                          {
                              Name = "dropkick.run.log",
                              File = string.Format("{0}.run.log", DateTime.Now.ToString("yyyyMMdd-HHmmssfff")),
                              Layout = layout,
                              AppendToFile = false
                          };
            app.ActivateOptions();

            l.AddAppender(app);
        }
    }
}