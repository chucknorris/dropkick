using System.Linq;
using System.Threading;
using Microsoft.Web.Administration;
using NUnit.Framework;
using dropkick.Tasks.Iis;

namespace dropkick.tests.Tasks.Iis
{
    [Category("Integration")]
    [TestFixture]
    public class NewIis7SiteTest
    {
        const string TestWebSiteName = "_DropKickTest_4789";

        [Test, Explicit]
        public void CreateSiteWithHttpAndHttpsBindings()
        {
            var task = new Iis7Task { WebsiteName = TestWebSiteName };
            task.Bindings = new[]
                                {
                                    new IisSiteBinding { Protocol = "http", Port = 16001 },
                                    new IisSiteBinding { Protocol = "https", Port = 16002 }
                                };

            var result = task.Execute();
            System.Diagnostics.Debug.WriteLine(result);
            Assert.IsFalse(result.ContainsError(), "Results of task execution contained an error.");

            assertSiteBinding("http", 16001);
        }

        static void assertSiteBinding(string protocol, int port)
        {
            using (var iis = ServerManager.OpenRemote("localhost"))
            {
                var site = iis.Sites[TestWebSiteName];
                Assert.IsNotNull(site, "Site '{0}' was not found.", TestWebSiteName);

                var binding = site.Bindings.Where(x => x.EndPoint.Port == port).FirstOrDefault();
                Assert.IsNotNull(binding, "Site '{0}' is not bound to port '{1}'", TestWebSiteName, port);
                Assert.AreEqual("http", binding.Protocol);
            }
        }

        [TestFixtureTearDown]
        protected void DeleteSite()
        {
            using (var iis = ServerManager.OpenRemote("localhost"))
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
    }
}
