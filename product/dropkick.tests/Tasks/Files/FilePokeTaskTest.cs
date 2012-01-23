using System.Collections.Generic;
using NUnit.Framework;
using dropkick.FileSystem;
using dropkick.Tasks.Files;
using Path = System.IO.Path;

namespace dropkick.tests.Tasks.Files
{
    [TestFixture]
    [Category("Integration")]
    public class FilePokeTaskTest
    {
        [Test]
        public void NameIsDescriptive()
        {
            var path = Path.GetTempFileName();
            var replacements = new List<FilePokeReplacement>();
            var task = new FilePokeTask(path, replacements, new DotNetPath());
            Assert.AreEqual(string.Format("Updated values in '{0}'", path), task.Name);
        }
    }
}