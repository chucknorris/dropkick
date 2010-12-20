namespace dropkick.tests.Tasks.Console
{
    using dropkick.Tasks.CommandLine;
    using NUnit.Framework;
    using System;

    [TestFixture]
    public class RemoteTest
    {
        [Test,Explicit]
        public void Remote()
        {
            var task = new RemoteCommandLineTask("ping")
                           {
                               Args = "www.fhlbtopeka.com -n 5",
                               Machine = "SrvTestWeb01"
                           };

            var output = task.Execute();
            foreach (var item in output.Results)
            {
                Console.WriteLine(item.Message);
            }
        }
    }
}