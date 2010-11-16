
using dropkick.Tasks.RoundhousE;
using NUnit.Framework;

namespace dropkick.tests.Tasks.RoundhousE
{
    [TestFixture]
    [Category("Integration")]
    public class RoundhousETest
    {
        [Test]
        public void TestRoundhousE()
        {
            var task = new RoundhousETask(".", "Test", "SQL2008", @"D:\Development\roundhouse\db\scripts", "TEST", true);
            task.Execute();
        }
    }
}
