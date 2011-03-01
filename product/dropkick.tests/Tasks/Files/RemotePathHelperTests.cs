namespace dropkick.tests.Tasks.Files
{
    using FileSystem;
    using NUnit.Framework;

    [TestFixture]
    public class RemotePathHelperTests
    {
        
        [Test][Explicit]
        public void TildaUncPath()
        {
            var path = RemotePathHelper.Convert("SrvTopeka02", @"~\bob\bill");
            Assert.AreEqual(@"\\SrvTopeka02\bob\bill", path);
        }

        [Test][Explicit]
        public void Bob()
        {
            var path = RemotePathHelper.Convert("SrvTopeka02", @"E:\bob\bill");
            Assert.AreEqual(@"\\SrvTopeka02\E$\bob\bill", path);
        }
    }
}