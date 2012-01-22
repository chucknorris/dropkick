using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;
using dropkick.Configuration;
using dropkick.Configuration.Dsl.Files;
using dropkick.DeploymentModel;

namespace dropkick.tests.Configuration.Dsl
{
    [TestFixture]
    [Category("Integration")]
    public class FilePokeTest
    {
        private TestProtoServer _protoServer;
        private string _filePath;
        private TestPhysicalServer _physicalServer;

        [SetUp]
        public void SetUp()
        {
            _protoServer = new TestProtoServer();
            _filePath = Path.GetTempFileName();
            HUB.Settings = new object();
        }

        [TearDown]
        public void CleanUp()
        {
            if (File.Exists(_filePath))
                File.Delete(_filePath);
        }

        private void SetFileContents(string fileContents)
        {
            File.WriteAllText(_filePath, fileContents);
        }

        private DeploymentResult VerifyTask()
        {
            _physicalServer = new TestPhysicalServer();
            _protoServer.ProtoTask.RegisterRealTasks(_physicalServer);
            return _physicalServer.Task.VerifyCanRun();
        }

        private DeploymentResult ExecuteTask()
        {
            _physicalServer = new TestPhysicalServer();
            _protoServer.ProtoTask.RegisterRealTasks(_physicalServer);
            return _physicalServer.Task.Execute();
        }

        private void AssertFileContents(string expected)
        {
            Assert.AreEqual(expected, File.ReadAllText(_filePath));
        }

        private void AssertFilePathIs(string filePath)
        {
            _physicalServer = new TestPhysicalServer();
            _protoServer.ProtoTask.RegisterRealTasks(_physicalServer);
            StringAssert.Contains(filePath, _physicalServer.Task.Name);
        }

        [Test]
        public void VerificationFailsIfFileNotFound()
        {
            var bogusFileName = Guid.NewGuid().ToString();

            _protoServer.FilePoke(bogusFileName);
            var result = VerifyTask();

            Assert.IsTrue(result.ContainsError());
        }

        [Test]
        public void VerificationSucceedsIfFileFound()
        {
            _protoServer.FilePoke(_filePath);
            var result = VerifyTask();

            Assert.IsFalse(result.ContainsError());
            Assert.AreEqual(1, result.Results.Count(x => x.Status == DeploymentItemStatus.Good));
        }

        [Test]
        public void ExecutionFailsIfFileNotFound()
        {
            var bogusFileName = Guid.NewGuid().ToString();

            _protoServer.FilePoke(bogusFileName);
            var result = ExecuteTask();

            Assert.IsTrue(result.ContainsError());
        }

        [Test]
        public void ExecutionSucceedsIfFileFound()
        {
            _protoServer.FilePoke(_filePath);
            var result = ExecuteTask();

            Assert.IsFalse(result.ContainsError());
            Assert.AreEqual(1, result.Results.Count(x => x.Status == DeploymentItemStatus.Good));
        }

        [Test]
        public void ReplaceOneItem()
        {
            SetFileContents("abcd");

            _protoServer.FilePoke(_filePath)
                .Replace("a", "x");
            ExecuteTask();

            AssertFileContents("xbcd");
        }

        [Test]
        public void ReplaceTwoItems()
        {
            SetFileContents("abcd");

            _protoServer.FilePoke(_filePath)
                .Replace("a", "x")
                .Replace("b", "y");
            ExecuteTask();

            AssertFileContents("xycd");
        }

        [Test]
        public void MultipleMatchesReplaced()
        {
            SetFileContents("abab");

            _protoServer.FilePoke(_filePath)
                .Replace("a", "x");
            ExecuteTask();

            AssertFileContents("xbxb");
        }

        [Test]
        public void SupportsRegex()
        {
            SetFileContents("abcd");

            _protoServer.FilePoke(_filePath)
                .Replace(@"[a-c]", "x");
            ExecuteTask();

            AssertFileContents("xxxd");
        }

        [Test]
        public void AllowsForRegexOptions()
        {
            SetFileContents("abcd");

            _protoServer.FilePoke(_filePath)
                .Replace("A", "b", RegexOptions.IgnoreCase);
            ExecuteTask();

            AssertFileContents("bbcd");
        }

        [Test]
        public void AllowsForEncoding()
        {
            File.WriteAllText(_filePath, "abcd", Encoding.ASCII);

            _protoServer.FilePoke(_filePath, Encoding.ASCII)
            .Replace("a", "x");

            ExecuteTask();
        }

        [Test]
        public void AlertGivenIfNothingReplaced()
        {
            SetFileContents("abcd");

            _protoServer.FilePoke(_filePath)
                .Replace("x", "y");

            var result = ExecuteTask();
            Assert.AreEqual(1, result.Count(x => x.Status == DeploymentItemStatus.Good));
            Assert.AreEqual(1, result.Count(x => x.Status == DeploymentItemStatus.Alert));
        }

        [Test]
        public void TokensReplacedInPath()
        {
            var path = Guid.NewGuid() + "{{Environment}}";
            HUB.Settings = new DropkickConfiguration
                           {
                               Environment = "environment"
                           };
            _protoServer.FilePoke(path);
            AssertFilePathIs(path.Replace("{{Environment}}", "environment"));
        }

        [Test]
        public void TokensReplacedInPatternAndReplacement()
        {
            SetFileContents("abcd");
            HUB.Settings = new DropkickConfiguration
                           {
                               Environment = "b"
                           };

            _protoServer.FilePoke(_filePath)
                .Replace("a{{Environment}}", "{{Environment}}x");
            ExecuteTask();

            AssertFileContents("bxcd");
        }

        [Test]
        public void PathMappedToPhysicalServer()
        {
            SetFileContents("abcd");

            _protoServer.FilePoke(_filePath);
            ExecuteTask();

            Assert.AreEqual(_filePath, _physicalServer.MappedPath); 
        }
    }
}
