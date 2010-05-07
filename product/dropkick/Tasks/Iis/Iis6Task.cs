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

            CheckForSiteAndVDirExistance(DoesSiteExist, DoesVirtualDirectoryExist, result);

            return result;
        }

        public override DeploymentResult Execute()
        {
            DirectoryEntry vdir =
                GetOrMakeNode(WebsiteName, VdirPath, "IIsWebVirtualDir");
            vdir.RefreshCache();

            vdir.Properties["Path"].Value = PathOnServer.FullName;
            CreateApplication(vdir);
            SetIisProperties(vdir);

            vdir.CommitChanges();
            vdir.Close();

            return new DeploymentResult();
        }


        public bool DoesSiteExist()
        {
            return ConvertSiteNameToSiteNumber(WebsiteName) > 0;
        }


        protected bool DoesVirtualDirectoryExist()
        {
            int siteNumber = ConvertSiteNameToSiteNumber(WebsiteName);
            string path = BuildIisPath(siteNumber, VdirPath);
            var entry = new DirectoryEntry(path);

            try
            {
                //trigger the *private* entry.Bind() method
                object adsobject = entry.NativeObject;
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

        private DirectoryEntry GetOrMakeNode(string basePath, string relPath, string schemaClassName)
        {
            //            var vr = new IISVirtualRoot();
            //            string error;
            //            vr.Create("IIS://localhost/W3svc/1/Root", @"C:\bob", "dk_test", out error);

            if (DoesVirtualDirectoryExist())
            {
                return new DirectoryEntry(basePath + relPath);
            }
            var parent = new DirectoryEntry(basePath);
            parent.RefreshCache();
            DirectoryEntry child = parent.Children.Add(relPath.Trim('/'), schemaClassName);
            child.CommitChanges();
            parent.CommitChanges();
            parent.Close();
            return child;
        }

        private void SetIisProperties(DirectoryEntry vdir)
        {
        }

        private void CreateApplication(DirectoryEntry vdir)
        {
            vdir.Invoke("AppCreate2", 0);
        }

        private void CheckVersionOfWindowsAndIis(DeploymentResult result)
        {
            int shouldBe5 = Environment.OSVersion.Version.Major;
            if (shouldBe5 != 5)
                result.AddAlert("This machine does not have IIS6 on it");
        }

        private string BuildIisPath(int siteNumber, string vDirPath)
        {
            return string.Format("IIS://localhost/w3svc/{0}/Root/{1}", siteNumber, vDirPath);
        }

        private int ConvertSiteNameToSiteNumber(string name)
        {
            var e = new DirectoryEntry("IIS://localhost/W3SVC");
            e.RefreshCache();
            foreach (DirectoryEntry entry in e.Children)
            {
                if (entry.SchemaClassName != "IIsWebServer")
                {
                    entry.Close();
                    continue;
                }

                string x = entry.Name;
                entry.Close();
                return int.Parse(x);
            }

            throw new Exception("could find your website");
        }


    }
}