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
        string _renameTo = "notstuff.txt";
        string _conflictFile = ".\\stuff2.txt";
        string _doesntExist = ".\\noexist.txt";

        [SetUp]
        public void Setup()
        {
            File.WriteAllText(_sourceFile, "source");
            File.WriteAllText(_conflictFile, "conflict");
        }

        [TearDown]
        public void CleanUp()
        {
            
                if (File.Exists(_sourceFile))
                    File.Delete(_sourceFile);
                if (File.Exists(_renameTo))
                    File.Delete(_renameTo);
                if (File.Exists(_conflictFile))
                    File.Delete(_conflictFile);
        }

        [Test]
        public void NameIsDescriptive()
        {
            var task = new RenameTask(_sourceFile, _renameTo, new DotNetPath());
            Assert.AreEqual(@"Rename '.\stuff.txt' to 'notstuff.txt'.", task.Name);
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

        [Test]
        public void NoExist()
        {
            var task = new RenameTask(_doesntExist, _renameTo, new DotNetPath());
            var o = task.VerifyCanRun();
            Assert.IsTrue(o.ContainsError());
        }

        [Test, Explicit]
        public void TryIt()
        {
            var task = new RenameTask(@"\\srvtestwebtpg\E$\FHLB MQApps\BloombergIntegration\bin\des.exe", "hi.exe", new DotNetPath());
            task.Execute();
        }
    }
}