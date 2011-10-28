namespace dropkick.tests.Tasks.Iis
{
    using System;
    using System.IO;
    using dropkick.Tasks.Iis;
    using Microsoft.Web.Administration;
    using NUnit.Framework;

    public class ExistingVirtualDirectorySpecs
    {
        #region Nested type: ExistingVirtualDirectorySpecsBase

        public abstract class ExistingVirtualDirectorySpecsBase : TinySpec
        {
            protected const string WebServer = "localhost";
            protected const string WebSiteName = "Default Web Site";
            protected const string VirtualDirectory = "vdirtest";
            protected const string NewPath = @"C:\NewPath";
            protected const string OldPath = @"C:\OldPath";
            protected const string NewAppPool = "NewAppPool";
            protected const string OldAppPool = "OldAppPool";
            protected static readonly string NewManagedRuntimeVersion = ManagedRuntimeVersion.V4;
            protected static readonly string OldManagedRuntimeVersion = ManagedRuntimeVersion.V2;
            protected const bool New32bitOn64 = true;
            protected const bool Old32bitOn64 = false;
            protected const ManagedPipelineMode NewManagedPipelineMode = ManagedPipelineMode.Integrated;
            protected const ManagedPipelineMode OldManagedPipelineMode = ManagedPipelineMode.Classic;

            protected VirtualDirectory vDir;
            protected Application application;
            protected ApplicationPool applicationPool;

            public override void Context()
            {
                Directory.CreateDirectory(OldPath);
                Directory.CreateDirectory(NewPath);

                var task = new Iis7Task
                {
                    PathOnServer = OldPath,
                    ServerName = WebServer,
                    VirtualDirectoryPath = VirtualDirectory,
                    AppPoolName = OldAppPool,
                    ManagedRuntimeVersion = OldManagedRuntimeVersion,
                    WebsiteName = WebSiteName,
                    Enable32BitAppOnWin64 = Old32bitOn64,
                    UseClassicPipeline = OldManagedPipelineMode == ManagedPipelineMode.Classic
                };
                var output = task.Execute();

                foreach (var item in output.Results)
                {
                    Console.WriteLine(item.Message);
                }
            }

            protected void LoadVirtualDirectory()
            {
                using (var iis = ServerManager.OpenRemote("localhost"))
                {
                    var site = iis.Sites[WebSiteName];
                    var appPath = "/" + VirtualDirectory;
                    application = site.Applications[appPath];
                    vDir = application.VirtualDirectories["/"];
                    applicationPool = iis.ApplicationPools[application.ApplicationPoolName];
                }
            }
        }

        #endregion

        #region Nested type: When_updating_an_existing_virtual_directory

        [Category("Iis7Task")]
        [Category("Integration")]
        public class When_updating_an_existing_virtual_directory : ExistingVirtualDirectorySpecsBase
        {
            public override void Because()
            {
                var task = new Iis7Task
                {
                    PathOnServer = NewPath,
                    ServerName = WebServer,
                    VirtualDirectoryPath = VirtualDirectory,
                    AppPoolName = NewAppPool,
                    ManagedRuntimeVersion = NewManagedRuntimeVersion,
                    WebsiteName = WebSiteName,
                    Enable32BitAppOnWin64 = New32bitOn64,
                    UseClassicPipeline = NewManagedPipelineMode == ManagedPipelineMode.Classic
                };

                var output = task.Execute();

                foreach (var item in output.Results)
                {
                    Console.WriteLine(item.Message);
                }

                LoadVirtualDirectory();
            }

            [Fact]
            public void It_should_update_the_path()
            {
                vDir.PhysicalPath.ShouldBeEqualTo(NewPath);
            }

            [Fact]
            public void It_should_update_the_apppool()
            {
                application.ApplicationPoolName.ShouldBeEqualTo(NewAppPool);
            }

            [Fact]
            public void It_should_update_the_managed_runtime_version()
            {
                applicationPool.ManagedRuntimeVersion.ShouldBeEqualTo(NewManagedRuntimeVersion);
            }

            [Fact]
            public void It_should_update_the_pipeline_mode()
            {
                applicationPool.ManagedPipelineMode.ShouldBeEqualTo(NewManagedPipelineMode);
            }

            [Fact]
            public void It_should_update_32bit_on_64bit_flag()
            {
                applicationPool.Enable32BitAppOnWin64.ShouldBeEqualTo(New32bitOn64);
            }
        }

        #endregion
    }
}