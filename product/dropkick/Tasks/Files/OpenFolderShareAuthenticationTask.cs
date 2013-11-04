using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dropkick;
using dropkick.DeploymentModel;
using dropkick.FileSystem;
using System.Text.RegularExpressions;

namespace dropkick.Tasks.Files
{
    public class OpenFolderShareAuthenticationTask : Task
    {
        private readonly string _to;
        private readonly string _userName;
        private readonly string _password;

        public OpenFolderShareAuthenticationTask(string to, string userName, string password)
        {
            _to = to;
            _userName = userName;
            _password = password;
        }

        public string Name
        {
            get { return "Creating new empty folder '{0}' with user name '{1}'".FormatWith(_to, _userName); }
        }

        public DeploymentResult VerifyCanRun()
        {
            var result = new DeploymentResult();
            var to = new DotNetPath().GetFullPath(_to);
            string toParent = GetRootShare(to);
            try
            {
                using (var context = FileShareAuthenticator.BeginFileShareAuthentication(toParent, _userName, _password))
                {
                    result.AddGood(System.IO.Directory.Exists(to) ? "'{0}' already exists.".FormatWith(to) : Name);
                }
            }
            catch (Exception err)
            {
                result.AddError("Failed to access '{0}' as user '{1}'".FormatWith(toParent, _userName), err);
            }
            //TODO figure out a good verify step...
            return result;
        }

        private string GetRootShare(string to)
        {
            var regex = new Regex(@"\\\\[^\\]*\\[^\\]*");
            var match = regex.Match(to);
            if (!match.Success)
            {
                throw new Exception("Unable to parse root share from " + to);
            }
            return match.Value;
        }

        public DeploymentResult Execute()
        {
            var result = new DeploymentResult();
            var to = new DotNetPath().GetFullPath(_to);

            var toParent = GetRootShare(to);
            try
            {
                using (var context = FileShareAuthenticator.BeginFileShareAuthentication(toParent, _userName, _password))
                {
                    result.AddGood("'{0}' authenticated with {1}.".FormatWith(to, _userName));
                }
            }
            catch (Exception err)
            {
                result.AddError("Failed to access '{0}' as user '{1}'".FormatWith(toParent, _userName), err);
            }
            return result;
        }
    }
}
