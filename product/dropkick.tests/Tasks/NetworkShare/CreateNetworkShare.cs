namespace dropkick.tests.Tasks.NetworkShare
{
    using System;
    using System.IO;
    using dropkick.Tasks.NetworkShare;
    using NUnit.Framework;

    [TestFixture]
    [Category("Integration")]
    public class CreateNetworkShare
    {
        [Test][Explicit]
        public void Execute()
        {
            if(Directory.Exists(".\\temp"))
                Directory.CreateDirectory(".\\temp");

            var t = new FolderShareTask();
            t.PointingTo = @".\temp";
            t.ShareName = "dk_test";
            t.Server = Environment.MachineName;
            
            t.Execute();
        }

        [Test][Explicit]
        public void Verify()
        {
            var t = new FolderShareTask();
            t.PointingTo = @"C:\temp";
            t.ShareName = "dk_test";
            t.Server = Environment.MachineName;

            var r = t.VerifyCanRun();
        }
    }
}