namespace dropkick.tests.Tasks.Iis
{
    using System;
    using System.IO;
    using dropkick.Tasks.Iis;
    using NUnit.Framework;

    [TestFixture]
    public class NewVirtualDirTest
    {
        [Test]
        public void NAME()
        {
            if(Directory.Exists(".\\bob"))
                Directory.Delete(".\\bob", true);

            Directory.CreateDirectory(".\\bob");
            var task = new Iis7Task
                       {
                           PathOnServer = new DirectoryInfo(".\\bob"),
                           ServerName = Environment.MachineName,
                           VdirPath = "bob",
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