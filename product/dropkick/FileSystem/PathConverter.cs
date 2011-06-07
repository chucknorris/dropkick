using dropkick.DeploymentModel;

namespace dropkick.FileSystem
{
    public static class PathConverter
    {
        private static readonly Path _dotnetPath = new DotNetPath();

        public static string Convert(PhysicalServer server, string path)
        {
            return _dotnetPath.GetPhysicalPath(server, path);
        }

        public static string Convert(string server, string path)
        {
            return Convert(new DeploymentServer(server), path);
        }
    }
}