namespace dropkick.tests.Tasks.Files
{
    using System.IO;
    using FileSystem;
    using NUnit.Framework;
    using dropkick.Tasks.Files;


    [TestFixture]
    public class RenameTest
    {
        string _sourceFile = ".\\stuff.txt";
        string _renameTo = ".\\notstuff.txt";
        string _conflictFile = ".\\stuff2.txt";

        [SetUp]
        public void Setup()
        {
            File.WriteAllText(_sourceFile, "source");
            File.WriteAllText(_conflictFile, "conflict");
        }

        [TearDown]
        public void CleanUp()
        {
            
//                if (File.Exists(_sourceFile))
//                    File.Delete(_sourceFile);
//                if (File.Exists(_renameTo))
//                    File.Delete(_renameTo);
//                if (File.Exists(_conflictFile))
//                    File.Delete(_conflictFile);
        }

        [Test]
        public void RenameFileWorks()
        {
           
                var task = new RenameTask(_sourceFile, _renameTo, new DotNetPath());
                task.Execute();

                Assert.IsTrue(File.Exists(_renameTo));
           
        }

        [Test]
        public void OriginalFileNoLongerExists()
        {
            var task = new RenameTask(_sourceFile, _renameTo, new DotNetPath());
            task.Execute();

            Assert.IsFalse(File.Exists(_sourceFile));
        }

        [Test]
        public void RenameOnTopOfConflictingFile()
        {
            var task = new RenameTask(_sourceFile, _conflictFile, new DotNetPath());
            task.Execute();
        }
    }
}