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
    using System.IO;
    using dropkick.Tasks.Files;
    using FileSystem;
    using NUnit.Framework;

    [TestFixture]
    public class CopyFileTest
    {
        readonly DotNetPath _path = new DotNetPath();
        const string BaseDir = @".\CopyFileTests";
        //const string BaseDir = @"\\srvtestweb01\e$\CopyFileTests";
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
            File.WriteAllLines(_path.Combine(_sourceDirectory, "test.txt"), new[] {"the test"});
            Directory.CreateDirectory(_destinationDirectory);
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
            string file = _path.Combine(_sourceDirectory, "test.txt");
            var t = new CopyFileTask(file, _destinationDirectory, null, new DotNetPath());
            t.Execute();

            string s = File.ReadAllText(_path.Combine(_destinationDirectory, "test.txt"));
            Assert.AreEqual("the test\r\n", s);
        }

        [Test]
        public void CopyFileToFileWithSameName()
        {
            string file = _path.Combine(_sourceDirectory, "test.txt");
            var t = new CopyFileTask(file, _destinationDirectory, "test.txt", new DotNetPath());
            t.Execute();

            string s = File.ReadAllText(_path.Combine(_destinationDirectory, "test.txt"));
            Assert.AreEqual("the test\r\n", s);
        }

        [Test]
        public void CopyFileToFileWithRename()
        {
            string file = _path.Combine(_sourceDirectory, "test.txt");
            var t = new CopyFileTask(file, _destinationDirectory, "newtest.txt", new DotNetPath());
            t.Execute();

            string s = File.ReadAllText(_path.Combine(_destinationDirectory, "newtest.txt"));
            Assert.AreEqual("the test\r\n", s);
        }

        [Test]
        public void CopyFileToFileWithSameNameAndOverwrite()
        {
            File.WriteAllLines(_path.Combine(_destinationDirectory, "test.txt"), new[] {"bad file"});

            string file = _path.Combine(_sourceDirectory, "test.txt");
            var t = new CopyFileTask(file, _destinationDirectory, "test.txt", new DotNetPath());
            t.Execute();

            string s = File.ReadAllText(_path.Combine(_destinationDirectory, "test.txt"));
            Assert.AreEqual("the test\r\n", s);
        }
    }
}