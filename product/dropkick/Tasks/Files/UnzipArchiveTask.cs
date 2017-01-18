using System;
using System.IO;
using dropkick.DeploymentModel;
using Ionic.Zip;
using log4net;
using Path = dropkick.FileSystem.Path;

namespace dropkick.Tasks.Files
{
   /// <summary>
   /// What to do, if an existing file found.
   /// Maps directly to Ionic.Zip.ExtractExistingFileAction
   /// </summary>
   public enum ExtractExistingFileAction
   {
      Throw = 0,
      OverwriteSilently = 1,
      DoNotOverwrite = 2,
      InvokeExtractProgressEvent = 3,
   }

   public class UnzipArchiveTask : BaseIoTask
   {
      readonly ILog _log = LogManager.GetLogger(typeof(UnzipArchiveTask));
      readonly DestinationCleanOptions _options;
      readonly ExtractExistingFileAction? _explicitExistingFileAction;
      string _zipArchiveFilename;
      string _to;

      public UnzipArchiveTask(string @zipArchiveFilename, string to, DestinationCleanOptions options, Path path, ExtractExistingFileAction? explicitExistingFileAction)
         : base(path)
      {
         _zipArchiveFilename = zipArchiveFilename;
         _to = to;
         _options = options;
         _explicitExistingFileAction = explicitExistingFileAction;
      }

      public override string Name
      {
         get { return string.Format("Unzip '{0}' to '{1}'", _zipArchiveFilename, _to); }
      }

      public override DeploymentResult Execute()
      {
         var result = new DeploymentResult();

         validatePaths(result);

         _zipArchiveFilename = _path.GetFullPath(_zipArchiveFilename);
         _to = _path.GetFullPath(_to);

         if(_options == DestinationCleanOptions.Delete)
            DeleteDestinationFirst(new DirectoryInfo(_to), result);

         unzipArchive(result, new DirectoryInfo(_to));

         result.AddGood(Name);

         return result;
      }

      public override DeploymentResult VerifyCanRun()
      {
         var result = new DeploymentResult();

         validatePaths(result);

         _zipArchiveFilename = _path.GetFullPath(_zipArchiveFilename);
         _to = _path.GetFullPath(_to);

         if(_options == DestinationCleanOptions.Delete) {
            if(_explicitExistingFileAction != null) {
               //could lead to inconsistent or unexpected behaviour...
               result.AddError("Don't call DeleteDestinationBeforeDeploying() and ExistingFilesAction() on the same task, use only one of them!");
            } else {
               result.AddAlert("The files and directories in '{0}' will be deleted before deploying", _to);
            }
         }

         testArchive(result);

         return result;
      }

      private void validatePaths(DeploymentResult result)
      {
         ValidateIsFile(result, _zipArchiveFilename);
         ValidateIsDirectory(result, _to);
      }

      private void testArchive(DeploymentResult result)
      {
         if(!ZipFile.IsZipFile(_zipArchiveFilename))
            result.AddError(String.Format("The file '{0}' is not a valid zip archive.", _zipArchiveFilename));

         using(var zip = ZipFile.Read(_zipArchiveFilename)) {
            result.AddGood("{0} items will be extracted from '{1}' to '{2}'", zip.Count, _zipArchiveFilename, _to);
         }
      }

      private void unzipArchive(DeploymentResult result, DirectoryInfo destination)
      {
         if(!destination.Exists)
            destination.Create();

         var count = 0;
         using(var zip = ZipFile.Read(_zipArchiveFilename)) {
            //doesn't work, if you extract entries one by one. You have to set it explicitly for each ZipEntry!
            //zip.ExtractExistingFile = getExistingFileAction();
            var existingFileAction = getExistingFileAction();
            foreach(var zipEntry in zip) {
               zipEntry.ExtractExistingFile = existingFileAction;
               logUnzip("Unzipping '{0}'", zipEntry.FileName);
               zipEntry.Extract(destination.FullName);
               count++;
            }
         }
         result.AddGood("{0} files unzipped", count);
      }

      private Ionic.Zip.ExtractExistingFileAction getExistingFileAction()
      {
         if(_explicitExistingFileAction.HasValue)
            switch(_explicitExistingFileAction.Value) {
               //bwah...
               case ExtractExistingFileAction.Throw:
                  return Ionic.Zip.ExtractExistingFileAction.Throw;
               case ExtractExistingFileAction.OverwriteSilently:
                  return Ionic.Zip.ExtractExistingFileAction.OverwriteSilently;
               case ExtractExistingFileAction.DoNotOverwrite:
                  return Ionic.Zip.ExtractExistingFileAction.DoNotOverwrite;
               case ExtractExistingFileAction.InvokeExtractProgressEvent:
                  return Ionic.Zip.ExtractExistingFileAction.InvokeExtractProgressEvent;
               default:
                  return Ionic.Zip.ExtractExistingFileAction.Throw;
            }

         return _options == DestinationCleanOptions.Delete
                  ? Ionic.Zip.ExtractExistingFileAction.OverwriteSilently
                  : Ionic.Zip.ExtractExistingFileAction.Throw;
      }

      private void logUnzip(string message, params object[] args)
      {
         _log.DebugFormat(message, args);
      }
   }
}
