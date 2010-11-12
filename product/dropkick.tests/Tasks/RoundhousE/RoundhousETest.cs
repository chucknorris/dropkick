
using dropkick.Tasks.RoundhousE;
using NUnit.Framework;

namespace dropkick.tests.Tasks.RoundhousE
{
    [TestFixture]
    [Category("Integration")]
    class RoundhousETest
    {
        [Test]
        public void TestRoundhousE()
        {
            var task = new RoundhousETask(".","Test","SQL2008",@".\scripts","TEST",true);
        }
    }
}
