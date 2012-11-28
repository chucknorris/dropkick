namespace dropkick.Configuration.Dsl.Files {
   using DeploymentModel;
   using FileSystem;
   using Tasks;
   using Tasks.Files;
   using System.Linq;
   using System.Collections.Generic;

   public class ProtoExistsTask :
      BaseProtoTask, ExistsOptions {

      readonly Path _path;
      readonly string _reason;
      bool _shouldAbortOnError;

      readonly List<string> _filesShouldExist = new List<string>();
      readonly List<string> _directoriesShouldExist = new List<string>();
      readonly List<string> _filesShould_NOT_Exist = new List<string>();
      readonly List<string> _directoriesShould_NOT_Exist = new List<string>();

      public ProtoExistsTask(Path path, string reason) : this(path, reason, false) { }
      public ProtoExistsTask(Path path, string reason, bool shouldAbortOnError)
         : base() {
         _path = path;
         _reason = reason;
         _shouldAbortOnError = shouldAbortOnError;
      }

      public override void RegisterRealTasks(PhysicalServer server) {

         server.AddTask(new ExistsTask(new DotNetPath(),
            _reason,
            _filesShouldExist.Select(x => server.MapPath(x)).ToList(),
            _directoriesShouldExist.Select(x => server.MapPath(x)).ToList(),
            _filesShould_NOT_Exist.Select(x => server.MapPath(x)).ToList(),
            _directoriesShould_NOT_Exist.Select(x => server.MapPath(x)).ToList(),
            _shouldAbortOnError));
      }

      public ExistsOptions FileShouldExist(params string[] filePaths) {
         foreach(var filePath in filePaths) {
            if(!_filesShouldExist.Contains(filePath)) {
               _filesShouldExist.Add(filePath);
            }
         }
         return this;
      }

      public ExistsOptions DirShouldExist(params string[] dirPaths) {
         foreach(var dirPath in dirPaths) {
            if(!_directoriesShouldExist.Contains(dirPath)) {
               _directoriesShouldExist.Add(dirPath);
            }
         }
         return this;
      }

      public ExistsOptions FileShould_NOT_Exist(params string[] filePaths) {
         foreach(var filePath in filePaths) {
            if(!_filesShould_NOT_Exist.Contains(filePath)) {
               _filesShould_NOT_Exist.Add(filePath);
            }
         }
         return this;
      }

      public ExistsOptions DirShould_NOT_Exist(params string[] dirPaths) {
         foreach(var dirPath in dirPaths) {
            if(!_directoriesShould_NOT_Exist.Contains(dirPath)) {
               _directoriesShould_NOT_Exist.Add(dirPath);
            }
         }
         return this;
      }

      public ExistsOptions ShouldAbortOnError() {
         _shouldAbortOnError = true;
         return this;
      }
   }
}
