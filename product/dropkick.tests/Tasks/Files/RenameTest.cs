namespace dropkick.tests.Tasks.Files
{
    using System.IO;
    using FileSystem;
    using NUnit.Framework;
    using dropkick.Tasks.Files;


    [TestFixture]
    public class RenameTest
    {
        
        [Test]
        public void RenameFileWorks()
        {
            try
            {
                File.Create(".\\stuff.txt").Dispose();
                var task = new RenameTask(".\\stuff.txt", "notstuff.txt", new DotNetPath());
                task.Execute();

                Assert.IsTrue(File.Exists(".\\notstuff.txt"));
            }
            finally
            {
                if (File.Exists(".\\stuff.txt"))
                    File.Delete(".\\stuff.txt");
                if (File.Exists(".\\notstuff.txt"))
                    File.Delete(".\\notstuff.txt");
            }
        }

        [Test]
        public void OriginalFileNoLongerExists()
        {
            try
            {
                File.Create(".\\stuff.txt").Dispose();
                var task = new RenameTask(".\\stuff.txt", "notstuff.txt", new DotNetPath());
                task.Execute();

                Assert.IsFalse(File.Exists(".\\stuff.txt"));
            }
            finally
            {
                if (File.Exists(".\\stuff.txt"))
                    File.Delete(".\\stuff.txt");
                if (File.Exists(".\\notstuff.txt"))
                    File.Delete(".\\notstuff.txt");
            }
        }
    }
}