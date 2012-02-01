using System.Linq;
using System.Threading;
using Microsoft.Web.Administration;
using NUnit.Framework;
using dropkick.DeploymentModel;
using dropkick.Tasks.Iis;

namespace dropkick.tests.Tasks.Iis
{
    public class Iis7TaskSpecs
    {
        public abstract class Iis7TaskSpecsContext : TinySpec
        {
            [Fact]
            public void It_should_not_return_any_errors_from_task_execution()
            {
                Assert.IsFalse(Result.ContainsError(), "Results of task execution contained an error.");
            }

            public override void Context()
            {
                Task = new Iis7Task { WebsiteName = TestWebSiteName };
            }

            public override void Because()
            {
                Result = Task.Execute();
                System.Diagnostics.Debug.WriteLine(Result);
            }

            public override void AfterObservations()
            {
                //DeleteSite();
            }

            protected void AssertSiteBinding(string protocol, int port)
            {
                using (var iis = ServerManager.OpenRemote(WebServerName))
                {
                    var site = iis.Sites[TestWebSiteName];
                    Assert.IsNotNull(site, "Site '{0}' was not found.", TestWebSiteName);

                    var binding = site.Bindings.FirstOrDefault(x => x.EndPoint.Port == port);
                    Assert.IsNotNull(binding, "Site '{0}' is not bound to port '{1}'", TestWebSiteName, port);
                    Assert.AreEqual("http", binding.Protocol);
                }
            }


            protected void DeleteSite()
            {
                using (var iis = ServerManager.OpenRemote(WebServerName))
                {
                    var site = iis.Sites[TestWebSiteName];
                    if (site == null)
                        return;

                    site.Delete();
                    iis.CommitChanges();
                    WaitForIis();
                }
            }

            /// <summary>
            /// Wait for IIS to do its thang.
            /// </summary>
            protected void WaitForIis()
            {
                Thread.Sleep(500);
            }

            protected Iis7Task Task;
            protected DeploymentResult Result;
            protected const string TestWebSiteName = "_DropKickTest_4789";
            protected const string WebServerName = "localhost";
        }

        [Category("Integration")]
        public class When_creating_a_site_with_a_single_http_binding : Iis7TaskSpecsContext
        {
            [Fact]
            public void It_should_bind_the_protocol_and_port()
            {
                AssertSiteBinding("http", 16003);
            }

            public override void  Because()
            {
                Task.Bindings = new[] { new IisSiteBinding { Port = 16003 } };
 	            base.Because();
            }
        }

        [Category("Integration")]
        public class When_creating_a_site_with_multiple_http_bindings : Iis7TaskSpecsContext
        {
            [Fact]
            public void It_should_bind_both_ports()
            {
                AssertSiteBinding("http", 16001);
                AssertSiteBinding("http", 16002);
            }

            public override void Because()
            {
                Task.Bindings = new[]
                                    {
                                        new IisSiteBinding { Protocol = "http", Port = 16001 },
                                        new IisSiteBinding { Protocol = "http", Port = 16002 }
                                    };

                base.Because();
            }
        }
    }
}
