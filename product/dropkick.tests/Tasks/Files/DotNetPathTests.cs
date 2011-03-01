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
    using dropkick.DeploymentModel;
    using FileSystem;
    using NUnit.Framework;

    [TestFixture]
    public class DotNetPathTests
    {
        DotNetPath _path;

        [SetUp]
        public void Setup()
        {
            _path = new DotNetPath();
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(".\\stuff"))
                Directory.Delete(".\\stuff");
        }

        [Test]
        public void DirectoryExists()
        {
            Directory.CreateDirectory(".\\stuff");
            Assert.IsTrue(_path.IsDirectory(".\\stuff"));
        }

        [Test]
        public void IsDirectoryReturnsFalseForAFile()
        {
            File.Create(".\\stuff.txt").Dispose();
            Assert.IsFalse(_path.IsDirectory(".\\stuff.txt"));
        }

        [Test]
        public void FileExists()
        {
            File.Create(".\\stuff.txt").Dispose();
            Assert.IsTrue(_path.IsFile(".\\stuff.txt"));
        }

        [Test, Explicit]
        public void RemotepathIsFileTest()
        {
            Assert.IsTrue(_path.IsFile(@"\\srvtestwebtpg\E$\FHLB MQApps\BloombergIntegration\bin\des.exe"));
        }

        [Test, Explicit]
        public void RemotePathIsDirectoryTest()
        {
            Assert.IsTrue(_path.IsDirectory(@"\\localhost\Appl2\Warehouse"));
        }

        [Test]
        public void IsFileReturnsFalseForADirecroty()
        {
            Directory.CreateDirectory(".\\stuff");
            Assert.IsFalse(_path.IsFile(".\\stuff"));
        }

        [Test][Explicit]
        public void RemoteFileTest()
        {
            Assert.IsTrue(File.Exists(@"\\srvtestwebtpg\E$\FHLB MQApps\BloombergIntegration\bin\FHLBank.BloombergIntegration.Host.exe.config"));
        }

        //valid characters in a share name .~!@#$%^&()_-'{} and spaces

        [Test]
        public void when_converting_a_share_to_a_local_path_it_should_work_when_the_share_is_prepended_with_a_tilda_aka_site_root()
        {
            var server = new DeploymentServer("localhost");
            Assert.AreEqual(@"C:\temp", _path.ConvertUncShareToLocalPath(server, @"~\c$\temp"));
        }
        
        [Test]
        public void when_converting_a_share_to_a_local_path_it_should_work_when_the_share_is_prepended_with_the_server()
        {
            var server = new DeploymentServer("localhost");
            Assert.AreEqual(@"C:\temp", _path.ConvertUncShareToLocalPath(server, @"\\localhost\c$\temp"));
        }

        [Test]
        public void when_converting_a_share_to_a_local_path_it_should_work_when_the_share_has_a_dollar_sign_in_it()
        {
            var server = new DeploymentServer("localhost");
            Assert.AreEqual(@"C:\temp", _path.ConvertUncShareToLocalPath(server, @"~\c$\temp"));
        }
        
        [Test,Explicit]
        public void when_converting_a_share_to_a_local_path_it_should_work_when_the_share_has_an_underscore_in_it()
        {
            var server = new DeploymentServer("localhost");
            Assert.AreEqual(@"C:\code", _path.ConvertUncShareToLocalPath(server, @"~\code_"));

        }       
        
        [Test,Explicit]
        public void when_converting_a_share_to_a_local_path_it_should_work_when_the_share_has_a_pound_sign_in_it()
        {
            var server = new DeploymentServer("localhost");
            Assert.AreEqual(@"C:\code", _path.ConvertUncShareToLocalPath(server, @"~\code#"));

        }
    }
}