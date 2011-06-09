using dropkick.DeploymentModel;

namespace dropkick.FileSystem
{
    public static class PathConverter
    {
        private static readonly Path _dotnetPath = new DotNetPath();

        /// <summary>
        /// This will convert a path string to the physical path (local server deploy will get a local address).
        /// If you need to force the path to be local always, you should call dropkick.FileSystem.Path.GetPhysicalPath(server,path,TRUE)
        /// </summary>
        /// <param name="server">Name of the server</param>
        /// <param name="path">The path on the server</param>
        /// <returns>A physical path, will be a local path if the deploy is run on the same server it is deploying to</returns>
        public static string Convert(PhysicalServer server, string path)
        {
            return _dotnetPath.GetPhysicalPath(server, path,false);
        }

        /// <summary>
        /// This will convert a path string to the physical path (local server deploy will get a local address).
        /// If you need to force the path to be local always, you should call dropkick.FileSystem.Path.GetPhysicalPath(server,path,TRUE)
        /// </summary>
        /// <param name="server">Name of the server</param>
        /// <param name="path">The path on the server</param>
        /// <returns>A physical path, will be a local path if the deploy is run on the same server it is deploying to</returns>
        public static string Convert(string server, string path)
        {
            return Convert(new DeploymentServer(server), path);
        }
    }
}