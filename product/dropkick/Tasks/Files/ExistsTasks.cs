namespace dropkick.Tasks.Files {
   using System.IO;
   using DeploymentModel;
   using Path = FileSystem.Path;
   using System.Collections.Generic;

   public class ExistsTask : BaseIoTask {
      readonly List<string> _filesShouldExist;
      readonly List<string> _directoriesShouldExist;
      readonly List<string> _filesShould_NOT_Exist;
      readonly List<string> _directoriesShould_NOT_Exist;

      /// <summary>
      /// Why do this? Like "All source files should be at their expected location"...
      /// </summary>
      string _reason;
      bool _shouldAbortOnError;

      public ExistsTask(Path path, string reason, List<string> filesShouldExist, List<string> directoriesShouldExist, List<string> filesShould_NOT_Exist, List<string> directoriesShould_NOT_Exist)
         : this(path, reason, filesShouldExist, directoriesShouldExist, filesShould_NOT_Exist, directoriesShould_NOT_Exist, false) { }

      public ExistsTask(Path path, string reason, List<string> filesShouldExist, List<string> directoriesShouldExist, List<string> filesShould_NOT_Exist, List<string> directoriesShould_NOT_Exist,
          bool shouldAbortOnError)
         : base(path) {
         _filesShouldExist = filesShouldExist ?? new List<string>();
         _directoriesShouldExist = directoriesShouldExist ?? new List<string>();
         _filesShould_NOT_Exist = filesShould_NOT_Exist ?? new List<string>();
         _directoriesShould_NOT_Exist = directoriesShould_NOT_Exist ?? new List<string>();

         _reason = reason;
         _shouldAbortOnError = shouldAbortOnError;
      }

      public override string Name {
         get {
            return "'{0}' - {1} file, {2} directory should exist, and {2} file and {3} directory should NOT!".FormatWith(
                     _reason,
                     _filesShouldExist.Count,
                     _directoriesShouldExist.Count,
                     _filesShould_NOT_Exist.Count,
                     _directoriesShould_NOT_Exist.Count);
         }
      }

      public override DeploymentResult VerifyCanRun() {
         var result = new DeploymentResult();
         result.AddNote(_reason);

         if(_filesShouldExist.Count > 0) {
            var tmpDR = new DeploymentResult();
            foreach(var filePath in _filesShouldExist) {
               string actualPath = _path.GetFullPath(filePath);
               if(!File.Exists(actualPath)) { tmpDR.AddError("  File '{0}' should exist!.".FormatWith(actualPath)); }
            }
            result.MergedWith(tmpDR);
            //errors show up anyway, give some feedback if everything OK.
            if(!tmpDR.ContainsError()) { result.AddNote("  All {0} files found!".FormatWith(_filesShouldExist.Count)); }
         } else {
            result.AddNote("  No Files that should exist.");
         }

         if(_directoriesShouldExist.Count > 0) {
            var tmpDR = new DeploymentResult();
            foreach(var dirPath in _directoriesShouldExist) {
               string actualPath = _path.GetFullPath(dirPath);
               if(!Directory.Exists(actualPath)) { tmpDR.AddError("  Directory '{0}' should exist".FormatWith(actualPath)); }
            }
            result.MergedWith(tmpDR);
            //errors show up anyway, give some feedback if everything OK.
            if(!tmpDR.ContainsError()) { result.AddNote("  All {0} directories found!".FormatWith(_directoriesShouldExist.Count)); }
         } else {
            result.AddNote("  No Directories that should exist.");
         }

         if(_filesShould_NOT_Exist.Count > 0) {
            var tmpDR = new DeploymentResult();
            foreach(var filePath in _filesShould_NOT_Exist) {
               string actualPath = _path.GetFullPath(filePath);
               if(File.Exists(actualPath)) { tmpDR.AddError("  File '{0}' should NOT exist!.".FormatWith(actualPath)); }
            }
            result.MergedWith(tmpDR);
            if(!tmpDR.ContainsError()) { result.AddNote("  None of the {0} files exist!".FormatWith(_filesShould_NOT_Exist.Count)); }
         } else {
            result.AddNote("  No Files that should NOT exist.");
         }

         if(_directoriesShould_NOT_Exist.Count > 0) {
            var tmpDR = new DeploymentResult();
            foreach(var dirPath in _directoriesShould_NOT_Exist) {
               string actualPath = _path.GetFullPath(dirPath);
               if(Directory.Exists(actualPath)) { tmpDR.AddError("  Directory '{0}' should NOT exist".FormatWith(actualPath)); }
            }
            result.MergedWith(tmpDR);
            if(!tmpDR.ContainsError()) { result.AddNote("  None of the {0} directories exist!".FormatWith(_directoriesShould_NOT_Exist.Count)); }
         } else {
            result.AddNote("  No Directories that should NOT exist.");
         }

         if(_shouldAbortOnError && result.ContainsError()) { result.AddAbort(_reason); }
         return result;
      }

      /// <summary>
      /// Calls VerifyCanRun();
      /// </summary>
      /// <returns></returns>
      public override DeploymentResult Execute() {
         return VerifyCanRun();
      }
   }
}