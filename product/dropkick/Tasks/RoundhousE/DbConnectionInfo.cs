using System.Data.SqlClient;

using dropkick.Prompting;

using Magnum.Extensions;

namespace dropkick.Tasks.RoundhousE
{
    public class DbConnectionInfo
    {
        public string Server { get; set; }
        public string Instance { get; set; }
        public string DatabaseName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public DbConnectionInfo()
        {
            _prompt = new ConsolePromptService();
        }

        public DbConnectionInfo(PromptService prompt)
        {
            _prompt = prompt;
        }

        public bool WillPromptForUserName()
        {
            return UserName.IsNotEmpty() && UserName.ShouldPrompt();
        }

        public bool WillPromptForPassword()
        {
            return UserName.IsNotEmpty() && Password.ShouldPrompt();
        }

        public string BuildConnectionString()
        {

            var builder = new SqlConnectionStringBuilder{
                InitialCatalog = DatabaseName, DataSource = Server
            };

            if (Instance.IsNotEmpty())
            {
                builder.DataSource = @"{0}\{1}".FormatWith(Server, Instance);
            }

            if (UserName.IsEmpty())
            {
                builder.IntegratedSecurity = true;
            }
            else
            {
                builder.IntegratedSecurity = false; 
                if (UserName.ShouldPrompt())
                    builder.UserID = _prompt.Prompt("Database '{0}' UserName".FormatWith(DatabaseName));
                else
                    builder.UserID = UserName;

                if (Password.ShouldPrompt())
                    builder.Password = _prompt.Prompt("Database '{0}' Password".FormatWith(DatabaseName));
                else
                    builder.Password = Password;
            }

            return builder.ConnectionString;
        }

        private readonly PromptService _prompt;
    }
}