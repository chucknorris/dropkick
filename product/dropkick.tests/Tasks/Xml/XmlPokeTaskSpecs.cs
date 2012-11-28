namespace dropkick.tests.Tasks.Xml
{
    using System.Collections.Generic;
    using System.IO;
    using Context;
    using dropkick.DeploymentModel;
    using dropkick.Tasks.Xml;
    using FileSystem;
    using NUnit.Framework;
   using System.Xml.Linq;

    public class XmlPokeTaskSpecs
    {
        public abstract class XmlPokeTaskSpecsBase : TinySpec
        {
            protected DeploymentResult result;
            protected XmlPokeTask task;
            protected string file_path = @".\Tasks\Xml\Context\Test.xml";
            protected string file_path_insert = @".\Tasks\Xml\Context\TestInsert.xml";
            protected string file_path_with_namespace = @".\Tasks\Xml\Context\TestWithNamespace.xml";
            protected string file_path_insert_with_namespace = @".\Tasks\Xml\Context\TestInsertWithNamespace.xml";
            protected string file_path_TestAddingElmahToWebConfig = @".\Tasks\Xml\Context\TestAddingElmahToWebConfig.xml";

            public override void Because()
            {
                result = task.Execute();
            }

            [Fact]
            public void should_execute_successfully()
            {
                System.Console.WriteLine(result.ToString());
            }
        }


        [ConcernFor("Xml")]
        [Category("Integration")]
        public class when_updating_the_settings_in_an_xml_file : XmlPokeTaskSpecsBase
        {
            protected IDictionary<string,string> replacement_items = new Dictionary<string, string>();

            public override void Context()
            {
                replacement_items.Add("//configuration/connectionStrings/add[@name='bob']/@connectionString", "bob");
                replacement_items.Add("//configuration/connectionStrings/add[@name='bob']/@providerName", "System.Data.OleDb");
                task = new XmlPokeTask(file_path, replacement_items, new DotNetPath());
            }

            [Fact]
            public void should_have_updated_the_file()
            {
                string updated_text = File.ReadAllText(file_path);
                updated_text.ShouldBeEqualTo(XmlContextResult.Result);
            }
        }

        [ConcernFor("Xml")]
        [Category("Integration")]
        public class when_inserting_into_the_settings_in_an_xml_file : XmlPokeTaskSpecsBase {
           protected IDictionary<string, string> replacement_items = new Dictionary<string, string>();
           protected IDictionary<string, string> insert_items = new Dictionary<string, string>();

           public override void Context() {
              replacement_items.Add("//configuration/connectionStrings/add[@name='bob']/@connectionString", "bob");
              replacement_items.Add("//configuration/connectionStrings/add[@name='bob']/@providerName", "System.Data.OleDb");

              insert_items.Add("//configuration/connectionStrings/add[@name='newDB']/@connectionString", "newConnectionString");
              insert_items.Add("//configuration/connectionStrings/add[@name='newDB']/@providerName", "System.Data.SqlClient");

              insert_items.Add("//configuration/connectionStrings/add[@name='newConnection2']/@connectionString", "newConnectionString2");
              insert_items.Add("//configuration/connectionStrings/add[@name='newConnection2']/@providerName", "System.Data.OleDb");
                            
              task = new XmlPokeTask(file_path_insert, replacement_items, insert_items, new DotNetPath());
           }

           [Fact]
           public void should_have_updated_the_file() {
              string updated_text = File.ReadAllText(file_path_insert);
              updated_text.ShouldBeEqualTo(XmlContextResult.ResultInsert);
           }
        }


        [ConcernFor("Xml")]
        [Category("Integration")]
        public class when_updating_the_settings_in_an_xml_file_with_namespaces : XmlPokeTaskSpecsBase
        {
            protected IDictionary<string, string> replacement_items = new Dictionary<string, string>();
            protected IDictionary<string, string> namespace_prefixes = new Dictionary<string, string> {{"test", "http://example.com/schemas/test"}};

            public override void Context()
            {
                replacement_items.Add("//test:configuration/test:connectionStrings/test:add[@name='bob']/@connectionString", "bob");
                replacement_items.Add("//test:configuration/test:connectionStrings/test:add[@name='bob']/@providerName", "System.Data.OleDb");

                task = new XmlPokeTask(file_path_with_namespace, replacement_items, new DotNetPath(), namespace_prefixes);
            }

            [Fact]
            public void should_have_updated_the_file()
            {
                string updated_text = File.ReadAllText(file_path_with_namespace);
                updated_text.ShouldBeEqualTo(XmlContextResult.ResultWithNamespace);
            }
        }

        [ConcernFor("Xml")]
        [Category("Integration")]
        public class when_inserting_into_the_settings_in_an_xml_file_with_namespaces : XmlPokeTaskSpecsBase {
           protected IDictionary<string, string> replacement_items = new Dictionary<string, string>();
           protected IDictionary<string, string> insert_items = new Dictionary<string, string>();
           protected IDictionary<string, string> namespace_prefixes = new Dictionary<string, string> { { "test", "http://example.com/schemas/test" } };

           public override void Context() {
              replacement_items.Add("//test:configuration/test:connectionStrings/test:add[@name='bob']/@connectionString", "bob");
              replacement_items.Add("//test:configuration/test:connectionStrings/test:add[@name='bob']/@providerName", "System.Data.OleDb");

              insert_items.Add("//test:configuration/test:connectionStrings/test:add[@name='newDB']/@connectionString", "newConnectionString");
              insert_items.Add("//test:configuration/test:connectionStrings/test:add[@name='newDB']/@providerName", "System.Data.SqlClient");

              insert_items.Add("//test:configuration/test:connectionStrings/test:add[@name='newConnection2']/@connectionString", "newConnectionString2");
              insert_items.Add("//test:configuration/test:connectionStrings/test:add[@name='newConnection2']/@providerName", "System.Data.OleDb");

              task = new XmlPokeTask(file_path_insert_with_namespace, replacement_items, insert_items, new DotNetPath(), namespace_prefixes);
           }

           [Fact]
           public void should_have_updated_the_file() {
              string updated_text = File.ReadAllText(file_path_insert_with_namespace);
              updated_text.ShouldBeEqualTo(XmlContextResult.ResultInsertWithNamespace);
           }
        }

        [ConcernFor("Xml")]
        [Category("Integration")]
        public class when_adding_Elmah_to_web_config_file : XmlPokeTaskSpecsBase {
           protected IDictionary<string, string> replacement_items = new Dictionary<string, string>();
           protected IDictionary<string, string> insert_items = new Dictionary<string, string>();

           public override void Context() {
              // = = = adding sectionGroups

              var sectionGroups = new Dictionary<string, string>{
                  {"//configuration/configSections/sectionGroup[@name='elmah']/section[@name='security']", "Elmah.SecuritySectionHandler, Elmah"},
                  {"//configuration/configSections/sectionGroup[@name='elmah']/section[@name='errorLog']", "Elmah.ErrorLogSectionHandler, Elmah"},
                  {"//configuration/configSections/sectionGroup[@name='elmah']/section[@name='errorMail']", "Elmah.ErrorMailSectionHandler, Elmah"},
                  {"//configuration/configSections/sectionGroup[@name='elmah']/section[@name='errorFilter']", "Elmah.ErrorFilterSectionHandler, Elmah"}
              };

              foreach(var sg in sectionGroups) {
                 insert_items.Add(sg.Key + "/@requirePermission", "false");
                 insert_items.Add(sg.Key + "/@type", sg.Value);
              }

              // = = = adding the following to system.web/httpModules
              //<add name="ErrorLog" type="Elmah.ErrorLogModule, Elmah" />
              //<add name="ErrorMail" type="Elmah.ErrorMailModule, Elmah" />
              //<add name="ErrorFilter" type="Elmah.ErrorFilterModule, Elmah" />
              string systemWeb_httpModulesXpath = @"//configuration/system.web/httpModules";
              insert_items.Add(systemWeb_httpModulesXpath + "/add[@name='ErrorLog']/@type", "Elmah.ErrorLogModule, Elmah");
              insert_items.Add(systemWeb_httpModulesXpath + "/add[@name='ErrorMail']/@type", "Elmah.ErrorMailModule, Elmah");
              insert_items.Add(systemWeb_httpModulesXpath + "/add[@name='ErrorFilter']/@type", "Elmah.ErrorFilterModule, Elmah");


              // = = = adding the following to system.webServer/modules:
              //<add name="ErrorLog" type="Elmah.ErrorLogModule, Elmah" preCondition="managedHandler" />
              //<add name="ErrorMail" type="Elmah.ErrorMailModule, Elmah" preCondition="managedHandler" />
              //<add name="ErrorFilter" type="Elmah.ErrorFilterModule, Elmah" preCondition="managedHandler" />

              string systemWebServer_modulesXpath = @"//configuration/system.webServer/modules";
              insert_items.Add(systemWebServer_modulesXpath + "/add[@name='ErrorLog']/@type", "Elmah.ErrorLogModule, Elmah");
              insert_items.Add(systemWebServer_modulesXpath + "/add[@name='ErrorLog']/@preCondition", "managedHandler");
              insert_items.Add(systemWebServer_modulesXpath + "/add[@name='ErrorMail']/@type", "Elmah.ErrorMailModule, Elmah");
              insert_items.Add(systemWebServer_modulesXpath + "/add[@name='ErrorMail']/@preCondition", "managedHandler");
              insert_items.Add(systemWebServer_modulesXpath + "/add[@name='ErrorFilter']/@type", "Elmah.ErrorFilterModule, Elmah");
              insert_items.Add(systemWebServer_modulesXpath + "/add[@name='ErrorFilter']/@preCondition", "managedHandler");


              // = = = adding elmah config part
              //<elmah>
              //   <security allowRemoteAccess="1" />
              //   <errorLog type="Elmah.XmlFileErrorLog, Elmah" logPath="~/App_Data/Logs" />    
              //   <errorFilter>
              //   <test>
              //      <and>
              //         <equal binding="HttpStatusCode" value="404" type="Int32" />
              //      </and>
              //   </test>
              //   </errorFilter>
              //</elmah>
              string elmahXpath = @"//configuration/elmah";
              insert_items.Add(elmahXpath + "/security/@allowRemoteAccess", "1");
              insert_items.Add(elmahXpath + "/errorLog/@type", "Elmah.XmlFileErrorLog, Elmah");
              insert_items.Add(elmahXpath + "/errorLog/@logPath", "~/App_Data/Logs");

              string equalXpath = elmahXpath + "/errorFilter/test/and/equal";
              insert_items.Add(equalXpath + "/@binding", "HttpStatusCode");
              insert_items.Add(equalXpath + "/@value", "404");
              insert_items.Add(equalXpath + "/@type", "Int32");

              // = = = adding elmah.axd securely
              //<location path="elmah.axd">
              //    <system.web>
              //      <authorization>
              //         <allow users="testDomain\admin, testDomain\otherAdmin"/>
              //         <deny users="*"/>
              //      </authorization>
              //      <httpHandlers>
              //         <add verb="POST,GET,HEAD" path="elmah.axd" type="Elmah.ErrorLogPageFactory, Elmah" />
              //      </httpHandlers>
              //    </system.web>
              //   <system.webServer>
              //    <security>
              //       <authorization>
              //          <clear/>
              //          <add users="testDomain\admin, testDomain\otherAdmin" accessType="Allow"/>
              //       </authorization>
              //    </security>
              //    <handlers>
              //       <add name="Elmah" path="elmah.axd" verb="POST,GET,HEAD" type="Elmah.ErrorLogPageFactory, Elmah" preCondition="integratedMode" />
              //    </handlers>
              //   </system.webServer>
              //</location>
              string locationXpath = @"//configuration/location";
              string ALLOWED_USERS = @"testDomain\admin, testDomain\otherAdmin";
              string ELMAH_AXD = @"elmah.axd";
              string ELMAH_ERRORLOGPAGEFACTORY = @"Elmah.ErrorLogPageFactory, Elmah";
              string GET_POST_HEAD = @"POST,GET,HEAD";
              insert_items.Add(locationXpath + "/@path", ELMAH_AXD);

              //adding stuff to system.web
              insert_items.Add(locationXpath + "/system.web/authorization/allow/@users", ALLOWED_USERS);
              insert_items.Add(locationXpath + "/system.web/authorization/deny/@users", "*");
              insert_items.Add(locationXpath + "/system.web/httpHandlers/add/@verb", GET_POST_HEAD);
              insert_items.Add(locationXpath + "/system.web/httpHandlers/add/@path", ELMAH_AXD);
              insert_items.Add(locationXpath + "/system.web/httpHandlers/add/@type", ELMAH_ERRORLOGPAGEFACTORY);

              //addign stuff to system.webserver
              insert_items.Add(locationXpath + "/system.webServer/security/authorization/clear", "");
              insert_items.Add(locationXpath + "/system.webServer/security/authorization/add/@users", ALLOWED_USERS);
              insert_items.Add(locationXpath + "/system.webServer/security/authorization/add/@accessType", "Allow");
              insert_items.Add(locationXpath + "/system.webServer/handlers/add/@name", "Elmah");
              insert_items.Add(locationXpath + "/system.webServer/handlers/add/@path", ELMAH_AXD);
              insert_items.Add(locationXpath + "/system.webServer/handlers/add/@verb", GET_POST_HEAD);
              insert_items.Add(locationXpath + "/system.webServer/handlers/add/@type", ELMAH_ERRORLOGPAGEFACTORY);
              insert_items.Add(locationXpath + "/system.webServer/handlers/add/@preCondition", "integratedMode");


              task = new XmlPokeTask(file_path_TestAddingElmahToWebConfig, replacement_items, insert_items, new DotNetPath());
           }

           [Fact]
           public void should_have_updated_the_file() {
              //string updated_text = File.ReadAllText(file_path_TestAddingElmahToWebConfig);
              var updatedXml = XElement.Load(file_path_TestAddingElmahToWebConfig);
              var expectedElmahXml = XElement.Load(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(XmlContextResult.ResultWebConfigWithElmah)));
                            
              //comparing two xml files of this size as plain text would be a "bit" fragile...
              //updated_text.ShouldBeEqualTo(XmlContextResult.ResultElmahWithNamespace);
              Assert.True(System.Xml.Linq.XNode.DeepEquals(updatedXml, expectedElmahXml));
           }
        }
    }
}