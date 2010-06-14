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
        readonly DotNetPath _path = new DotNetPath();
        //const string BaseDir = @".\CopyFileTests";
        const string BaseDir = @"\\srvtestweb01\e$\CopyFileTests";
        readonly string _sourceDirectory = @"{0}\source".FormatWith(BaseDir);
        readonly string _destinationDirectory = @"{0}\dest".FormatWith(BaseDir);

        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            if (Directory.Exists(BaseDir))
                Directory.Delete(BaseDir, true);

            Directory.CreateDirectory(BaseDir);
            Directory.CreateDirectory(_sourceDirectory);
            File.WriteAllLines(_path.Combine(_sourceDirectory, "test.txt"), new[] { "the test" });
            Directory.CreateDirectory(_destinationDirectory);
        }

        [TearDown]
        public void TearDown()
        {
            Directory.Delete(BaseDir,true);
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
        public void CopyFileToFileWithSameName()
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

        [Test]
        public void CopyFileToFileWithSameNameAndOverwrite()
        {
            File.WriteAllLines(_path.Combine(_destinationDirectory, "test.txt"), new[] { "bad file" });

            var file = _path.Combine(_sourceDirectory, "test.txt");
            var t = new CopyFileTask(file, _destinationDirectory, "test.txt", new DotNetPath());
            t.Execute();

            var s = File.ReadAllText(_path.Combine(_destinationDirectory, "test.txt"));
            Assert.AreEqual("the test\r\n", s);
        }

    }
}