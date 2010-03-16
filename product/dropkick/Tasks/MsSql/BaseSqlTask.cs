namespace dropkick.Tasks.MsSql
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using Configuration.Dsl;
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

        private string GetConnectionString()
        {
            var cs = new SqlConnectionStringBuilder
                         {
                             DataSource = ServerName,
                             InitialCatalog = DatabaseName,
                             IntegratedSecurity = true
                         };

            return cs.ConnectionString;
        }
    }
}