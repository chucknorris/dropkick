using System.IO;
using dropkick.FileSystem;
using NUnit.Framework;

namespace dropkick.tests.Tasks.Files
{
    [TestFixture]
    public class DotNetPathTests
    {
        [Test]
        public void DirectoryExists()
        {
            var p = new DotNetPath();
            try
            {
                Directory.CreateDirectory(".\\stuff");
                Assert.IsTrue(p.IsDirectory(".\\stuff"));
            }
            finally
            {
                if(Directory.Exists(".\\stuff"))
                   Directory.Delete(".\\stuff");
            }
        }

        [Test]
        public void IsDirectoryReturnsFalseForAFile()
        {
            var p = new DotNetPath();
            try
            {
                File.Create(".\\stuff.txt").Dispose();
                Assert.IsFalse(p.IsDirectory(".\\stuff.txt"));
            }
            finally
            {
                if (File.Exists(".\\stuff.txt"))
                    File.Delete(".\\stuff.txt");
            }
            
        }

        [Test]
        public void FileExists()
        {
            var p = new DotNetPath();
            try
            {
                File.Create(".\\stuff.txt").Dispose();
                Assert.IsTrue(p.IsFile(".\\stuff.txt"));
            }
            finally
            {
                if (File.Exists(".\\stuff.txt"))
                    File.Delete(".\\stuff.txt");
            }
        }

        [Test]
        public void IsFileReturnsFalseForADirecroty()
        {
            var p = new DotNetPath();
            try
            {
                Directory.CreateDirectory(".\\stuff");
                Assert.IsFalse (p.IsFile(".\\stuff"));
            }
            finally
            {
                if (Directory.Exists(".\\stuff"))
                    Directory.Delete(".\\stuff");
            }
        }
    }
}