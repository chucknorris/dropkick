using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dropkick.FileSystem;
using dropkick.Tasks.Files;
using NUnit.Framework;
using Path = System.IO.Path;
using dropkick.DeploymentModel;

namespace dropkick.tests.Tasks.Files {

   [TestFixture]
   [Category("Integration")]
   public class ExistsTests {
      string tempFile = Path.GetTempFileName();
      //these two directories should exist without creating anything...
      string tempPath = Path.GetTempPath();
      string dllPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

      [Test]
      public void FilesShouldExistTests() {
         //this exists
         var fileDoesExist = new ExistsTask(new DotNetPath(), "FilesShouldExistTests", new List<string> { tempFile }, null, null, null);
         var result = fileDoesExist.Execute();
         result.ContainsError().ShouldBeFalse();

         //this does not.
         var fileDoesNotExist = new ExistsTask(new DotNetPath(), "FilesShouldExistTests", new List<string> { tempFile + Guid.NewGuid() + ".thisShouldNotExist" }, null, null, null);
         var resultNotExists = fileDoesNotExist.Execute();
         resultNotExists.ContainsError().ShouldBeTrue();
      }

      [Test]
      public void FilesShould_NOT_ExistTests() {
         //this exists
         var fileDoesExist = new ExistsTask(new DotNetPath(), "FilesShould_NOT_ExistTests", null, null, new List<string> { tempFile }, null);
         var result = fileDoesExist.Execute();
         result.ContainsError().ShouldBeTrue();

         //this does not. - result should contain errors!
         var fileDoesNotExist = new ExistsTask(new DotNetPath(), "FilesShould_NOT_ExistTests", null, null, new List<string> { tempFile + Guid.NewGuid() + ".thisShouldNotExist" }, null);
         var resultNotExists = fileDoesNotExist.Execute();
         resultNotExists.ContainsError().ShouldBeFalse();
      }

      [Test]
      public void DirectoryShouldExistTests() {
         //this exists
         var dirDoesExist = new ExistsTask(new DotNetPath(), "DirectoryShouldExistTests", null, new List<string> { tempPath, dllPath }, null, null);
         var result = dirDoesExist.Execute();
         result.ContainsError().ShouldBeFalse();
         result.ShouldContain(DeploymentItemStatus.Good, 1);

         //this does not.
         var dirDoesNotExist = new ExistsTask(new DotNetPath(), "DirectoryShouldExistTests", null, new List<string> { tempPath + Guid.NewGuid() + ".thisShouldNotExist", tempPath + Guid.NewGuid() + ".thisShouldNotExistToo" }, null, null);
         var resultNotExists = dirDoesNotExist.Execute();
         //should contain an error
         resultNotExists.ContainsError().ShouldBeTrue();
         //should be two errors
         resultNotExists.ShouldContain(DeploymentItemStatus.Error, 2);
      }

      [Test]
      public void DirectoryShould_NOT_ExistTests() {
         //these don't exist, a good thing now
         var dirDoesExist = new ExistsTask(new DotNetPath(), "DirectoryShould_NOT_ExistTests", null, null, null, new List<string> { tempPath + Guid.NewGuid(), dllPath + Guid.NewGuid() });
         var result = dirDoesExist.Execute();
         result.ContainsError().ShouldBeFalse();
         result.ShouldContain(DeploymentItemStatus.Good, 1);
         result.ShouldContain(DeploymentItemStatus.Note, 3);

         //this does not.
         var dirDoesNotExist = new ExistsTask(new DotNetPath(), "DirectoryShould_NOT_ExistTests", null, null, null, new List<string> { tempPath, dllPath });
         var resultNotExists = dirDoesNotExist.Execute();
         //should contain an error
         resultNotExists.ContainsError().ShouldBeTrue();
         //should be two Error results
         resultNotExists.ShouldContain(DeploymentItemStatus.Error, 2);
      }

      [Test]
      public void ShouldAbortOnError_is_set_it_should_not_abort_when_no_error_found() {
         var dirDoesExist_valid = new ExistsTask(new DotNetPath(), "dummy", null, new List<string> { tempPath, dllPath }, null, null,
            true);
         var result = dirDoesExist_valid.Execute();

         result.ContainsError().ShouldBeFalse();
         result.ShouldAbort.ShouldBeFalse();
      }

      [Test]
      public void ShouldAbortOnError_is_set_it_should_abort_when_error_found() {
         var dirDoesExist_invalid = new ExistsTask(new DotNetPath(), "dummy", null, new List<string> { tempPath + Guid.NewGuid(), dllPath + Guid.NewGuid() }, null, null,
            true);
         var result = dirDoesExist_invalid.Execute();

         result.ContainsError().ShouldBeTrue();
         result.ShouldAbort.ShouldBeTrue();
      }

      [Test]
      public void ShouldAbortOnError_not_set_should_not_abort_when_error_found() {
         var dirDoesExist_invalid = new ExistsTask(new DotNetPath(), "dummy", null, new List<string> { tempPath + Guid.NewGuid(), dllPath + Guid.NewGuid() }, null, null,
            false);
         var result = dirDoesExist_invalid.Execute();

         result.ContainsError().ShouldBeTrue();
         result.ShouldAbort.ShouldBeFalse();
      }
   }
}
