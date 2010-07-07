using System.IO;
using dropkick.FileSystem;
using NUnit.Framework;

namespace dropkick.tests.Tasks.Files
{
    [TestFixture]
    public class DotNetPathTests
    {
        DotNetPath _path;

        [SetUp]
        public void Setup()
        {
            _path = new DotNetPath();
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(".\\stuff"))
                Directory.Delete(".\\stuff");
        }

        [Test]
        public void DirectoryExists()
        {
            Directory.CreateDirectory(".\\stuff");
            Assert.IsTrue(_path.IsDirectory(".\\stuff"));
        }

        [Test]
        public void IsDirectoryReturnsFalseForAFile()
        {
            File.Create(".\\stuff.txt").Dispose();
            Assert.IsFalse(_path.IsDirectory(".\\stuff.txt"));
        }

        [Test]
        public void FileExists()
        {
            File.Create(".\\stuff.txt").Dispose();
            Assert.IsTrue(_path.IsFile(".\\stuff.txt"));
        }

        [Test, Explicit]
        public void RemotepathIsFileTest()
        {
            Assert.IsTrue(_path.IsFile(@"\\srvtestwebtpg\E$\FHLB MQApps\BloombergIntegration\bin\des.exe"));
        }

        [Test]
        public void IsFileReturnsFalseForADirecroty()
        {
            Directory.CreateDirectory(".\\stuff");
            Assert.IsFalse(_path.IsFile(".\\stuff"));
        }

        [Test]
        public void RemoteFileTest()
        {
            Assert.IsTrue(File.Exists(@"\\srvtestwebtpg\E$\FHLB MQApps\BloombergIntegration\bin\FHLBank.BloombergIntegration.Host.exe.config"));
        }
    }
}