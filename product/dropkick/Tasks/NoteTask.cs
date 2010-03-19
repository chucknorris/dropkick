namespace dropkick.Tasks
{
    using DeploymentModel;

    public class NoteTask :
        Task
    {
        readonly string _message;

        public NoteTask(string message)
        {
            _message = message;
        }

        public string Name
        {
            get { return "NOTE: {0}".FormatWith(_message); }
        }

        public DeploymentResult VerifyCanRun()
        {
            return new DeploymentResult();
        }

        public DeploymentResult Execute()
        {
            //do nothing
            return new DeploymentResult();
        }
    }
}