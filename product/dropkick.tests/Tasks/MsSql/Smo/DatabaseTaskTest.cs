﻿using System;
using System.Data.SqlClient;
using System.Text;
using NUnit.Framework;
using dropkick.Tasks.MsSql.Smo;

namespace dropkick.tests.Tasks.MsSql.Smo
{
    [TestFixture]
    public class DatabaseTaskTest
    {
        private string TestDb { get; set; }
        private string TestConnectionStringForMasterDb { get; set; }
        private string TestDbServer { get; set; }

        [SetUp]
        public void SetUp()
        {
            TestDb = "test_" + Guid.NewGuid().ToString("N");
            TestConnectionStringForMasterDb =
                System.Configuration.ConfigurationManager.AppSettings["TestConnectionStringForMasterDb"];
            var builder = new SqlConnectionStringBuilder(TestConnectionStringForMasterDb);
            TestDbServer = builder.DataSource;
        }

        [TearDown]
        public void TearDown()
        {
            DropTestDb();
        }

        [Test]
        public void DatabaseCreateIfDoesntExistsTest()
        {
            var target = new DatabaseTask();
            target.OpeningOptions = OpeningOptions.CreateIfDoesntExists;
            target.DbServer = TestDbServer;
            target.DbName = TestDb;
            target.ScriptFiles.Add(@".\tasks\mssql\smo\newtable.sql");
            target.CreateScriptFiles.Add(@".\tasks\mssql\smo\newtable2.sql");
            target.Execute();

            Assert.AreEqual(true, DbExists());
            Assert.AreEqual(true, TableExists("NewTable"));
            Assert.AreEqual(true, TableExists("NewTable2"));
        }

        [Test]
        [ExpectedException(typeof(Exception))]
        public void DatabaseFailIfDoesntExistsExceptionTest()
        {
            var target = new DatabaseTask();
            target.OpeningOptions = OpeningOptions.FailIfDoesntExists;
            target.DbServer = TestDbServer;
            target.DbName = TestDb;
            target.ScriptFiles.Add(@".\tasks\mssql\smo\newtable.sql");
            target.Execute();
        }

        [Test]
        public void DatabaseFailIfDoesntExistsTest()
        {
            CreateTestDb();

            var target = new DatabaseTask();
            target.OpeningOptions = OpeningOptions.FailIfDoesntExists;
            target.DbServer = TestDbServer;
            target.DbName = TestDb;
            target.ScriptFiles.Add(@".\tasks\mssql\smo\newtable.sql");
            target.CreateScriptFiles.Add(@".\tasks\mssql\smo\newtable2.sql");
            target.Execute();

            Assert.AreEqual(true, DbExists());
            Assert.AreEqual(true, TableExists("NewTable"));
            Assert.AreEqual(false, TableExists("NewTable2"));
        }

        [Test]
        public void DatabaseDropTest()
        {
            CreateTestDb();
            Assert.AreEqual(true, DbExists());

            var target = new DatabaseTask();
            target.OpeningOptions = OpeningOptions.FailIfDoesntExists;
            target.DbServer = TestDbServer;
            target.DbName = TestDb;
            target.ScriptFiles.Add(@".\tasks\mssql\smo\newtable.sql");
            target.CreateScriptFiles.Add(@".\tasks\mssql\smo\newtable2.sql");
            target.DropDb = true;
            target.Execute();

            Assert.AreEqual(false, DbExists());
        }

        private bool TableExists(string tableName)
        {
            bool result = false;
            using (var connection = new SqlConnection(TestConnectionStringForMasterDb))
            {
                connection.Open();
                var cmdText = new StringBuilder();
                cmdText.AppendFormat("USE {0}; ", TestDb);
                cmdText.AppendFormat("SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  TABLE_NAME = '{0}'", tableName);
                using (var cmd = new SqlCommand(cmdText.ToString(),connection))
                {
                    result = ((int)cmd.ExecuteScalar()) == 1;
                }
                connection.Close();
            }
            return result;
        }

        private bool DbExists()
        {
            bool result;
            using (var connection = new SqlConnection(TestConnectionStringForMasterDb))
            {
                connection.Open();
                using (var cmd = new SqlCommand(String.Format("SELECT db_id('{0}')", TestDb), connection))
                {
                    object value = cmd.ExecuteScalar();
                    result = value != DBNull.Value;
                }
                connection.Close();
            }
            return result;
        }

        private void CreateTestDb()
        {
            using (var connection = new SqlConnection(TestConnectionStringForMasterDb))
            {
                connection.Open();
                using (var cmd = new SqlCommand(String.Format("CREATE DATABASE {0}", TestDb), connection))
                {
                    cmd.ExecuteNonQuery();
                }
                using (var cmd = new SqlCommand(String.Format("USE {0}; CREATE TABLE OldTable(Id int NOT NULL)", TestDb), connection))
                {
                    cmd.ExecuteNonQuery();
                }
                connection.Close();
            }            
        }

        private void DropTestDb()
        {
            SqlConnection.ClearAllPools(); // neccessary to be able to drop the database because 'real' connection to the test db is still open
            using (var connection = new SqlConnection(TestConnectionStringForMasterDb))
            {
                connection.Open();
                using (var cmd = new SqlCommand(String.Format("IF DB_ID (N'{0}') IS NOT NULL DROP DATABASE {0}", TestDb), connection))
                {
                    cmd.ExecuteNonQuery();
                }
                connection.Close();
            }
        }

    }
}