namespace dropkick.Tasks
{
    using System;
    using DeploymentModel;

    public class NoteProtoTask :
        BaseTask
    {
        public string Message { get; set; }

        public NoteProtoTask(string message)
        {
            Message = message;
        }

        public override Action<TaskSite> RegisterTasks()
        {
            return s =>
                   {
                       s.AddTask(new NoteTask(Message));
                   };
        }
    }
}