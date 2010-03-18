namespace dropkick.Configuration.Dsl.Notes
{
    using Tasks;

    public static class Extension
    {
        public static void Note(this Server server, string note)
        {
            var proto = new NoteProtoTask(note);
            server.RegisterTask(proto);
        }
    }
}