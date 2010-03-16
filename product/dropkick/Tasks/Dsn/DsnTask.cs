namespace dropkick.Tasks.Dsn
{
    using System;
    using System.Runtime.InteropServices;
    using System.Threading;
    using Configuration.Dsl;
    using DeploymentModel;

    public class DsnTask :
        Task
    {
        readonly DsnAction _action;
        readonly string _databaseName;
        readonly DsnDriver _driver;
        readonly string _dsnName;
        readonly string _serverName;

        public DsnTask(string serverName, string dsnName, DsnAction action, DsnDriver driver, string databaseName)
        {
            _serverName = serverName;
            _databaseName = databaseName;
            _dsnName = dsnName;
            _action = action;
            _driver = driver;
        }



        public string Name
        {
            get { return "DSN: {0}".FormatWith(_dsnName); }
        }

        public DeploymentResult VerifyCanRun()
        {
            var result = new DeploymentResult();

            VerifyInAdministratorRole(result);

            return result;
        }

        public DeploymentResult Execute()
        {
            var result = new DeploymentResult();

            try
            {
                bool value = SQLConfigDataSource((IntPtr) 0, (int) _action, _driver.Value, "SERVER={0}\0DSN={1}\0DESCRIPTION=NewDSN\0DATABASE={2}\0TRUSTED_CONNECTION=YES".FormatWith(_serverName, _dsnName, _databaseName));
                result.AddGood("Created DSN");
            }
            catch (Exception ex)
            {
                result.AddError("Failed to create DSN", ex);
            }


            return result;
        }


        [DllImport("ODBCCP32.dll")]
        static extern bool SQLConfigDataSource(IntPtr parent, int request, string driver, string attributes);

        void VerifyInAdministratorRole(DeploymentResult result)
        {
            if (Thread.CurrentPrincipal.IsInRole("Administrator"))
            {
                result.AddAlert("You are not in the Administrator role");
            }
            else
            {
                result.AddGood("You are in the Administrator role");
            }
        }
    }
}