using System;
using System.IO;
using dropkick.FileSystem;
using dropkick.Tasks.Files;
using dropkick.Wmi;
using NUnit.Framework;

namespace dropkick.tests.Tasks.Files
{
    public class CopyDirectoryTaskSpecs
    {
        public abstract class CopyDirectoryTaskSpecsBase : TinySpec
        {
            protected CopyDirectoryTask task;
            protected readonly DotNetPath _path = new DotNetPath();
            protected string _baseDir = @".\CopyDirectoryTests";
            protected string _source = @".\CopyDirectoryTests\source";
            protected string _dest = @".\CopyDirectoryTests\dest";

            public override void Context()
            {
                if (Directory.Exists(_baseDir)) { Directory.Delete(_baseDir, true); }

                Directory.CreateDirectory(_baseDir);
                Directory.CreateDirectory(_source);
                File.WriteAllLines(_path.Combine(_source, "test.txt"), new[] { "the test" });
                Directory.CreateDirectory(_dest);
            }

            public override void Because()
            {
                task.Execute();
            }

            public override void AfterObservations()
            {
                Directory.Delete(_baseDir, true);
            }
        }

        public class when_copying_files_to_a_local_directory : CopyDirectoryTaskSpecsBase
        {
            public override void Context()
            {
                base.Context();
                task = new CopyDirectoryTask(_source, _dest, DestinationCleanOptions.None, new DotNetPath());
            }


            [Fact]
            public void should_be_able_to_get_the_contents_of_a_copied_file()
            {
                var s = File.ReadAllText(_path.Combine(_dest, "test.txt"));
                Assert.AreEqual("the test\r\n", s);
            }

        }


        //NOTE: this should already be converted prior to copy.
        //public class when_copying_files_to_a_directory_using_tildas_ : CopyDirectoryTaskSpecsBase
        //{
        //    private const string shareName = "testsldkfjslkd";
        //    private string sharedLoc = @"~\{0}".FormatWith(shareName);
        //    public override void Context()
        //    {
        //        base.Context();
        //        Win32Share.Create("127.0.0.1", shareName, _dest, "");
        //        task = new CopyDirectoryTask(_source, sharedLoc, DestinationCleanOptions.None, new DotNetPath());
        //    }

        //    [Fact]
        //    public void should_be_able_to_get_the_contents_of_a_copied_file()
        //    {
        //        string dest = @"\\127.0.0.1\{0}".FormatWith(shareName);
        //        var s = File.ReadAllText(_path.Combine(dest, "test.txt"));
        //        Assert.AreEqual("the test\r\n", s);
        //    }

        //    public override void AfterObservations()
        //    {
        //        Win32Share.Delete("127.0.0.1", shareName);
        //        base.AfterObservations();

        //    }
        //}
    }
}