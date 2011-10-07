namespace dropkick.Configuration.Dsl.Iis
{
    public static class OperationExtension
    {
        public static IisOperations Iis7(this ProtoServer protoServer)
        {
            return new IisOperations(protoServer) { Version = IisVersion.Seven };
        }
    }
}
