namespace dropkick.DeploymentModel
{
    using System;
    using System.Diagnostics;

    [DebuggerDisplay("[{Status}:{Message}")]
    public class DeploymentItem
    {
        public DeploymentItem(DeploymentItemStatus status, string message)
        {
            Status = status;
            Message = message;
        }

        public DeploymentItem(DeploymentItemStatus status, string message, Exception exception)
        {
            Status = status;
            Message = message;
            Exception = exception;
        }

        public DeploymentItemStatus Status { get; private set; }
        public string Message { get; private set; }
        public Exception Exception { get; private set; }
    }
}