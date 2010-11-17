
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
            var task = new RoundhousETask(".", "Test", "SQL2008", @"d:\development\roundhouse\db\SQLServer\TestRoundhousE", "TEST", true);
            var results = task.Execute();

            Assert.IsFalse(results.ContainsError());
        }
    }
}
