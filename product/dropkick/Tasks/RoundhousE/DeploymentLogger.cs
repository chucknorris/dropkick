using System;
using dropkick.DeploymentModel;
using roundhouse.infrastructure.logging;

namespace dropkick.Tasks.RoundhousE
{
    public class DeploymentLogger : Logger
    {
        private readonly DeploymentResult _results;

        public DeploymentLogger(DeploymentResult results)
        {
            _results = results;
        }
        public void log_a_debug_event_containing(string message, params object[] args)
        {
           // _results.AddNote(message, args);
        }

        public void log_an_info_event_containing(string message, params object[] args)
        {
            _results.AddNote(message, args);
        }

        public void log_a_warning_event_containing(string message, params object[] args)
        {
            _results.AddAlert(message, args);
        }

        public void log_an_error_event_containing(string message, params object[] args)
        {
            _results.AddError(message.FormatWith(args));
        }

        public void log_a_fatal_event_containing(string message, params object[] args)
        {
            _results.AddError(message.FormatWith(args));
        }

        public object underlying_type
        {
            get { return _results; }
        }
    }
}