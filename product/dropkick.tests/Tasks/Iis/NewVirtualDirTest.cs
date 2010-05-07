namespace dropkick.tests.Tasks.Iis
{
    using System;
    using System.IO;
    using dropkick.Tasks.Iis;
    using Microsoft.Web.Administration;
    using NUnit.Framework;

    [TestFixture]
    public class NewVirtualDirTest
    {
        [Test,Explicit]
        public void Create_A_VirtualDiretory()
        {
            var task = new Iis7Task
                       {
                           PathOnServer = new DirectoryInfo("E:\\FHLBApp\\FHLBank.Security.Web"),
                           ServerName = "SrvTestWeb01",
                           VdirPath = "FHLBSecurity",
                           WebsiteName = "FHLB",
                           AppPoolName = "FHLBSecurity"

                       };
            var output = task.Execute();
            foreach (var item in output.Results)
            {
                Console.WriteLine(item.Message);
            }
        }

        [Test, Explicit]
        public void Create_An_AppPool()
        {
            var iis = ServerManager.OpenRemote("SrvTestWeb01");
            iis.ApplicationPools.Add("MATTYB");
            
            iis.CommitChanges();
        }
    }
}