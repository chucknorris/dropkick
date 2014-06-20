using dropkick.Tasks;
using dropkick.Tasks.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dropkick.Configuration.Dsl.Files
{
    public class OpenFolderShareAuthenticationProtoTask : BaseProtoTask
    {
        private readonly string _folderName;
        private readonly string _userName;
        private readonly string _password;

        public OpenFolderShareAuthenticationProtoTask(string folderName, string userName, string password)
        {
            _folderName = ReplaceTokens(folderName);
            _userName = userName;
            _password = password;
        }

        public override void RegisterRealTasks(dropkick.DeploymentModel.PhysicalServer server)
        {
            string to = server.MapPath(_folderName);

            var task = new OpenFolderShareAuthenticationTask(to, _userName, _password);
            server.AddTask(task);
        }
    }
}
