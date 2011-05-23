using dropkick.FileSystem;
using dropkick.Tasks.NetworkShare;
using dropkick.Wmi;

namespace dropkick.tests
{
    using System.IO;
    using System.Reflection;
    using dropkick.DeploymentModel;
    using NUnit.Framework;

    [TestFixture]
    public class RemotePathHelperTest
    {
        [Test]
        [Category("Integration")]
        public void when_deploying_to_c_share_should_convert_remote_path_successfully()
        {
            var path = new DotNetPath();
            var x = path.ConvertUncShareToLocalPath(new DeploymentServer("127.0.0.1"), @"~\c$\Bill");
            Assert.AreEqual("C:\\Bill", x);
        }

        [Test]
        [Category("Integration")]
        public void when_deploying_to_temp_share_should_convert_remote_path_successfully()
        {
            var path = new DotNetPath();
            
            string serverName = "127.0.0.1";
            string shareName = "yoyotemp";
            string destination = @".\temp";
            string sub_folder = "Bill";

            if (!Directory.Exists(destination)) {Directory.CreateDirectory(destination);}

            Win32Share.Create(serverName, shareName, destination, "yoyo");
            var x = path.ConvertUncShareToLocalPath(new DeploymentServer(serverName), @"~\{0}\{1}".FormatWith(shareName,sub_folder));
            Win32Share.Delete(serverName, shareName);

            Assert.AreEqual(Path.GetFullPath(Path.Combine(destination,sub_folder)) , x);
        }

        [Test]
        public void when_deploying_to_c_colon_should_convert_to_share_successfully()
        {
            var server = "127.0.0.1";
            var path = "C:\\bob\\bill";
            var expected = @"\\127.0.0.1\C$\bob\bill";
            var actual = RemotePathHelper.Convert(server, path);
            Assert.AreEqual(expected, actual);

        }

        [Test]
        public void StringFormatTest()
        {
            var expected = "1234 ";
            var actual = string.Format("{0,-5}", "1234");
            Assert.AreEqual(expected, actual);
        }

        //[Test]
        //public void CreateADirectoryThatDoesNotExistRemotely() {

        //    var p = @"\\192.168.2.109\C$\Temp\dropkick.remote";
        //    var assembly = Assembly.GetExecutingAssembly();
        //    var assemblyName = assembly.GetName().Name + ".dll";
        //    if (!Directory.Exists(p)) Directory.CreateDirectory(p);
        //    File.Copy(Assembly.GetExecutingAssembly().Location, Path.Combine(p,assemblyName));
        //    //
        //   // p = System.IO.Path.Combine(p, "dropkick.remote.exe");

        //  //  File.Copy(@".\dropkick.remote.exe", p);
        //}
    }
}