using dropkick.FileSystem;

namespace dropkick.tests
{
    using System.IO;
    using System.Reflection;
    using dropkick.DeploymentModel;
    using NUnit.Framework;

    [TestFixture]
    public class RemotePathHelperTest
    {
        [Test, Explicit]
        public void FHLBApp()
        {
            var path = new DotNetPath();
            var x = path.ConvertUncShareToLocalPath(new DeploymentServer("sellersd"), @"~\FHLBWinSvc\Bill");
            Assert.AreEqual("D:\\Development\\cue_dep\\Shares\\FHLBWinSvc\\Bill", x);
        }

        [Test]
        public void NAME()
        {
            var server = "SrvTopeka01";
            var path = "E:\\bob\\bill";
            var expected = @"\\SrvTopeka01\E$\bob\bill";
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