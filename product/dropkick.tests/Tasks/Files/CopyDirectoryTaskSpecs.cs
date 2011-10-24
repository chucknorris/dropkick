using System.IO;
using dropkick.FileSystem;
using dropkick.Tasks.Files;
using NUnit.Framework;

namespace dropkick.tests.Tasks.Files
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

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

        public class when_copying_files_to_a_local_directory_and_the_directory_does_not_exist : CopyDirectoryTaskSpecsBase
        {
            public override void Context()
            {
                base.Context();
                task = new CopyDirectoryTask(_source, _dest, null, DestinationCleanOptions.None, null, new DotNetPath());
            }

            [Fact]
            public void should_be_able_to_get_the_contents_of_a_copied_file()
            {
                var s = File.ReadAllText(_path.Combine(_dest, "test.txt"));
                Assert.AreEqual("the test\r\n", s);
            }

        }

        public class when_copying_files_to_a_local_directory_and_the_directory_exists : CopyDirectoryTaskSpecsBase
        {
            public override void Context()
            {
                base.Context();
                Directory.CreateDirectory(_dest);
                task = new CopyDirectoryTask(_source, _dest, null, DestinationCleanOptions.None, null, new DotNetPath());
            }

            [Fact]
            public void should_be_able_to_get_the_contents_of_a_copied_file()
            {
                var s = File.ReadAllText(_path.Combine(_dest, "test.txt"));
                Assert.AreEqual("the test\r\n", s);
            }
        }

        public class when_copying_files_to_a_local_directory_and_the_directory_exists_and_is_read_only : CopyDirectoryTaskSpecsBase
        {
            public override void Context()
            {
                base.Context();
                Directory.CreateDirectory(_dest);
                var destDir = new DirectoryInfo(_dest);
                destDir.Attributes = FileAttributes.ReadOnly;

                task = new CopyDirectoryTask(_source, _dest, null, DestinationCleanOptions.None, null, new DotNetPath());
            }

            [Fact]
            public void should_be_able_to_get_the_contents_of_a_copied_file()
            {
                var s = File.ReadAllText(_path.Combine(_dest, "test.txt"));
                Assert.AreEqual("the test\r\n", s);
            }
        }

        public class when_copying_files_to_a_local_directory_and_the_destination_file_is_readonly : CopyDirectoryTaskSpecsBase
        {
            public override void Context()
            {
                base.Context();
                Directory.CreateDirectory(_dest);
                var dest = _path.Combine(_dest, "test.txt");
                File.WriteAllLines(dest, new[] { "bad file" });
                var destFile = new FileInfo(dest);
                destFile.IsReadOnly = true;
                Assert.IsTrue(destFile.IsReadOnly, "Expected the destination file to be readonly");

                task = new CopyDirectoryTask(_source, _dest, null, DestinationCleanOptions.None, null, new DotNetPath());
            }

            [Fact]
            public void should_be_able_to_get_the_contents_of_a_copied_file()
            {
                var s = File.ReadAllText(_path.Combine(_dest, "test.txt"));
                Assert.AreEqual("the test\r\n", s);
            }
        }

        public class when_copying_files_to_a_local_directory_and_excluding_certain_items : CopyDirectoryTaskSpecsBase
        {
            public override void Context()
            {
                base.Context();

                File.WriteAllLines(_path.Combine(_source, "notcopied.txt"), new[] { "new" });
                Directory.CreateDirectory(_dest);
                var dest = _path.Combine(_dest, "test.txt");
                File.WriteAllLines(dest, new[] { "bad file" });

                IList<Regex> ignoredCopyPatterns = new List<Regex>();
                ignoredCopyPatterns.Add(new Regex("notcopied*"));

                task = new CopyDirectoryTask(_source, _dest, ignoredCopyPatterns, DestinationCleanOptions.Clear, null, new DotNetPath());
            }

            [Fact]
            public void should_be_able_to_get_the_contents_of_a_copied_file()
            {
                var s = File.ReadAllText(_path.Combine(_dest, "test.txt"));
                Assert.AreEqual("the test\r\n", s);
            }

            [Fact]
            public void should_not_include_the_excluded_file()
            {
                Assert.AreEqual(false, File.Exists(_path.Combine(_dest, "notcopied.txt")));
            }
        }

        public class when_copying_files_to_a_local_directory_and_cleaning_the_files_prior : CopyDirectoryTaskSpecsBase
        {
            private string subdirectory;

            public override void Context()
            {
                base.Context();
                Directory.CreateDirectory(_dest);
                subdirectory = _path.Combine(_dest, "sub");
                Directory.CreateDirectory(subdirectory);
                var dest = _path.Combine(_dest, "test.txt");
                File.WriteAllLines(dest, new[] { "bad file" });

                task = new CopyDirectoryTask(_source, _dest, null, DestinationCleanOptions.Clear, null, new DotNetPath());
            }

            [Fact]
            public void should_be_able_to_get_the_contents_of_a_copied_file()
            {
                var s = File.ReadAllText(_path.Combine(_dest, "test.txt"));
                Assert.AreEqual("the test\r\n", s);
            }

            [Fact]
            public void should_still_have_a_sub_directory()
            {
                Assert.AreEqual(true, Directory.Exists(subdirectory));
            }
        }

        public class when_copying_files_to_a_local_directory_and_cleaning_the_files_prior_excluding_some_items : CopyDirectoryTaskSpecsBase
        {
            private string subdirectoryFile;

            public override void Context()
            {
                base.Context();
                Directory.CreateDirectory(_dest);
                string subdirectory = _path.Combine(_dest, "sub");
                Directory.CreateDirectory(subdirectory);
                var dest = _path.Combine(_dest, "test.txt");
                File.WriteAllLines(dest, new[] { "bad file" });
                File.WriteAllLines(_path.Combine(_dest, "notcleared.txt"), new[] { "old" });
                subdirectoryFile = _path.Combine(subdirectory, "app_offline.htm");
                File.WriteAllLines(subdirectoryFile, new[] { "ignored" });

                IList<Regex> clearIgnoredPatterns = new List<Regex>();
                clearIgnoredPatterns.Add(new Regex("notcleared*"));
                clearIgnoredPatterns.Add(new Regex(@"[Aa]pp_[Oo]ffline\.htm"));

                task = new CopyDirectoryTask(_source, _dest, null, DestinationCleanOptions.Clear, clearIgnoredPatterns, new DotNetPath());
            }

            [Fact]
            public void should_be_able_to_get_the_contents_of_a_copied_file()
            {
                var s = File.ReadAllText(_path.Combine(_dest, "test.txt"));
                Assert.AreEqual("the test\r\n", s);
            }

            [Fact]
            public void should_not_delete_the_excluded_file()
            {
                Assert.AreEqual(true, File.Exists(_path.Combine(_dest, "notcleared.txt")));
            }  
            
            [Fact]
            public void should_not_delete_the_excluded_file_in_a_subdirectory()
            {
                Assert.AreEqual(true, File.Exists(subdirectoryFile));
            }
        }

        public class when_copying_files_to_a_local_directory_and_deleting_the_files_prior : CopyDirectoryTaskSpecsBase
        {
            public override void Context()
            {
                base.Context();
                Directory.CreateDirectory(_dest);
                var dest = _path.Combine(_dest, "test.txt");
                File.WriteAllLines(dest, new[] { "bad file" });

                task = new CopyDirectoryTask(_source, _dest, null, DestinationCleanOptions.Delete, null, new DotNetPath());
            }

            [Fact]
            public void should_be_able_to_get_the_contents_of_a_copied_file()
            {
                var s = File.ReadAllText(_path.Combine(_dest, "test.txt"));
                Assert.AreEqual("the test\r\n", s);
            }
        }

        public class when_copying_files_to_a_local_directory_and_deleting_the_files_prior_and_the_directory_exists_and_is_read_only : CopyDirectoryTaskSpecsBase
        {
            public override void Context()
            {
                base.Context();
                Directory.CreateDirectory(_dest);
                var destDir = new DirectoryInfo(_dest);
                destDir.Attributes = FileAttributes.ReadOnly;

                task = new CopyDirectoryTask(_source, _dest, null, DestinationCleanOptions.Delete, null, new DotNetPath());
            }

            [Fact]
            public void should_be_able_to_get_the_contents_of_a_copied_file()
            {
                var s = File.ReadAllText(_path.Combine(_dest, "test.txt"));
                Assert.AreEqual("the test\r\n", s);
            }
        }

    }
}