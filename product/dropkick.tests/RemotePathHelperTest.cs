using dropkick.FileSystem;

namespace dropkick.tests
{
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
    }
}