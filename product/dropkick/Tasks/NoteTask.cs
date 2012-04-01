namespace dropkick.Tasks
{
    using System;
    using System.Collections.Generic;
    using DeploymentModel;

    public class NoteTask :
        Task
    {
        readonly DeploymentItemStatus _status;
        readonly string _message;
        Dictionary<DeploymentItemStatus, Action<DeploymentResult>> _actions;

        public NoteTask(string message, DeploymentItemStatus status)
        {
            _message = message;
            _status = status;

            _actions = new Dictionary<DeploymentItemStatus, Action<DeploymentResult>>();
            _actions.Add(DeploymentItemStatus.Good, r=> r.AddGood(_message));
            _actions.Add(DeploymentItemStatus.Alert, r=> r.AddAlert(_message));
            _actions.Add(DeploymentItemStatus.Error, r=> r.AddError(_message));
            _actions.Add(DeploymentItemStatus.Note, r=> r.AddNote(_message));
            _actions.Add(DeploymentItemStatus.Verbose, r=> r.AddVerbose(_message));
        }

        public NoteTask(string message) : this(message, DeploymentItemStatus.Note)
        {
        }

        public string Name
        {
            get { return "NOTE: {0}".FormatWith(_message); }
        }

        public DeploymentResult VerifyCanRun()
        {
            return ReturnResult();
        }

        public DeploymentResult Execute()
        {
            return ReturnResult();
        }

        DeploymentResult ReturnResult()
        {
            var dr = new DeploymentResult();

            _actions[_status](dr);

            return dr;
        }
    }
}