// Copyright 2007-2010 The Apache Software Foundation.
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace dropkick.Tasks.MsSql
{
    using System;
    using System.Data;
    using System.Text;
    using DeploymentModel;

    public class OutputSqlTask :
        BaseSqlTask
    {
        public OutputSqlTask(string serverName, string databaseName) : base(serverName, databaseName)
        {
        }

        public override string Name
        {
            get { return string.Format("SQL TASK on server '{0}' for database '{1}'", ServerName, DatabaseName); }
        }

        public string OutputSql { get; set; }

        public override DeploymentResult VerifyCanRun()
        {
            var result = new DeploymentResult();

            //can I connect to the server?

            IDbConnection conn = null;
            try
            {
                conn = GetConnection();
                conn.Open();
                result.AddGood("I can talk to the database");
            }
            catch (Exception)
            {
                result.AddAlert("I cannot open the connection");
                throw;
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }

            //can I connect to the database?
            if (OutputSql != null)
                result.AddAlert(string.Format("I will run the sql '{0}'", OutputSql));


            return result;
        }

        public override DeploymentResult Execute()
        {
            using (IDbConnection conn = GetConnection())
            {
                conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = OutputSql;
                    cmd.CommandType = CommandType.Text;

                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        PrintDataReaderToConsole(dr);
                    }
                }
            }

            return new DeploymentResult();
        }

        void PrintDataReaderToConsole(IDataReader dr)
        {
            var sb = new StringBuilder();
            foreach (DataRow c in dr.GetSchemaTable().Rows)
            {
                sb.AppendFormat("{0} |", c[0]);
            }
            sb.Length = sb.Length - 2;
            Console.WriteLine(sb.ToString());

            while (dr.Read())
            {
                Console.WriteLine(dr[0].ToString());
            }
        }
    }
}