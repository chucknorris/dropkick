namespace dropkick.tests.Tasks.Xml.Context
{
    public static class XmlContextResult
    {
        public static string Result = @"<?xml version=""1.0"" encoding=""utf-8""?>
<configuration>
  <connectionStrings>
    <add name=""bob"" connectionString=""bob"" providerName=""System.Data.SqlClient"" />
    <add name=""nancy"" connectionString=""Data Source=(local);Initial Catalog=Nancy;Integrated Security=True;Pooling=false;Connection Timeout=600"" providerName=""System.Data.SqlClient"" />
  </connectionStrings>
</configuration>";

        public static string ResultWithNamespace = @"<?xml version=""1.0"" encoding=""utf-8""?>
<configuration xmlns=""http://example.com/schemas/test"">
  <connectionStrings>
    <add name=""bob"" connectionString=""bob"" providerName=""System.Data.SqlClient"" />
    <add name=""nancy"" connectionString=""Data Source=(local);Initial Catalog=Nancy;Integrated Security=True;Pooling=false;Connection Timeout=600"" providerName=""System.Data.SqlClient"" />
  </connectionStrings>
</configuration>";

    }
}
