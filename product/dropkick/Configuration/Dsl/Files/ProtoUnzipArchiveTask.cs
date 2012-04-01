using dropkick.DeploymentModel;
using dropkick.FileSystem;
using dropkick.Tasks;
using dropkick.Tasks.Files;
using Magnum;

namespace dropkick.Configuration.Dsl.Files
{
   public class ProtoUnzipArchiveTask : BaseProtoTask, UnzipOptions
   {
      readonly Path _path;
      readonly string _archiveFilename;
      string _to;
      DestinationCleanOptions _options = DestinationCleanOptions.None;
      ExtractExistingFileAction? _explicitExistingFileAction = null;

      public ProtoUnzipArchiveTask(Path path, string archiveFilename)
      {
         Guard.AgainstEmpty(archiveFilename);
         _path = path;
         _archiveFilename = ReplaceTokens(archiveFilename);
      }

      public UnzipOptions To(string destinationPath)
      {
         _to = ReplaceTokens(destinationPath);
         return this;
      }

      public UnzipOptions ExistingFilesAction(ExtractExistingFileAction action)
      {
         _explicitExistingFileAction = action;
         return this;
      }
      public void DeleteDestinationBeforeDeploying()
      {
         _options = DestinationCleanOptions.Delete;
      }

      public override void RegisterRealTasks(PhysicalServer server)
      {
         var to = server.MapPath(_to);
         var archiveFilename = server.MapPath(_archiveFilename);
         var task = new UnzipArchiveTask(archiveFilename, to, _options, _path, _explicitExistingFileAction);
         server.AddTask(task);
      }
   }
}
