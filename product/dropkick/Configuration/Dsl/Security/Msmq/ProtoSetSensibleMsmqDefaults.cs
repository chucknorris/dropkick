namespace dropkick.Configuration.Dsl.Security.Msmq
{
    using System;
    using DeploymentModel;
    using dropkick.Dsl.Msmq;
    using Tasks;
    using Tasks.Security.Msmq;

    public class ProtoSetSensibleMsmqDefaults : BaseProtoTask
    {
        readonly string _queue;

        public ProtoSetSensibleMsmqDefaults(string queue)
        {
            _queue = ReplaceTokens(queue);
        }

        public override void RegisterRealTasks(PhysicalServer site)
        {
            var ub = new UriBuilder("msmq", site.Name);
            ub.Path = _queue;
            site.AddTask(new SetSensibleMsmqDefaults(new QueueAddress(ub.Uri)));

        }
    }
}