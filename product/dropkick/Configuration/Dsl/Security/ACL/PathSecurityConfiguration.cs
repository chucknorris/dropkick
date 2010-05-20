namespace dropkick.Configuration.Dsl.Security
{
    public class PathSecurityConfiguration :
        FileSecurityConfig
    {
        readonly string _path;
        readonly ProtoServer _server;

        public PathSecurityConfiguration(ProtoServer server, string path)
        {
            _server = server;
            _path = path;
        }

        #region FileSecurityConfig Members

        public void Clear()
        {
            var proto = new ProtoPathClearTask(_path);
            _server.RegisterProtoTask(proto);
        }

        public void GrantRead(string group)
        {
            var proto = new ProtoPathGrantReadTask(_path, group);
            _server.RegisterProtoTask(proto);
        }

        public void GrantReadWrite(string group)
        {
            var proto = new ProtoPathGrantReadWriteTask(_path, group);
            _server.RegisterProtoTask(proto);
        }

        #endregion
    }
}