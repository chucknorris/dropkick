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
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using DeploymentModel;

    public abstract class BaseSqlTask :
        Task
    {
        protected BaseSqlTask(string serverName, string databaseName)
        {
            ServerName = serverName;
            DatabaseName = databaseName;
        }

        public string ServerName { get; set; }
        public string DatabaseName { get; set; }

        public abstract string Name { get; }
        public abstract DeploymentResult VerifyCanRun();
        public abstract DeploymentResult Execute();


        public void TestConnectivity(DeploymentResult result)
        {
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
        }

        public IDbConnection GetConnection()
        {
            return new SqlConnection(GetConnectionString());
        }

        string GetConnectionString()
        {
            var cs = new SqlConnectionStringBuilder
                         {
                             DataSource = ServerName,
                             InitialCatalog = DatabaseName,
                             IntegratedSecurity = true
                         };

            return cs.ConnectionString;
        }

        public void ExecuteSqlWithNoReturn(string sql)
        {
            using (IDbConnection conn = GetConnection())
            {
                conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public T ExecuteSqlWithOneReturn<T>(string sql)
        {
            using (IDbConnection conn = GetConnection())
            {
                conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.CommandType = CommandType.Text;
                    return (T)cmd.ExecuteScalar();
                }
            }
        }
        public IList<T> ExecuteSqlWithListReturn<T>(string sql)
        {
            using (IDbConnection conn = GetConnection())
            {
                conn.Open();
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.CommandType = CommandType.Text;
                    var result = new List<T>();
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            result.Add((T)dr.GetValue(0));
                        }
                    }
                    return result;
                }
            }
        }
    }
}