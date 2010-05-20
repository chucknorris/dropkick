namespace dropkick.Configuration.Dsl.RoundhousE
{
    public static class Extension
    {
        public static RoundhousEOptions RoundhousE(this ProtoServer protoServer)
        {
            var proto = new RoundhousEProtoTask();
            protoServer.RegisterProtoTask(proto);
            return proto;
        }
    }
}