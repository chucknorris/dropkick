// Copyright 2007-2010 The Apache Software Foundation.
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace dropkick.tests.Tasks.Files
{
    using System;
    using System.IO;
    using dropkick.Configuration.Dsl.Files;
    using dropkick.DeploymentModel;
    using dropkick.Tasks.Files;
    using FileSystem;
    using NUnit.Framework;

    [TestFixture]
    public class CopyFileTest
    {
        readonly DotNetPath _path = new DotNetPath();
        const string BaseDir = @".\CopyFileTests";
        //const string BaseDir = @"\\srvtestweb01\e$\CopyFileTests";
        string _sourceFilePath;
        string _destinationFolderPath;

        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            var sourceDirectory = @"{0}\source".FormatWith(BaseDir);
            var destinationDirectory = @"{0}\dest".FormatWith(BaseDir);

            if (Directory.Exists(BaseDir))
                Directory.Delete(BaseDir, true);

            Directory.CreateDirectory(BaseDir);
            Directory.CreateDirectory(sourceDirectory);
            File.WriteAllLines(_path.Combine(sourceDirectory, "test.txt"), new[] { "the test" });
            Directory.CreateDirectory(destinationDirectory);

            _sourceFilePath = _path.GetFullPath(_path.Combine(sourceDirectory, "test.txt"));
            _destinationFolderPath = _path.GetFullPath(destinationDirectory);
        }

        [TearDown]
        public void TearDown()
        {
            Directory.Delete(BaseDir, true);
        }

        #endregion

        [Test]
        public void CopyFileToDirectory()
        {
            var t = new CopyFileTask(_sourceFilePath, _destinationFolderPath, null, new DotNetPath());
            t.Execute();

            string s = File.ReadAllText(_path.Combine(_destinationFolderPath, "test.txt"));
            Assert.AreEqual("the test\r\n", s);
        }

        [Test]
        [Explicit]
        public void CopyFileToUncDirectory()
        {
            var toDir = @"\\srvtestmachine\some_directory";
            var fromDir = @".\bob";
            HUB.Settings = new object();

            var proto = new ProtoCopyFileTask(new DotNetPath(), fromDir);
            proto.ToDirectory(toDir);

            var server = new DeploymentServer("dru");

            proto.RegisterRealTasks(server);
        }

        [Test]
        public void CopyFileToFileWithSameName()
        {
            var t = new CopyFileTask(_sourceFilePath, _destinationFolderPath, "test.txt", new DotNetPath());
            t.Execute();

            string s = File.ReadAllText(_path.Combine(_destinationFolderPath, "test.txt"));
            Assert.AreEqual("the test\r\n", s);
        }

        [Test]
        public void CopyFileToFileWithRename()
        {
            var t = new CopyFileTask(_sourceFilePath, _destinationFolderPath, "newtest.txt", new DotNetPath());
            t.Execute();

            string s = File.ReadAllText(_path.Combine(_destinationFolderPath, "newtest.txt"));
            Assert.AreEqual("the test\r\n", s);
        }

        [Test]
        public void CopyFileToFileWithSameNameAndOverwrite()
        {
            File.WriteAllLines(_path.Combine(_destinationFolderPath, "test.txt"), new[] { "bad file" });

            var t = new CopyFileTask(_sourceFilePath, _destinationFolderPath, "test.txt", new DotNetPath());
            t.Execute();

            string s = File.ReadAllText(_path.Combine(_destinationFolderPath, "test.txt"));
            Assert.AreEqual("the test\r\n", s);
        }
        
        [Test]
        public void CopyFileToFileWithSameNameAndOverwriteReadOnly()
        {
            var dest = _path.Combine(_destinationFolderPath, "test.txt");
            File.WriteAllLines(dest, new[] { "bad file" });
            var destFile = new FileInfo(dest);
            //destFile.Attributes = (destFile.Attributes & FileAttributes.ReadOnly);
            destFile.IsReadOnly = true;

            Assert.IsTrue(destFile.IsReadOnly,"Expected the destination file to be readonly");

            var t = new CopyFileTask(_sourceFilePath, _destinationFolderPath, "test.txt", new DotNetPath());
            t.Execute();

            string s = File.ReadAllText(_path.Combine(_destinationFolderPath, "test.txt"));
            Assert.AreEqual("the test\r\n", s);
        }

        [Test]
        [Explicit]
        public void CopyFileToUncPath()
        {
            var sourceUncPath = PathConverter.Convert(Environment.MachineName, _path.GetFullPath(_sourceFilePath));
            var destinationUncPath = PathConverter.Convert(Environment.MachineName, _path.GetFullPath(_destinationFolderPath));

            var t = new CopyFileTask(sourceUncPath, destinationUncPath, null, new DotNetPath());
            t.Execute();

            string s = File.ReadAllText(_path.Combine(destinationUncPath, "test.txt"));
            Assert.AreEqual("the test\r\n", s);
        }

    }
}