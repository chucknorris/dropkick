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
    public class CopyDirectoryTest
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            if (Directory.Exists(_baseDir))
                Directory.Delete(_baseDir, true);

            Directory.CreateDirectory(_baseDir);
            Directory.CreateDirectory(_source);
            File.WriteAllLines(_path.Combine(_source, "test.txt"), new[] {"the test"});
            Directory.CreateDirectory(_dest);
        }

        [TearDown]
        public void TearDown()
        {
            Directory.Delete(_baseDir, true);
        }

        #endregion

        readonly DotNetPath _path = new DotNetPath();

        string _baseDir = @".\CopyDirectoryTests";
        string _source = @".\CopyDirectoryTests\source";
        string _dest = @".\CopyDirectoryTests\dest";

        [Test]
        public void CopyDirectory()
        {
            var t = new CopyDirectoryTask(_source, _dest, DestinationCleanOptions.None);
            t.Execute();

            var s = File.ReadAllText(_path.Combine(_dest, "test.txt"));
            Assert.AreEqual("the test\r\n", s);
        }

    }
}