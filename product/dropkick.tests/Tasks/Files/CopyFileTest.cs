namespace dropkick.tests.Tasks.Files
{
    using System.IO;
    using dropkick.Tasks.Files;
    using FileSystem;
    using NUnit.Framework;
    using Path = FileSystem.Path;

    [TestFixture]
    public class CopyFileTest
    {
        DotNetPath _path = new DotNetPath();
        string _baseDir = @".\CopyFileTests";
        string _sourceDirectory = @".\CopyFileTests\source";
        string _destinationDirectory = @".\CopyFileTests\dest";

        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            if (Directory.Exists(_baseDir))
                Directory.Delete(_baseDir, true);

            Directory.CreateDirectory(_baseDir);
            Directory.CreateDirectory(_sourceDirectory);
            File.WriteAllLines(_path.Combine(_sourceDirectory, "test.txt"), new[] { "the test" });
            Directory.CreateDirectory(_destinationDirectory);
        }

        [TearDown]
        public void TearDown()
        {
            Directory.Delete(_baseDir,true);
        }

        #endregion



        [Test]
        public void CopyFileToDirectory()
        {
            var file = _path.Combine(_sourceDirectory, "test.txt");
            var t = new CopyFileTask(file, _destinationDirectory, null, new DotNetPath());
            t.Execute();

            var s = File.ReadAllText(_path.Combine(_destinationDirectory, "test.txt"));
            Assert.AreEqual("the test\r\n", s);
        }

        [Test]
        public void CopyFileToFile()
        {
            var file = _path.Combine(_sourceDirectory, "test.txt");
            var t = new CopyFileTask(file, _destinationDirectory, "test.txt", new DotNetPath());
            t.Execute();

            var s = File.ReadAllText(_path.Combine(_destinationDirectory, "test.txt"));
            Assert.AreEqual("the test\r\n", s);
        }

        [Test]
        public void CopyFileToFileWithRename()
        {
            var file = _path.Combine(_sourceDirectory, "test.txt");
            var t = new CopyFileTask(file, _destinationDirectory, "newtest.txt", new DotNetPath());
            t.Execute();

            var s = File.ReadAllText(_path.Combine(_destinationDirectory, "newtest.txt"));
            Assert.AreEqual("the test\r\n", s);
        }

    }
}