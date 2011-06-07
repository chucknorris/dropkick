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
using log4net.Appender;
using log4net.Core;
using log4net.Layout;

namespace dropkick
{
    using System;
    using log4net;

    public enum LogLevel
    {
        Debug,
        Info,
        Warn,
        Error,
        Fatal,
    }

    internal class LogNames
    {
        public const string COARSE = "dropkick.coarsegrain";
        public const string FINE = "dropkick.finegrain";
        public const string FILE = "dropkick.changes.file";
        public const string SECURITY = "dropkick.changes.security";
        public const string DATABASE = "dropkick.changes.database";
    }

    public static class Logging
    {
        static readonly ILog _fineLog = LogManager.GetLogger(LogNames.FINE);
        static readonly ILog _coarseLog = LogManager.GetLogger(LogNames.COARSE);
        static readonly KnownLoggers _knownLoggers = new KnownLoggers();

        /// <summary>
        /// This is the base logger
        /// </summary>
        /// <param name="logger">The logger to log to</param>
        /// <param name="level">The Log Level</param>
        /// <param name="ex">This is the exception, pass in null if there is no exception</param>
        /// <param name="format">This is the message with formatting items in it</param>
        /// <param name="args">These are the arguments that are used in formatting</param>
        public static void LogAMessage(ILog logger, LogLevel level, Exception ex, string format, params object[] args)
        {
            Level logLevel = GetLevel(level);
            logger.Logger.Log(typeof(Logging), logLevel, format.FormatWith(args), ex);
        }

        /// <remarks>Yuck</remarks>
        private static Level GetLevel(LogLevel level)
        {
            Level returnValue = Level.Debug;

            switch (level)
            {
                case LogLevel.Debug:
                    returnValue = Level.Debug;
                    break;
                case LogLevel.Info:
                    returnValue = Level.Info;
                    break;
                case LogLevel.Warn:
                    returnValue = Level.Warn;
                    break;
                case LogLevel.Error:
                    returnValue = Level.Error;
                    break;
                case LogLevel.Fatal:
                    returnValue = Level.Fatal;
                    break;
            }

            return returnValue;
        }

        public static void Fine(string format, params object[] args)
        {
            Fine(LogLevel.Debug, format, args);
        }

        public static void Fine(LogLevel level, string format, params object[] args)
        {
            LogAMessage(_fineLog, level, null, format, args);
        }

        public static void Coarse(string format, params object[] args)
        {
            Coarse(LogLevel.Info, format, args);
        }

        public static void Coarse(LogLevel level, string format, params object[] args)
        {
            LogAMessage(_coarseLog, level, null, format, args);
        }

        public static void Warn(string format, params object[] args)
        {
            LogAMessage(_coarseLog, LogLevel.Warn, null, format, args);
        }

        public static void Error(Exception ex, string format, params object[] args)
        {
            LogAMessage(_coarseLog, LogLevel.Error, ex, format, args);
        }

        public static KnownLoggers WellKnown
        {
            get { return _knownLoggers; }
        }

        private static bool _alreadySetRunAppender = false;

        public static void SetRunAppender()
        {
            if (!_alreadySetRunAppender)
            {
                _alreadySetRunAppender = true;

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

    public class KnownLoggers
    {
        readonly ILog _fileLog = LogManager.GetLogger(LogNames.FILE);
        readonly ILog _securityLog = LogManager.GetLogger(LogNames.SECURITY);
        readonly ILog _databaseLog = LogManager.GetLogger(LogNames.DATABASE);

        public ILog FileChanges
        {
            get { return _fileLog; }
        }

        public ILog SecurityChanges
        {
            get { return _securityLog; }
        }

        public ILog DatabaseChanges
        {
            get { return _databaseLog; }
        }
    }



}