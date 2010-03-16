namespace dropkick.tests.Tasks.MsSql
{
    using dropkick.Tasks.MsSql;
    using NUnit.Framework;

    [TestFixture]
    public class OutputSqlTest
    {
        [Test]
        public void NAME()
        {
            var t = new OutputSqlTask(".", "master");
            t.OutputSql = "SELECT 1 AS BOB";
            //TODO: friggin console output is hard to test
            var o = t.Execute();
        }
    }
}