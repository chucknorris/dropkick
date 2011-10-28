namespace dropkick.Configuration.Dsl.Iis
{
    public class IisOperations
    {
        readonly ProtoServer _protoServer;

        public IisOperations(ProtoServer protoServer)
        {
            _protoServer = protoServer;
        }

        public IisVersion Version { get; set; }

        public IisApplicationPoolOperation ApplicationPool(string applicationPoolName)
        {
            var task = new IisApplicationPoolOperationProtoTask(applicationPoolName) { Version = Version };
            _protoServer.RegisterProtoTask(task);
            return task;
        }
    }

    public interface IisApplicationPoolOperation
    {
        void Start();
        void Stop();
    }
}
