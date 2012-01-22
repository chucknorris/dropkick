using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using dropkick.Tasks.Files;

namespace dropkick.tests.Tasks.Files
{
    [TestFixture]
    [Category("Integration")]
    public class FilePokeTaskTest
    {
        private string _filePath;
        private IList<FilePokeReplacement> _replacementItems;
        private FilePokeTask _task;

        [SetUp]
        public void Setup()
        {
            _filePath = Path.GetTempFileName();

            _replacementItems = new List<FilePokeReplacement>();
            _task = new FilePokeTask(_filePath, _replacementItems);
        }

        [TearDown]
        public void CleanUp()
        {
            if (File.Exists(_filePath))
                File.Delete(_filePath);
        }

        [Test]
        public void NameIsDescriptive()
        {
            Assert.AreEqual(string.Format("Updated values in '{0}'", _filePath), _task.Name);
        }
    }
}