namespace dropkick.tests.Tasks.Iis
{
    using System;
    using dropkick.Tasks.Iis;
    using Microsoft.Web.Administration;
    using NUnit.Framework;

    [TestFixture]
    public class NewVirtualIis6DirTest
    {
        [Test, Explicit]
        public void Create_A_VirtualDiretory()
        {
            var task = new Iis6Task
                           {
                               PathOnServer = "E:\\FHLBApp\\FHLBank.Security.Web",
                               ServerName = "srvtest19",
                               VirtualDirectoryPath = "Test_dk",
                               WebsiteName = "Exchequer",
                               AppPoolName = "Test_dk"
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