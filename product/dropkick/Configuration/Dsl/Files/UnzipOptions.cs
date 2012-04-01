namespace dropkick.Configuration.Dsl.Files
{
   public interface UnzipOptions
   {
      UnzipOptions To(string destinationPath);

      /// <summary>
      /// What to do, if an existing file found.
      /// Don't use with DeleteDestinationBeforeDeploying
      /// </summary>
      /// <param name="action"></param>
      /// <returns></returns>
      UnzipOptions ExistingFilesAction(dropkick.Tasks.Files.ExtractExistingFileAction action);

      /// <summary>
      /// Don't use with ExistingFilesAction
      /// </summary>
      void DeleteDestinationBeforeDeploying();
   }
}
