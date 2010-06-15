namespace dropkick.Tasks.Iis
{
    using System;
    using System.DirectoryServices;
    using DeploymentModel;

    public class Iis6Task :
        BaseIisTask
    {
        public override int VersionNumber
        {
            get { return 6; }
        }

        public override DeploymentResult VerifyCanRun()
        {
            var result = new DeploymentResult();

            CheckVersionOfWindowsAndIis(result);

            CheckForSiteAndVDirExistance(new Iis6Path(ServerName, WebsiteName).DoesSiteExist, DoesVirtualDirectoryExist, result);

            return result;
        }

        public override DeploymentResult Execute()
        {
            var vdir = CreateVDirNode(WebsiteName, VdirPath, "IIsWebVirtualDir");
            vdir.RefreshCache();

            vdir.Properties["Path"].Value = PathOnServer;
            CreateApplication(vdir);

            vdir.CommitChanges();
            vdir.Close();

            return new DeploymentResult();
        }

        bool DoesVirtualDirectoryExist()
        {
            var entry = new Iis6Path(ServerName, WebsiteName, VdirPath).ToDirectoryEntry();

            try
            {
                //trigger the *private* entry.Bind() method
                var adsobject = entry.NativeObject;
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                entry.Dispose();
            }
        }

        DirectoryEntry CreateVDirNode(string siteName, string vDirName, string schemaClassName)
        {
            if (DoesVirtualDirectoryExist())
            {
                return new Iis6Path(ServerName, siteName, vDirName).ToDirectoryEntry();
            }

            var path = new Iis6Path(ServerName, siteName);
            var parent = path.ToDirectoryEntry();
            parent.RefreshCache();
            var child = parent.Children.Add(vDirName.Trim('/'), schemaClassName);
            child.CommitChanges();
            parent.CommitChanges();
            parent.Close();
            return child;
        }

        static void CreateApplication(DirectoryEntry vdir)
        {
            vdir.Invoke("AppCreate2", 0);
        }

        static void CheckVersionOfWindowsAndIis(DeploymentResult result)
        {
            var shouldBe5 = Environment.OSVersion.Version.Major;
            if (shouldBe5 != 5)
                result.AddAlert("This machine does not have IIS6 on it");
        }
    }
}