namespace dropkick.tests.Tasks.Iis
{
    using System;
    using System.IO;
    using dropkick.Tasks.Iis;
    using NUnit.Framework;

    [TestFixture]
    public class NewVirtualDirTest
    {
        [Test,Explicit]
        public void Create_A_VirtualDiretory()
        {
            if(Directory.Exists(".\\bob"))
                Directory.Delete(".\\bob", true);

            Directory.CreateDirectory(".\\bob");
            var task = new Iis7Task
                       {
                           PathOnServer = new DirectoryInfo(".\\dk_test"),
                           ServerName = Environment.MachineName,
                           VdirPath = "dk_test",
                           WebsiteName = "Default Web Site"
                       };
            var output = task.Execute();
            foreach (var item in output.Results)
            {
                Console.WriteLine(item.Message);
            }
        }
    }
}