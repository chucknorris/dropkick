using Microsoft.Web.Administration;
using NUnit.Framework;
using dropkick.Tasks.Iis;

namespace dropkick.tests.Tasks.Iis
{
	[TestFixture]
	[Category("Iis7Task")]
	public class When_updating_an_existing_virtual_directory : TinySpec
	{
		const string WebServer = "localhost";
		const string WebSiteName = "Default Web Site";
		const string VirtualDirectory = "vdirtest";
		const string NewPath = @"C:\NewPath";
		const string OldPath = @"C:\OldPath";
		const string NewAppPool = "NewAppPool";
		const string OldAppPool = "OldAppPool";
		static readonly string NewManagedRuntimeVersion = ManagedRuntimeVersion.V4;
		static readonly string OldManagedRuntimeVersion = ManagedRuntimeVersion.V2;
		const bool New32bitOn64 = true;
		const bool Old32bitOn64 = false;
		const ManagedPipelineMode NewManagedPipelineMode = ManagedPipelineMode.Integrated;
		const ManagedPipelineMode OldManagedPipelineMode = ManagedPipelineMode.Classic;

		VirtualDirectory _virtualDirectory;
		Application _application;
		ApplicationPool _appPool;

		[Test, Explicit]
		public void It_should_update_the_path()
		{
			_virtualDirectory.PhysicalPath.ShouldBeEqualTo(NewPath);
		}

		[Test, Explicit]
		public void It_should_update_the_apppool()
		{
			_application.ApplicationPoolName.ShouldBeEqualTo(NewAppPool);
		}

		[Test, Explicit]
		public void It_should_update_the_managed_runtime_version()
		{
			_appPool.ManagedRuntimeVersion.ShouldBeEqualTo(NewManagedRuntimeVersion);
		}

		[Test, Explicit]
		public void It_should_update_the_pipeline_mode()
		{
			_appPool.ManagedPipelineMode.ShouldBeEqualTo(NewManagedPipelineMode);
		}

		[Test, Explicit]
		public void It_should_update_32bit_on_64bit_flag()
		{
			_appPool.Enable32BitAppOnWin64.ShouldBeEqualTo(New32bitOn64);
		}

		public override void Because()
		{
			var task = new Iis7Task
			           	{
			           		PathOnServer = NewPath,
			           		ServerName = WebServer,
			           		VdirPath = VirtualDirectory,
			           		AppPoolName = NewAppPool,
			           		ManagedRuntimeVersion = NewManagedRuntimeVersion,
			           		WebsiteName = WebSiteName,
							Enable32BitAppOnWin64 = New32bitOn64,
							UseClassicPipeline = NewManagedPipelineMode == ManagedPipelineMode.Classic
			           	};
			var output = task.Execute();

			foreach (var item in output.Results)
			{
				System.Console.WriteLine(item.Message);
			}

			loadVirtualDirectory();
		}

		public override void Context()
		{
			var task = new Iis7Task
			           	{
							PathOnServer = OldPath,
							ServerName = WebServer,
							VdirPath = VirtualDirectory,
							AppPoolName = OldAppPool,
							ManagedRuntimeVersion = OldManagedRuntimeVersion,
							WebsiteName = WebSiteName,
							Enable32BitAppOnWin64 = Old32bitOn64,
							UseClassicPipeline = OldManagedPipelineMode == ManagedPipelineMode.Classic
			           	};
			var output = task.Execute();

			foreach (var item in output.Results)
			{
				System.Console.WriteLine(item.Message);
			}
		}

		private void loadVirtualDirectory()
		{
			using (var iis = ServerManager.OpenRemote("localhost"))
			{
				var site = iis.Sites[WebSiteName];
				var appPath = "/" + VirtualDirectory;
				_application = site.Applications[appPath];
				_virtualDirectory = _application.VirtualDirectories["/"];
				_appPool = iis.ApplicationPools[_application.ApplicationPoolName];
			}
		}
	}
}
