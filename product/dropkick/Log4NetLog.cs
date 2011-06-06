using log4net;

namespace dropkick
{
    public class Log4NetLog : Log
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
}