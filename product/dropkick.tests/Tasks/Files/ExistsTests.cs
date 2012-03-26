using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dropkick.FileSystem;
using dropkick.Tasks.Files;
using NUnit.Framework;
using Path = System.IO.Path;

namespace dropkick.tests.Tasks.Files {

   [TestFixture]
   [Category("Integration")]
   public class ExistsTests {

      [Test]
      public void FilesShouldExistTests() {
         string tempFile = Path.GetTempFileName();

         //this exists
         var fileDoesExist = new ExistsTask(new DotNetPath(), new List<string> { tempFile }, null, null, null);
         var result = fileDoesExist.Execute();
         result.ContainsError().ShouldBeFalse();

         //this does not.
         var fileDoesNotExist = new ExistsTask(new DotNetPath(), new List<string> { tempFile + Guid.NewGuid() + ".thisShouldNotExist" }, null, null, null);
         var resultNotExists = fileDoesNotExist.Execute();
         resultNotExists.ContainsError().ShouldBeTrue();
      }

      [Test]
      public void FilesShould_NOT_ExistTests() {
         string tempFile = Path.GetTempFileName();

         //this exists
         var fileDoesExist = new ExistsTask(new DotNetPath(), null, null, new List<string> { tempFile }, null);
         var result = fileDoesExist.Execute();
         result.ContainsError().ShouldBeTrue();

         //this does not. - result should contain errors!
         var fileDoesNotExist = new ExistsTask(new DotNetPath(), null, null, new List<string> { tempFile + Guid.NewGuid() + ".thisShouldNotExist" }, null);
         var resultNotExists = fileDoesNotExist.Execute();
         resultNotExists.ContainsError().ShouldBeFalse();
      }

      [Test]
      public void DirectoryShouldExistTests() {
         string tempFile = Path.GetTempPath();

         //this exists
         var fileDoesExist = new ExistsTask(new DotNetPath(), null, new List<string> { tempFile }, null, null);
         var result = fileDoesExist.Execute();
         result.ContainsError().ShouldBeFalse();

         //this does not.
         var fileDoesNotExist = new ExistsTask(new DotNetPath(), null, new List<string> { tempFile + Guid.NewGuid() + ".thisShouldNotExist", tempFile + Guid.NewGuid() + ".thisShouldNotExistToo" }, null, null);
         var resultNotExists = fileDoesNotExist.Execute();
         //should contain an error
         resultNotExists.ContainsError().ShouldBeTrue();
         //should be two errors
         resultNotExists.Results.Count(x => x.Status == dropkick.DeploymentModel.DeploymentItemStatus.Error).ShouldBeEqualTo(2);
      }

      [Test]
      public void DirectoryShould_NOT_ExistTests() {
         string tempFile = Path.GetTempPath();

         //this exists
         var fileDoesExist = new ExistsTask(new DotNetPath(), null, null, null, new List<string> { tempFile });
         var result = fileDoesExist.Execute();
         result.ContainsError().ShouldBeTrue();

         //this does not.
         var fileDoesNotExist = new ExistsTask(new DotNetPath(), null, null, null, new List<string> { tempFile + Guid.NewGuid() + ".thisShouldNotExist", tempFile + Guid.NewGuid() + ".thisShouldNotExistToo" });
         var resultNotExists = fileDoesNotExist.Execute();
         //should contain an error
         resultNotExists.ContainsError().ShouldBeFalse();
         //should be two Good results
         resultNotExists.Results.Count(x => x.Status == dropkick.DeploymentModel.DeploymentItemStatus.Good).ShouldBeEqualTo(2);
      }
   }
}
