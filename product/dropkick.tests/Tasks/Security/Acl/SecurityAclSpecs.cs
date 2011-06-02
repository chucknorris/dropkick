using System;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using dropkick.DeploymentModel;
using dropkick.FileSystem;
using dropkick.Tasks.Security;
using dropkick.Tasks.Security.Acl;
using NUnit.Framework;


namespace dropkick.tests.Tasks.Security.Acl
{
    public class SecurityAclSpecs
    {
        public abstract class SecurityAclSpecsBase : TinySpec
        {
            protected DeploymentResult result;

            public override void Context()
            {

            }
        }

        [ConcernFor("RemoveAclsInheritanceTask"), Category("Integration")]
        public class when_removing_security_acl_inheritance : SecurityAclSpecsBase
        {
            private RemoveAclsInheritanceTask task;
            private string path = @".\removeInheritanceFolder";

            public override void Context()
            {
                base.Context();

                if (Directory.Exists(path)) Directory.Delete(path);
                Directory.CreateDirectory(path);
                task = new RemoveAclsInheritanceTask(path);
            }

            public override void Because()
            {
                result = task.Execute();
            }

            [Fact]
            public void should_remove_inheritance_from_the_folder()
            {
                bool isInherited = false;

                var security = Directory.GetAccessControl(path);
                var rules = security.GetAccessRules(true, true, typeof(NTAccount));

                foreach (FileSystemAccessRule rule in rules)
                {
                    if (rule.IsInherited) isInherited = true;
                }

                Assert.AreEqual(false, isInherited);
            }

            [Fact]
            public void should_preserve_the_rules_that_were_there_before()
            {
                var security = Directory.GetAccessControl(path);
                var rules = security.GetAccessRules(true, true, typeof(NTAccount));
                Assert.AreNotEqual(0, rules.Count);
            }
        }

        [ConcernFor("ClearAclsTask"), Category("Integration")]
        public class when_clearing_acls_from_a_folder_preserving_nothing_and_not_removing_any_default_preservations : SecurityAclSpecsBase
        {
            private ClearAclsTask task;

            private string path = @".\clearAclsFolder";

            public override void Context()
            {
                base.Context();
                if (Directory.Exists(path)) Directory.Delete(path);
                Directory.CreateDirectory(path);
                var createUser = new GrantReadWriteTask(path, WellKnownSecurityRoles.CurrentUser, new DotNetPath());
                result = createUser.Execute();
                Assert.AreEqual(false, result.ContainsError());
                task = new ClearAclsTask(path, null, null);
            }

            public override void Because()
            {
                result = task.Execute();
            }

            [Fact]
            public void should_clear_current_user()
            {
                bool found = false;
                var security = Directory.GetAccessControl(path);
                var rules = security.GetAccessRules(true, true, typeof(NTAccount));
                foreach (AuthorizationRule rule in rules)
                {
                    if (rule.IdentityReference.Value == WellKnownSecurityRoles.CurrentUser)
                    {
                        found = true;
                    }
                }

               Assert.AreEqual(false,found);
            }

        } 
        
        [ConcernFor("ClearAclsTask"), Category("Integration")]
        public class when_clearing_acls_from_a_folder_preserving_currentuser_and_not_removing_any_default_preservations : SecurityAclSpecsBase
        {
            private ClearAclsTask task;

            private string path = @".\clearAclsPreserveCurrentUserFolder";

            public override void Context()
            {
                base.Context();
                if (Directory.Exists(path)) Directory.Delete(path);
                Directory.CreateDirectory(path);
                var createUser = new GrantReadWriteTask(path, WellKnownSecurityRoles.CurrentUser, new DotNetPath());
                result = createUser.Execute();
                Assert.AreEqual(false, result.ContainsError());
                var groupsToPreserve = new System.Collections.Generic.List<string>
                {
                    WellKnownSecurityRoles.CurrentUser
                };
                task = new ClearAclsTask(path, groupsToPreserve, null);
            }

            public override void Because()
            {
                result = task.Execute();
            }

            [Fact]
            public void should_not_clear_current_user()
            {
                bool found = false;
                var security = Directory.GetAccessControl(path);
                var rules = security.GetAccessRules(true, true, typeof(NTAccount));
                foreach (AuthorizationRule rule in rules)
                {
                    if (rule.IdentityReference.Value == WellKnownSecurityRoles.CurrentUser)
                    {
                        found = true;
                    }
                }

               Assert.AreEqual(true,found);
            }

        }

        [ConcernFor("ClearAclsTask"), Category("Integration")]
        public class when_clearing_acls_from_a_folder_preserving_currentuser_and_removing_all_default_preservations_after_removing_inheritance : SecurityAclSpecsBase
        {
            private ClearAclsTask task;

            private string path = @".\clearAclsNoInheritanceFolder";

            public override void Context()
            {
                base.Context();
                if (Directory.Exists(path)) Directory.Delete(path);
                Directory.CreateDirectory(path);
                var createUser = new GrantReadWriteTask(path, WellKnownSecurityRoles.CurrentUser, new DotNetPath());
                result = createUser.Execute();
                Assert.AreEqual(false, result.ContainsError());
                
                RemoveAclsInheritanceTask remove = new RemoveAclsInheritanceTask(path);
                result = remove.Execute();

                var groupsToPreserve = new System.Collections.Generic.List<string>
                {
                    WellKnownSecurityRoles.CurrentUser
                };
                var groupsToRemove = new System.Collections.Generic.List<string>
                {
                    WellKnownSecurityRoles.System,
                    WellKnownSecurityRoles.Administrators,
                    WellKnownSecurityRoles.Users,
                };
                task = new ClearAclsTask(path, groupsToPreserve, groupsToRemove);
            }

            public override void Because()
            {
                result = task.Execute();
            }

            [Fact]
            public void should_not_clear_current_user()
            {
                bool found = false;
                var security = Directory.GetAccessControl(path);
                var rules = security.GetAccessRules(true, true, typeof(NTAccount));
                foreach (AuthorizationRule rule in rules)
                {
                    if (rule.IdentityReference.Value == WellKnownSecurityRoles.CurrentUser)
                    {
                        found = true;
                    }
                }

                Assert.AreEqual(true, found);
            }

        }

    }
}