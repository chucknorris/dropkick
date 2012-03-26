namespace dropkick.Tasks.Files {
   using System.IO;
   using DeploymentModel;
   using Path = FileSystem.Path;
   using System.Collections.Generic;

   public class ExistsTask : Task {
      Path _path;
      readonly List<string> _filesShouldExist;
      readonly List<string> _directoriesShouldExist;
      readonly List<string> _filesShould_NOT_Exist;
      readonly List<string> _directoriesShould_NOT_Exist;

      public ExistsTask(Path path, List<string> filesShouldExist, List<string> directoriesShouldExist, List<string> filesShould_NOT_Exist, List<string> directoriesShould_NOT_Exist) {
         _path = path;
         _filesShouldExist = filesShouldExist ?? new List<string>();
         _directoriesShouldExist = directoriesShouldExist ?? new List<string>();
         _filesShould_NOT_Exist = filesShould_NOT_Exist ?? new List<string>();
         _directoriesShould_NOT_Exist = directoriesShould_NOT_Exist ?? new List<string>();
      }

      public string Name {
         get { return "Does {0} file, {1} directory exist, and {2} file and {3} directory does NOT exists?".FormatWith(_filesShouldExist.Count, _directoriesShouldExist.Count, _filesShould_NOT_Exist.Count, _directoriesShould_NOT_Exist.Count); }
      }

      public DeploymentResult VerifyCanRun() {
         var result = new DeploymentResult();

         if(_filesShouldExist.Count > 0) {
            bool fErr = false;
            foreach(var filePath in _filesShouldExist) {
               string actualPath = _path.GetFullPath(filePath);
               if(!_path.IsFile(filePath)) { fErr = true; result.AddError("'{0}' is not an acceptable path. It doesn't appear to be a file.".FormatWith(filePath)); }
            }
            if(!fErr) { result.AddNote("{0} files that should exist.".FormatWith(_filesShouldExist.Count)); }
         } else {
            result.AddNote("No Files that should exist.");
         }

         if(_directoriesShouldExist.Count > 0) {
            bool dErr = false;
            foreach(var dirPath in _directoriesShouldExist) {
               string actualPath = _path.GetFullPath(dirPath);
               if(!_path.IsDirectory(dirPath)) { dErr = true; result.AddError("'{0}' is not an acceptable path. It doesn't appear to be a directory.".FormatWith(dirPath)); }
            }
            if(!dErr) { result.AddNote("{0} directories that should exist.".FormatWith(_directoriesShouldExist.Count)); }
         } else {
            result.AddNote("No Directories that should exist.");
         }

         if(_filesShould_NOT_Exist.Count > 0) {
            bool fnErr = false;
            foreach(var filePath in _filesShould_NOT_Exist) {
               string actualPath = _path.GetFullPath(filePath);
               if(!_path.IsFile(filePath)) { fnErr = true; result.AddError("'{0}' is not an acceptable path. It doesn't appear to be a file.".FormatWith(filePath)); }
            }
            if(!fnErr) { result.AddNote("{0} files that should not exist.".FormatWith(_filesShould_NOT_Exist.Count)); }
         } else {
            result.AddNote("No Files that should not exist.");
         }

         if(_directoriesShould_NOT_Exist.Count > 0) {
            bool dnErr = false;
            foreach(var dirPath in _directoriesShould_NOT_Exist) {
               string actualPath = _path.GetFullPath(dirPath);
               if(!_path.IsDirectory(dirPath)) { dnErr = true; result.AddError("'{0}' is not an acceptable path. It doesn't appear to be a directory.".FormatWith(dirPath)); }
            }
            if(!dnErr) { result.AddNote("{0} directories that should not exist.".FormatWith(_directoriesShould_NOT_Exist.Count)); }
         } else {
            result.AddNote("No Directories that should not exist.");
         }

         return result;
      }

      public DeploymentResult Execute() {
         var result = new DeploymentResult();


         if(_filesShouldExist.Count > 0) {
            foreach(var filePath in _filesShouldExist) {
               string actualPath = _path.GetFullPath(filePath);
               if(File.Exists(actualPath))
                  result.AddGood("File '{0}' exists.".FormatWith(actualPath));
               else {
                  result.AddError("File '{0}' does NOT exist!".FormatWith(actualPath));
               }
            }
         } else {
            result.AddNote("No Files that should exist.");
         }

         if(_directoriesShouldExist.Count > 0) {
            foreach(var filePath in _directoriesShouldExist) {
               string actualPath = _path.GetFullPath(filePath);
               if(Directory.Exists(actualPath))
                  result.AddGood("Directory '{0}' exists.".FormatWith(actualPath));
               else {
                  result.AddError("Directory '{0}' does NOT exist!".FormatWith(actualPath));
               }
            }
         } else {
            result.AddNote("No Directories that should exist.");
         }

         if(_filesShould_NOT_Exist.Count > 0) {
            foreach(var filePath in _filesShould_NOT_Exist) {
               string actualPath = _path.GetFullPath(filePath);
               if(File.Exists(actualPath))
                  result.AddError("File '{0}' should NOT exist!.".FormatWith(actualPath));
               else {
                  result.AddGood("File '{0}' does not exist!".FormatWith(actualPath));
               }
            }
         } else {
            result.AddNote("No Files that should not exist.");
         }

         if(_directoriesShould_NOT_Exist.Count > 0) {
            foreach(var filePath in _directoriesShould_NOT_Exist) {
               string actualPath = _path.GetFullPath(filePath);
               if(Directory.Exists(actualPath))
                  result.AddError("Directory '{0}' should NOT exist".FormatWith(actualPath));
               else {
                  result.AddGood("Directory '{0}' does not exist!".FormatWith(actualPath));
               }
            }
         } else {
            result.AddNote("No Directories that should not exist.");
         }



         return result;
      }
   }


}