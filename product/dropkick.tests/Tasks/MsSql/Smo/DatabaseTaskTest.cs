using System;
using System.Data.SqlClient;
using NUnit.Framework;
using dropkick.Tasks.MsSql;

namespace dropkick.tests.Tasks.MsSql.Smo
{
    [TestFixture]
    public class DatabaseTaskTest
    {
        [Test]
        public void CreatingDatabaseTest()
        {
            var target = new DatabaseTask();
            target.CreateIfDoesntExists = true;
            target.DbServer = @".\sqlexpress";
            string dbName = "test_" + Guid.NewGuid().ToString("N");
            target.DbName = dbName;
            target.Execute();

            using (var connection = new SqlConnection(@"Data Source=.\sqlexpress;;Integrated Security=SSPI;Initial Catalog=master"))
            {
                connection.Open();
                var cmd = new SqlCommand("select db_id('" + dbName +"')", connection);
                object value = cmd.ExecuteScalar();
                Assert.AreNotEqual(DBNull.Value, value);

                cmd = new SqlCommand("DROP DATABASE " + dbName, connection);
                cmd.ExecuteNonQuery();
            }

        }
    }
}