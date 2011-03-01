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
namespace dropkick
{
    using System;
    using log4net;

    internal class Lognames
    {
        public const string COARSE = "dropkick.coarsegrain";
        public const string FINE = "dropkick.finegrain";
        public const string FILE = "dropkick.changes.file";
        public const string SECURITY = "dropkick.changes.security";
    }

    public static class Logging
    {
        static readonly ILog _fineLog = LogManager.GetLogger(Lognames.FINE);
        static readonly ILog _coarseLog = LogManager.GetLogger(Lognames.COARSE);
        static readonly KnownLoggers _knownLoggers = new KnownLoggers();

        public static void Fine(string format, params object[] args)
        {
            _fineLog.DebugFormat(format, args);
        }

        public static void Coarse(string format, params object[] args)
        {
            _coarseLog.InfoFormat(format, args);
        }

        public static void Warn(string format, params object[] args)
        {
            _coarseLog.WarnFormat(format, args);
        }

        public static void Error(Exception ex, string format, params object[] args)
        {
            _coarseLog.Error(format.FormatWith(args),ex);
        }

        public static KnownLoggers WellKnown
        {
            get { return _knownLoggers; }
        }
    }

    public class KnownLoggers
    {
        readonly ILog _fileLog = LogManager.GetLogger(Lognames.FILE);
        readonly ILog _securityLog = LogManager.GetLogger(Lognames.SECURITY);

        public ILog FileChanges
        {
            get { return _fileLog; }
        }

        public ILog SecurityChanges
        {
            get { return _securityLog; }
        }

        public void LogSecurityChange(Action<Log> action)
        {
            //[%properties{section}]
            using (ThreadContext.Stacks["section"].Push("security"))
            {
                action(null);
            }
        }
    }

    public class Log4NetLog :
        Log
    {
        ILog _log;

        public Log4NetLog(ILog log)
        {
            _log = log;
        }

        public void Note(string format, object[] args)
        {
            _log.InfoFormat(format, args);
        }
    }

    public interface Log
    {
        void Note(string format, object[] args);
    }
}