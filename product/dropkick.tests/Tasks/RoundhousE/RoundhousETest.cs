
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
            var task = new RoundhousETask(".", "SQL2005", "TestRoundhousE", true, @"E:\external projects\kaithos\roundhouse\db\SQLServer\TestRoundhousE", "TEST", true);
            var results = task.Execute();

            Assert.IsFalse(results.ContainsError());
        }
    }
}
