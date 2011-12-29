namespace dropkick.tests.Tasks.Iis
{
    using System;
    using dropkick.Tasks.Iis;
    using NUnit.Framework;

    [TestFixture]
    public class NewVirtualIis6DirTest
    {
        [Test, Explicit]
        public void Create_A_VirtualDiretory()
        {
            var task = new Iis6Task
                           {
                               PathOnServer = "C:\\Web\\FHLBank.Security.Web.Test",
                               ServerName = "JASON-W2K3-VM",
                               VirtualDirectoryPath = "Test_dk",
                               WebsiteName = "Default Web Site",
                               AppPoolName = "Test_dk"
                           };

            var output = task.Execute();
            foreach (var item in output.Results)
            {
                Console.WriteLine(item.Message);
            }
        }
    }
}