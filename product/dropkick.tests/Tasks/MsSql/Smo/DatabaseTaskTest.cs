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
            target.ScriptFile = @".\tasks\mssql\smo\createtable.sql";
            string dbName = "test_" + Guid.NewGuid().ToString("N");
            target.DbName = dbName;
            target.Execute();

            using (var connection = new SqlConnection(@"Data Source=.\sqlexpress;;Integrated Security=SSPI;Initial Catalog=master"))
            {
                connection.Open();
                using (var cmd = new SqlCommand("select db_id('" + dbName + "')", connection))
                {
                    object value = cmd.ExecuteScalar();
                    Assert.AreNotEqual(DBNull.Value, value);
                }
                connection.Close();
            }


            using (var connection = new SqlConnection(@"Data Source=.\sqlexpress;;Integrated Security=SSPI;Initial Catalog=" + dbName))
            {
                connection.Open();
                using (var cmd = new SqlCommand("SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  TABLE_NAME = 'TestTable'", connection))
                {
                    int intValue = (int)cmd.ExecuteScalar();
                    Assert.AreEqual(1, intValue);
                }
                connection.Close();
            }

            SqlConnection.ClearAllPools();
            using (var connection = new SqlConnection(@"Data Source=.\sqlexpress;;Integrated Security=SSPI;Initial Catalog=master"))
            {
                connection.Open();
                using (var cmd = new SqlCommand("DROP DATABASE " + dbName, connection))
                {
                    cmd.ExecuteNonQuery();
                }
                connection.Close();
            }

        }
    }
}