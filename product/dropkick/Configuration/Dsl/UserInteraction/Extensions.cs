namespace dropkick.Configuration.Dsl.UserInteraction
{
    public static class Extension
    {
        public static void Pause(this ProtoServer server, string messageToDisplay)
        {
            server.RegisterProtoTask(new PauseProtoTask(messageToDisplay));
        }
    }
}