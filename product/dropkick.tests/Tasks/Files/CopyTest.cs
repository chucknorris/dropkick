namespace dropkick.tests.Tasks.Files
{
    using System.IO;
    using dropkick.Tasks.Files;
    using NUnit.Framework;

    [TestFixture]
    public class CopyTest
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            if (Directory.Exists(_path))
                Directory.Delete(_path, true);

            Directory.CreateDirectory(_path);
            Directory.CreateDirectory(_source);
            File.WriteAllLines(Path.Combine(_source, "test.txt"), new[] {"the test"});
            Directory.CreateDirectory(_dest);
        }

        [TearDown]
        public void TearDown()
        {
        }

        #endregion

        string _path = ".\\test";
        string _source = ".\\test\\source";
        string _dest = ".\\test\\dest";

        [Test]
        public void Copy()
        {
            var t = new CopyTask(_source, _dest);
            t.Execute();

            string s = File.ReadAllText(Path.Combine(_dest, "test.txt"));
            Assert.AreEqual("the test\r\n", s);
        }
    }
}