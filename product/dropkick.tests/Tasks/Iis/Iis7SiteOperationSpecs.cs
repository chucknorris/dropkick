namespace dropkick.tests.Tasks.Iis
{
    using System.Threading;
    using dropkick.Tasks.Iis;
    using Microsoft.Web.Administration;
    using NUnit.Framework;

    public class Iis7SiteOperationSpecs
    {
        #region Nested type: Iis7SiteOperationsSpecsContext

        public abstract class Iis7SiteOperationsSpecsContext : TinySpec
        {
            public override void Context()
            {
                CreateSite();
            }

            public override void AfterObservations()
            {
                DeleteSite();
            }

            protected void CreateSite()
            {
                using (var iis = ServerManager.OpenRemote(WebServerName))
                {
                    Site = iis.Sites[SiteName];
                    if (Site == null)
                    {
                        Site = iis.Sites.Add(SiteName, @"C:\Temp", 16000);
                        iis.CommitChanges();
                        WaitForIis();
                    }
                }
            }

            protected void DeleteSite()
            {
                using (var iis = ServerManager.OpenRemote(WebServerName))
                {
                    var site = iis.Sites[SiteName];
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

            protected void SiteShouldNotExist()
            {
                using (var iis = ServerManager.OpenRemote(WebServerName))
                    Assert.IsNull(iis.Sites[SiteName]);
            }

            protected Iis7SiteOperationTask Task;
            protected Site Site { get; private set; }
            protected const string SiteName = "DropKickTestSite5775";
            protected const string WebServerName = "localhost";
        }

        #endregion

        #region Nested type: When_deleting_an_existing_site

        [Category("Integration")]
        public class When_stopping_a_running_application_pool : Iis7SiteOperationsSpecsContext
        {
            public override void Context()
            {
                base.Context();

                Task = new Iis7SiteOperationTask
                {
                    ServerName = WebServerName,
                    SiteName = SiteName,
                    Operation = Iis7SiteOperation.DeleteSite
                };

            }

            public override void Because()
            {
                Task.Execute().LogToConsole();
            }

            [Fact]
            public void the_site_should_be_deleted()
            {
                SiteShouldNotExist();
            }
        }

        #endregion
    }
}
