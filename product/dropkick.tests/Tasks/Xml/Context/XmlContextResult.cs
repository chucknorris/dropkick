namespace dropkick.tests.Tasks.Xml.Context
{
    public static class XmlContextResult
    {
        public static string Result = @"<?xml version=""1.0"" encoding=""utf-8""?>
<configuration>
  <connectionStrings>
    <add name=""bob"" connectionString=""bob"" providerName=""System.Data.OleDb"" />
    <add name=""nancy"" connectionString=""Data Source=(local);Initial Catalog=Nancy;Integrated Security=True;Pooling=false;Connection Timeout=600"" providerName=""System.Data.SqlClient"" />
  </connectionStrings>
</configuration>";

        public static string ResultWithNamespace = @"<?xml version=""1.0"" encoding=""utf-8""?>
<configuration xmlns=""http://example.com/schemas/test"">
  <connectionStrings>
    <add name=""bob"" connectionString=""bob"" providerName=""System.Data.OleDb"" />
    <add name=""nancy"" connectionString=""Data Source=(local);Initial Catalog=Nancy;Integrated Security=True;Pooling=false;Connection Timeout=600"" providerName=""System.Data.SqlClient"" />
  </connectionStrings>
</configuration>";

        public static string ResultInsert = @"<?xml version=""1.0"" encoding=""utf-8""?>
<configuration>
  <connectionStrings>
    <add name=""bob"" connectionString=""bob"" providerName=""System.Data.OleDb"" />
    <add name=""nancy"" connectionString=""Data Source=(local);Initial Catalog=Nancy;Integrated Security=True;Pooling=false;Connection Timeout=600"" providerName=""System.Data.SqlClient"" />
    <add name=""newDB"" connectionString=""newConnectionString"" providerName=""System.Data.SqlClient"" />
    <add name=""newConnection2"" connectionString=""newConnectionString2"" providerName=""System.Data.OleDb"" />
  </connectionStrings>
</configuration>";

        public static string ResultInsertWithNamespace = @"<?xml version=""1.0"" encoding=""utf-8""?>
<configuration xmlns=""http://example.com/schemas/test"">
  <connectionStrings>
    <add name=""bob"" connectionString=""bob"" providerName=""System.Data.OleDb"" />
    <add name=""nancy"" connectionString=""Data Source=(local);Initial Catalog=Nancy;Integrated Security=True;Pooling=false;Connection Timeout=600"" providerName=""System.Data.SqlClient"" />
    <add name=""newDB"" connectionString=""newConnectionString"" providerName=""System.Data.SqlClient"" />
    <add name=""newConnection2"" connectionString=""newConnectionString2"" providerName=""System.Data.OleDb"" />
  </connectionStrings>
</configuration>";

        public static string ResultWebConfigWithElmah = @"<?xml version=""1.0"" encoding=""utf-8""?>
<configuration>
  <connectionStrings>
  </connectionStrings>
  <configSections>
     <sectionGroup name=""elmah"">
       <section name=""security"" requirePermission=""false"" type=""Elmah.SecuritySectionHandler, Elmah"" />
       <section name=""errorLog"" requirePermission=""false"" type=""Elmah.ErrorLogSectionHandler, Elmah"" />
       <section name=""errorMail"" requirePermission=""false"" type=""Elmah.ErrorMailSectionHandler, Elmah"" />
       <section name=""errorFilter"" requirePermission=""false"" type=""Elmah.ErrorFilterSectionHandler, Elmah"" />
     </sectionGroup>
  </configSections>

  <system.web>
     <httpModules>
       <add name=""ErrorLog"" type=""Elmah.ErrorLogModule, Elmah"" />
       <add name=""ErrorMail"" type=""Elmah.ErrorMailModule, Elmah"" />
       <add name=""ErrorFilter"" type=""Elmah.ErrorFilterModule, Elmah"" />
     </httpModules>
  </system.web>

  <system.webServer>
    <modules>
      <add name=""ErrorLog"" type=""Elmah.ErrorLogModule, Elmah"" preCondition=""managedHandler"" />
      <add name=""ErrorMail"" type=""Elmah.ErrorMailModule, Elmah"" preCondition=""managedHandler"" />
      <add name=""ErrorFilter"" type=""Elmah.ErrorFilterModule, Elmah"" preCondition=""managedHandler"" />
    </modules>
  </system.webServer>

  <elmah>
    <security allowRemoteAccess=""1"" />
    <errorLog type=""Elmah.XmlFileErrorLog, Elmah"" logPath=""~/App_Data/Logs"" />    
    <errorFilter>
      <test>
        <and>
          <equal binding=""HttpStatusCode"" value=""404"" type=""Int32"" />
        </and>
      </test>
    </errorFilter>
  </elmah>

  <location path=""elmah.axd"">
    <system.web>
      <authorization>        
        <allow users=""testDomain\admin, testDomain\otherAdmin""/>
        <deny users=""*""/>
      </authorization>
      <httpHandlers>
        <add verb=""POST,GET,HEAD"" path=""elmah.axd"" type=""Elmah.ErrorLogPageFactory, Elmah"" />
      </httpHandlers>
    </system.web>    
    <system.webServer>
      <security>
        <authorization>
          <clear></clear>
          <add users=""testDomain\admin, testDomain\otherAdmin"" accessType=""Allow""/>
        </authorization>
      </security>
      <handlers>
        <add name=""Elmah"" path=""elmah.axd"" verb=""POST,GET,HEAD"" type=""Elmah.ErrorLogPageFactory, Elmah"" preCondition=""integratedMode"" />
      </handlers>
    </system.webServer>
  </location>
</configuration>";
    }
}
