using System.Linq;
using System.Threading;
using Microsoft.Web.Administration;
using NUnit.Framework;
using dropkick.DeploymentModel;
using dropkick.Tasks.Iis;
using dropkick.Tasks.Security.Certificate;

namespace dropkick.tests.Tasks.Iis
{
    public class Iis7TaskSpecs
    {
        public abstract class Iis7TaskSpecsContext : TinySpec
        {
            // TODO: This is specific to my self-signed cert.
            protected const string CertificateThumbprint = @"13d8ae4000e8d5ac8930c3cdb6c995640c715b86";

            [Fact]
            public void It_should_not_return_any_errors_from_task_verification()
            {
                Assert.IsFalse(VerificationResult.ContainsError(), "Results of task verification contained an error.");
            }

            [Fact]
            public void It_should_not_return_any_errors_from_task_execution()
            {
                Assert.IsFalse(ExecutionResult.ContainsError(), "Results of task execution contained an error.");
            }

            public override void Context()
            {
                Task = new Iis7Task { WebsiteName = TestWebSiteName, ServerName = WebServerName };
            }

            public override void Because()
            {
                VerificationResult = Task.VerifyCanRun();
                System.Diagnostics.Debug.WriteLine(VerificationResult);
                if (VerificationResult.ContainsError()) return;
                
                ExecutionResult = Task.Execute();
                System.Diagnostics.Debug.WriteLine(ExecutionResult);
            }

            public override void AfterObservations()
            {
                DeleteSite();
            }

            protected void AssertSiteBinding(string protocol, int port, string certificateThumbprint = null)
            {
                using (var iis = ServerManager.OpenRemote(WebServerName))
                {
                    var site = iis.Sites[TestWebSiteName];
                    Assert.IsNotNull(site, "Site '{0}' was not found.", TestWebSiteName);

                    var binding = site.Bindings.FirstOrDefault(x => x.EndPoint.Port == port);
                    Assert.IsNotNull(binding, "Site '{0}' is not bound to port '{1}'", TestWebSiteName, port);
                    Assert.AreEqual(protocol, binding.Protocol);

                    if (certificateThumbprint != null)
                    {
                        Assert.AreEqual(CertificateStoreUtility.GetCertificateHashForThumbprint(certificateThumbprint), binding.CertificateHash);
                    }
                }
            }

            protected void AssertNotSiteBinding(int port)
            {
                using (var iis = ServerManager.OpenRemote(WebServerName))
                {
                    var site = iis.Sites[TestWebSiteName];
                    Assert.IsNotNull(site, "Site '{0}' was not found.", TestWebSiteName);

                    var binding = site.Bindings.FirstOrDefault(x => x.EndPoint.Port == port);
                    Assert.IsNull(binding, "Unexpected binding. Site '{0}' is bound to port '{1}'.", TestWebSiteName, port);
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
            protected DeploymentResult ExecutionResult;
            protected DeploymentResult VerificationResult;
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

        [Category("Integration")]
        public class When_creating_a_site_with_a_single_https_binding : Iis7TaskSpecsContext
        {
            [Fact]
            public void It_should_bind_the_https_protocol_and_port()
            {
                AssertSiteBinding("https", 16004, CertificateThumbprint);
            }

            public override void Because()
            {
                Task.Bindings = new[] { new IisSiteBinding { Protocol = "https", Port = 16004, CertificateThumbPrint = CertificateThumbprint } };
                base.Because();
            }
        }

        [Category("Integration")]
        public class When_creating_a_site_with_multiple_https_binding : Iis7TaskSpecsContext
        {
            [Fact]
            public void It_should_bind_both_ports()
            {
                AssertSiteBinding("https", 16005, CertificateThumbprint);
                AssertSiteBinding("https", 16006, CertificateThumbprint);
            }

            public override void Because()
            {
                Task.Bindings = new[]
                                    {
                                        new IisSiteBinding { Protocol = "https", Port = 16005, CertificateThumbPrint = CertificateThumbprint },
                                        new IisSiteBinding { Protocol = "https", Port = 16006, CertificateThumbPrint = CertificateThumbprint }
                                    };
                base.Because();
            }
        }

        [Category("Integration")]
        public class When_creating_a_site_with_http_and_https_binding : Iis7TaskSpecsContext
        {
            [Fact]
            public void It_should_bind_the_http_protocol_and_port()
            {
                AssertSiteBinding("http", 16007);
            }

            [Fact]
            public void It_should_bind_the_https_protocol_and_port()
            {
                AssertSiteBinding("https", 16008, CertificateThumbprint);
            }

            public override void Because()
            {
                Task.Bindings = new[]
                                    {
                                        new IisSiteBinding { Protocol = "http", Port = 16007 },
                                        new IisSiteBinding { Protocol = "https", Port = 16008, CertificateThumbPrint = CertificateThumbprint }
                                    };
                base.Because();
            }
        }

        [Category("Integration")]
        public class When_creating_a_site_that_already_exists : Iis7TaskSpecsContext
        {
            const int ExistingHttpsPort = 16016;
            const int PortToChange = 16014;
            const int PortToAdd = 16013;
            const int PortToPreserve = 16015;
            const int PortToRemove = 16012;

            [Fact]
            public void It_should_remove_undefined_bindings()
            {
                AssertNotSiteBinding(PortToRemove);
            }

            [Fact]
            public void It_should_add_newly_defined_bindings()
            {
                AssertSiteBinding("http", PortToAdd);
            }

            [Fact]
            public void It_should_update_protocol_on_redefined_bindings()
            {
                AssertSiteBinding("https", PortToChange);
            }

            [Fact]
            public void It_should_preserve_already_defined_bindings()
            {
                AssertSiteBinding("http", PortToPreserve);
            }

            public override void Because()
            {
                // Build existing site
                Task.Bindings = new[]
                                    {
                                        new IisSiteBinding { Protocol = "http", Port = PortToRemove },
                                        new IisSiteBinding { Protocol = "http", Port = PortToChange },
                                        new IisSiteBinding { Protocol = "http", Port = PortToPreserve },
                                        new IisSiteBinding { Protocol = "https", Port = ExistingHttpsPort, CertificateThumbPrint = CertificateThumbprint }
                                    };
                base.Because();

                WaitForIis();

                System.Diagnostics.Debug.WriteLine("Executing task again to update existing site.");
                Context();
                // Update it
                Task.Bindings = new[]
                                    {
                                        new IisSiteBinding { Protocol = "https", Port = PortToChange, CertificateThumbPrint = CertificateThumbprint },
                                        new IisSiteBinding { Protocol = "http", Port = PortToAdd },
                                        new IisSiteBinding { Protocol = "http", Port = PortToPreserve },
                                        new IisSiteBinding { Protocol = "https", Port = ExistingHttpsPort, CertificateThumbPrint = CertificateThumbprint }
                                    };
                base.Because();
            }
        }
    }
}
