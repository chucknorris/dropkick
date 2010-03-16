namespace dropkick.Tasks.Dsn
{
    public class DsnDriver
    {
        public static DsnDriver Access()
        {
            return new DsnDriver(){Value = "Microsoft Access Driver (*.MDB)\0"};
        }

        public static DsnDriver Sql()
        {
            return new DsnDriver(){Value="SQL Server"};
        }

        public string Value { get; private set; }
    }
}