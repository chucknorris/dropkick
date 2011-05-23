using dropkick.DeploymentModel;

namespace dropkick.FileSystem
{
    public static class PathConverter
    {
        private static readonly Path _dotnetPath = new DotNetPath();

        public static string Convert(PhysicalServer server, string path)
        {
            if (server.IsLocal)
            {
                return _dotnetPath.ConvertUncShareToLocalPath(server, path);
            } else {
                return RemotePathHelper.Convert(server, path);
            }
        }
    }
}