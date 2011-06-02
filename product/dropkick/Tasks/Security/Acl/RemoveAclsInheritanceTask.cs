// ==============================================================================
// 
// Heartland Common Framework
// 
// Copyright ©  2010-2011 Heartland Crop Insurance, Inc. All rights reserved.
// 
// ==============================================================================
using System.IO;
using dropkick.DeploymentModel;

namespace dropkick.Tasks.Security.Acl
{
    public class RemoveAclsInheritanceTask : BaseSecurityTask
    {
        #region Fields

        private readonly string _path;

        #endregion

        #region Constructors

        public RemoveAclsInheritanceTask(string path)
        {
            _path = path;
        }

        #endregion

        #region Overriden Members

        public override string Name
        {
            get { return "Remove ACL Inheritance on '{0}'".FormatWith(_path); }
        }

        public override DeploymentResult VerifyCanRun()
        {
            var result = new DeploymentResult();
            base.VerifyInAdministratorRole(result);
            return result;
        }

        public override DeploymentResult Execute()
        {
            var result = new DeploymentResult();

            var security = Directory.GetAccessControl(_path);
            security.SetAccessRuleProtection(true, true);
            Directory.SetAccessControl(_path, security);

            LogSecurity("[security][acl] Removed ACL inheritance on '{0}'. Preserved existing security.",  _path);
            result.AddGood("Removed ACL inheritance on '{0}'. Preserved existing security.",  _path);

            return result;
        }

        #endregion
    }
}