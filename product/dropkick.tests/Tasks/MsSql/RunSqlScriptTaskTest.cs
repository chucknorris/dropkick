namespace dropkick.tests.Tasks.MsSql
{
    using dropkick.Tasks.MsSql;
    using NUnit.Framework;

    [TestFixture]
    public class RunSqlScriptTaskTest
    {
        [Test]
        public void NAME()
        {
            var t = new RunSqlScriptTask(".", "master");
            t.ScriptToRun = @".\tasks\mssql\test.sql";
            t.Execute();
        }
    }
}