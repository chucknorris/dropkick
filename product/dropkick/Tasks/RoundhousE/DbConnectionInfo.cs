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
            var datasource = Server;
            if (Instance.IsNotEmpty())
                datasource = @"{0}\{1}".FormatWith(Server, Instance);

            string credentials;
            if (UserName.IsEmpty())
            {
                credentials = "integrated security=sspi;";
            }
            else
            {
                credentials = "userId=";
                if (UserName.ShouldPrompt())
                    credentials += _prompt.Prompt("Database '{0}' UserName".FormatWith(DatabaseName));
                else
                    credentials += UserName;

                credentials += ";password=";
                if (Password.ShouldPrompt())
                    credentials += _prompt.Prompt("Database '{0}' Password".FormatWith(DatabaseName));
                else
                    credentials += Password;
            }

            return "data source={0};initial catalog={1};{2};"
                .FormatWith(datasource, DatabaseName, credentials);
        }

        private readonly PromptService _prompt;
    }
}