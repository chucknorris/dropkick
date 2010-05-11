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
    using Path=System.IO.Path;

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
        public void CopyDirectory()
        {
            var t = new CopyDirectoryTask(_source, _dest, DestinationCleanOptions.None);
            t.Execute();

            string s = File.ReadAllText(Path.Combine(_dest, "test.txt"));
            Assert.AreEqual("the test\r\n", s);
        }

        [Test]
        public void CopyFile()
        {
            var t = new CopyFileTask("D:\\dru", "D:\\bob", new DotNetPath());
            t.Execute();
        }
    }
}