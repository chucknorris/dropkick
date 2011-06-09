using System.IO;
using dropkick.DeploymentModel;
using dropkick.FileSystem;
using dropkick.Wmi;
using NUnit.Framework;
using Path = dropkick.FileSystem.Path;

namespace dropkick.tests
{
    public class PathConverterSpecs
    {
        public abstract class PathConverterSpecsBase : TinySpec
        {
            protected Path path;
            protected string localServerName = "localhost";
            protected PhysicalServer server;
            protected string result;

            public override void Context()
            {
                path = new DotNetPath();
                server = new DeploymentServer(localServerName);
            }

            public string test_share_name(string shareName, string destination)
            {
                var serverName = "localhost";
                if (!Directory.Exists(destination))
                {
                    Directory.CreateDirectory(destination);
                }
                Win32Share.Create(serverName, shareName, destination, "");

                var server = new DeploymentServer(serverName);
                var actual = path.GetPhysicalPath(server, @"~\{0}".FormatWith(shareName),false);

                Win32Share.Delete(serverName, shareName);

                Assert.AreEqual(System.IO.Path.GetFullPath(destination), actual);

                return actual;
            }
        }

        [Category("Integration")]
        public class when_using_PathConverter_to_convert_a_path_and_deploying_to_a_local_server : PathConverterSpecsBase
        {
            public override void Because() {}

            [Fact]
            public void should_convert_tilda_c_dollarsign_bill_successfully()
            {
                Assert.AreEqual("C:\\Bill", path.GetPhysicalPath(server, @"~\c$\Bill", false));
            }

            [Fact]
            public void should_convert_c_colon_successfully()
            {
                Assert.AreEqual(@"C:\bob\bill", PathConverter.Convert(server, "C:\\bob\\bill"));
            }

            [Fact]
            public void should_convert_remote_path_with_tilda_shareName_subfolder_successfully()
            {
                var shareFolder = @".\temp";
                var sub_folder = "Bill";

                var share = test_share_name("yoyotemp", shareFolder);

                Assert.AreEqual(System.IO.Path.Combine(System.IO.Path.GetFullPath(shareFolder), sub_folder), path.GetPhysicalPath(server, System.IO.Path.Combine(share, sub_folder), false));
            }
        }

        public class when_using_PathConverter_to_convert_a_path_and_deploying_to_a_remote_server : PathConverterSpecsBase
        {
            public string remoteServerName = "remoteServer";

            public override void Context()
            {
                base.Context();
                server = new DeploymentServer(remoteServerName);
            }

            public override void Because() {}

            [Fact]
            public void should_convert_tilda_c_dollarsign_bill_successfully()
            {
                Assert.AreEqual(@"\\{0}\C$\Bill".FormatWith(remoteServerName), path.GetPhysicalPath(server, @"~\C$\Bill", false));
            }

            [Fact]
            public void should_convert_c_colon_successfully()
            {
                Assert.AreEqual(@"\\{0}\C$\bob\bill".FormatWith(remoteServerName), PathConverter.Convert(server, "C:\\bob\\bill"));
            }
        }
    }
}