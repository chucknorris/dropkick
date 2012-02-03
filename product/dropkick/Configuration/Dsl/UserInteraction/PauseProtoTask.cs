namespace dropkick.Configuration.Dsl.UserInteraction
{
    using DeploymentModel;
    using Tasks;
    using Tasks.UserInteraction;

    public class PauseProtoTask : BaseProtoTask
    {
        public PauseProtoTask(string message)
        {
            Message = ReplaceTokens(message);
        }

        public string Message { get; private set; }

        public override void RegisterRealTasks(PhysicalServer s)
        {
            s.AddTask(new PauseTask(Message));
        }
    }
}