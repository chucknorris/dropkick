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
            protected readonly DotNetPath Path = new DotNetPath();
            protected string baseDirectory = @".\CopyDirectoryTests";
            protected string sourceDir = @".\CopyDirectoryTests\source";
            protected string destDir = @".\CopyDirectoryTests\dest";

            public override void Context()
            {
                if (Directory.Exists(baseDirectory)) { Directory.Delete(baseDirectory, true); }

                Directory.CreateDirectory(baseDirectory);
                Directory.CreateDirectory(sourceDir);
                File.WriteAllLines(Path.Combine(sourceDir, "test.txt"), new[] { "the test" });
            }

            public override void Because()
            {
                task.Execute();
            }

            public override void AfterObservations()
            {
                Directory.Delete(baseDirectory, true);
            }
        }

        public class when_copying_files_to_a_local_directory_and_the_directory_does_not_exist : CopyDirectoryTaskSpecsBase
        {
            public override void Context()
            {
                base.Context();
                task = new CopyDirectoryTask(sourceDir, destDir, null, DestinationCleanOptions.None, null, new DotNetPath());
            }

            [Fact]
            public void should_be_able_to_get_the_contents_of_a_copied_file()
            {
                var s = File.ReadAllText(Path.Combine(destDir, "test.txt"));
                Assert.AreEqual("the test\r\n", s);
            }

        }

        public class when_copying_files_to_a_local_directory_and_the_directory_exists : CopyDirectoryTaskSpecsBase
        {
            public override void Context()
            {
                base.Context();
                Directory.CreateDirectory(destDir);
                task = new CopyDirectoryTask(sourceDir, destDir, null, DestinationCleanOptions.None, null, new DotNetPath());
            }

            [Fact]
            public void should_be_able_to_get_the_contents_of_a_copied_file()
            {
                var s = File.ReadAllText(Path.Combine(destDir, "test.txt"));
                Assert.AreEqual("the test\r\n", s);
            }
        }

        public class when_copying_files_to_a_local_directory_and_the_directory_exists_and_is_read_only : CopyDirectoryTaskSpecsBase
        {
            public override void Context()
            {
                base.Context();
                Directory.CreateDirectory(base.destDir);
                var destDir = new DirectoryInfo(base.destDir);
                destDir.Attributes = FileAttributes.ReadOnly;

                task = new CopyDirectoryTask(sourceDir, base.destDir, null, DestinationCleanOptions.None, null, new DotNetPath());
            }

            [Fact]
            public void should_be_able_to_get_the_contents_of_a_copied_file()
            {
                var s = File.ReadAllText(Path.Combine(destDir, "test.txt"));
                Assert.AreEqual("the test\r\n", s);
            }
        }

        public class when_copying_files_to_a_local_directory_and_the_destination_file_is_readonly : CopyDirectoryTaskSpecsBase
        {
            public override void Context()
            {
                base.Context();
                Directory.CreateDirectory(destDir);
                var dest = Path.Combine(destDir, "test.txt");
                File.WriteAllLines(dest, new[] { "bad file" });
                var destFile = new FileInfo(dest);
                destFile.IsReadOnly = true;
                Assert.IsTrue(destFile.IsReadOnly, "Expected the destination file to be readonly");

                task = new CopyDirectoryTask(sourceDir, destDir, null, DestinationCleanOptions.None, null, new DotNetPath());
            }

            [Fact]
            public void should_be_able_to_get_the_contents_of_a_copied_file()
            {
                var s = File.ReadAllText(Path.Combine(destDir, "test.txt"));
                Assert.AreEqual("the test\r\n", s);
            }
        }

        public class when_copying_files_to_a_local_directory_and_excluding_certain_items : CopyDirectoryTaskSpecsBase
        {
            public override void Context()
            {
                base.Context();

                File.WriteAllLines(Path.Combine(sourceDir, "notcopied.txt"), new[] { "new" });
                Directory.CreateDirectory(destDir);
                var dest = Path.Combine(destDir, "test.txt");
                File.WriteAllLines(dest, new[] { "bad file" });

                IList<Regex> ignoredCopyPatterns = new List<Regex>();
                ignoredCopyPatterns.Add(new Regex("notcopied*"));

                task = new CopyDirectoryTask(sourceDir, destDir, ignoredCopyPatterns, DestinationCleanOptions.Clear, null, new DotNetPath());
            }

            [Fact]
            public void should_be_able_to_get_the_contents_of_a_copied_file()
            {
                var s = File.ReadAllText(Path.Combine(destDir, "test.txt"));
                Assert.AreEqual("the test\r\n", s);
            }

            [Fact]
            public void should_not_include_the_excluded_file()
            {
                Assert.AreEqual(false, File.Exists(Path.Combine(destDir, "notcopied.txt")));
            }
        }

        public class when_copying_files_to_a_local_directory_and_cleaning_the_files_prior : CopyDirectoryTaskSpecsBase
        {
            private string subdirectory;

            public override void Context()
            {
                base.Context();
                Directory.CreateDirectory(destDir);
                subdirectory = Path.Combine(destDir, "sub");
                Directory.CreateDirectory(subdirectory);
                var dest = Path.Combine(destDir, "test.txt");
                File.WriteAllLines(dest, new[] { "bad file" });

                task = new CopyDirectoryTask(sourceDir, destDir, null, DestinationCleanOptions.Clear, null, new DotNetPath());
            }

            [Fact]
            public void should_be_able_to_get_the_contents_of_a_copied_file()
            {
                var s = File.ReadAllText(Path.Combine(destDir, "test.txt"));
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
                Directory.CreateDirectory(destDir);
                string subdirectory = Path.Combine(destDir, "sub");
                Directory.CreateDirectory(subdirectory);
                var dest = Path.Combine(destDir, "test.txt");
                File.WriteAllLines(dest, new[] { "bad file" });
                File.WriteAllLines(Path.Combine(destDir, "notcleared.txt"), new[] { "old" });
                subdirectoryFile = Path.Combine(subdirectory, "app_offline.htm");
                File.WriteAllLines(subdirectoryFile, new[] { "ignored" });

                IList<Regex> clearIgnoredPatterns = new List<Regex>();
                clearIgnoredPatterns.Add(new Regex("notcleared*"));
                clearIgnoredPatterns.Add(new Regex(@"[Aa]pp_[Oo]ffline\.htm"));

                task = new CopyDirectoryTask(sourceDir, destDir, null, DestinationCleanOptions.Clear, clearIgnoredPatterns, new DotNetPath());
            }

            [Fact]
            public void should_be_able_to_get_the_contents_of_a_copied_file()
            {
                var s = File.ReadAllText(Path.Combine(destDir, "test.txt"));
                Assert.AreEqual("the test\r\n", s);
            }

            [Fact]
            public void should_not_delete_the_excluded_file()
            {
                Assert.AreEqual(true, File.Exists(Path.Combine(destDir, "notcleared.txt")));
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
                Directory.CreateDirectory(destDir);
                var dest = Path.Combine(destDir, "test.txt");
                File.WriteAllLines(dest, new[] { "bad file" });

                task = new CopyDirectoryTask(sourceDir, destDir, null, DestinationCleanOptions.Delete, null, new DotNetPath());
            }

            [Fact]
            public void should_be_able_to_get_the_contents_of_a_copied_file()
            {
                var s = File.ReadAllText(Path.Combine(destDir, "test.txt"));
                Assert.AreEqual("the test\r\n", s);
            }
        }

        public class when_copying_files_to_a_local_directory_and_deleting_the_files_prior_and_the_directory_exists_and_is_read_only : CopyDirectoryTaskSpecsBase
        {
            public override void Context()
            {
                base.Context();
                Directory.CreateDirectory(base.destDir);
                var destDir = new DirectoryInfo(base.destDir);
                destDir.Attributes = FileAttributes.ReadOnly;

                task = new CopyDirectoryTask(sourceDir, base.destDir, null, DestinationCleanOptions.Delete, null, new DotNetPath());
            }

            [Fact]
            public void should_be_able_to_get_the_contents_of_a_copied_file()
            {
                var s = File.ReadAllText(Path.Combine(destDir, "test.txt"));
                Assert.AreEqual("the test\r\n", s);
            }
        }

    }
}