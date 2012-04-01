namespace dropkick.tests.Tasks.Xml {
   using System;
   using System.Collections.Generic;
   using System.IO;
   using System.Xml.Linq;
   using Context;
   using dropkick.DeploymentModel;
   using dropkick.Tasks.Xml;
   using FileSystem;
   using NUnit.Framework;

   public class XmlPokeTaskSpecs {
      public abstract class XmlPokeTaskSpecsBase : TinySpec {
         protected DeploymentResult result;
         protected XmlPokeTask task;
         protected string file_path = @".\Tasks\Xml\Context\Test.xml";
         protected string file_path_insert = @".\Tasks\Xml\Context\TestInsert.xml";
         protected string file_path_insert_shouldBeFirst = @".\Tasks\Xml\Context\TestInsert_shouldBeFirst.xml";
         protected string file_path_with_namespace = @".\Tasks\Xml\Context\TestWithNamespace.xml";
         protected string file_path_insert_with_namespace = @".\Tasks\Xml\Context\TestInsertWithNamespace.xml";
         protected string file_path_TestAddingElmahToWebConfig = @".\Tasks\Xml\Context\TestAddingElmahToWebConfig.xml";
         protected string file_path_TestAddingElmahToWebConfigWithNamespace = @".\Tasks\Xml\Context\TestAddingElmahToWebConfigWithNamespace.xml";
         protected string file_path_TestAddingElmahToWebConfigWithNamespace__fails_if_namespace_not_set_properly = @".\Tasks\Xml\Context\TestAddingElmahToWebConfigWithNamespace__fails_if_namespace_not_set_properly.xml";

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
      public class when_updating_the_settings_in_an_xml_file : XmlPokeTaskSpecsBase {
         protected IDictionary<string, string> replacement_items = new Dictionary<string, string>();

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
         protected IDictionary<string, Tuple<string, bool>> insert_items = new Dictionary<string, Tuple<string, bool>>();

         public override void Context()
         {
            replacement_items.Add("//configuration/connectionStrings/add[@name='bob']/@connectionString", "bob");
            replacement_items.Add("//configuration/connectionStrings/add[@name='bob']/@providerName", "System.Data.OleDb");

            insert_items.Add("//configuration/connectionStrings/add[@name='newDB']/@connectionString", Tuple.Create("newConnectionString", false));
            insert_items.Add("//configuration/connectionStrings/add[@name='newDB']/@providerName", Tuple.Create("System.Data.SqlClient", false));

            insert_items.Add("//configuration/connectionStrings/add[@name='newConnection2']/@connectionString", Tuple.Create("newConnectionString2", false));
            insert_items.Add("//configuration/connectionStrings/add[@name='newConnection2']/@providerName", Tuple.Create("System.Data.OleDb", false));

            task = new XmlPokeTask(file_path_insert, replacement_items, insert_items, new DotNetPath());
         }

         [Fact]
         public void should_have_updated_the_file()
         {
            string updated_text = File.ReadAllText(file_path_insert);
            updated_text.ShouldBeEqualTo(XmlContextResult.ResultInsert);
         }
      }

      [ConcernFor("Xml")]
      [Category("Integration")]
      public class when_inserting_into_the_settings_in_an_xml_file_using_shouldBeFirst : XmlPokeTaskSpecsBase {
         protected IDictionary<string, string> replacement_items = new Dictionary<string, string>();
         protected IDictionary<string, Tuple<string, bool>> insert_items = new Dictionary<string, Tuple<string, bool>>();

         public override void Context()
         {
            replacement_items.Add("//configuration/connectionStrings/add[@name='bob']/@connectionString", "bob");
            replacement_items.Add("//configuration/connectionStrings/add[@name='bob']/@providerName", "System.Data.OleDb");

            insert_items.Add("//configuration/connectionStrings/add[@name='newDB']/@connectionString", Tuple.Create("newConnectionString", false));
            insert_items.Add("//configuration/connectionStrings/add[@name='newDB']/@providerName", Tuple.Create("System.Data.SqlClient", false));

            insert_items.Add("//configuration/connectionStrings/add[@name='newConnection2']/@connectionString", Tuple.Create("newConnectionString2", true));//here is the difference :)
            insert_items.Add("//configuration/connectionStrings/add[@name='newConnection2']/@providerName", Tuple.Create("System.Data.OleDb", false));

            task = new XmlPokeTask(file_path_insert_shouldBeFirst, replacement_items, insert_items, new DotNetPath());
         }

         [Fact]
         public void should_have_updated_the_file()
         {
            string updated_text = File.ReadAllText(file_path_insert_shouldBeFirst);
            updated_text.ShouldBeEqualTo(XmlContextResult.ResultInsert_shouldBeFirst);
         }
      }


      [ConcernFor("Xml")]
      [Category("Integration")]
      public class when_updating_the_settings_in_an_xml_file_with_namespaces : XmlPokeTaskSpecsBase {
         protected IDictionary<string, string> replacement_items = new Dictionary<string, string>();
         protected IDictionary<string, string> namespace_prefixes = new Dictionary<string, string> { { "test", "http://example.com/schemas/test" } };

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
         protected IDictionary<string, Tuple<string, bool>> insert_items = new Dictionary<string, Tuple<string, bool>>();
         protected IDictionary<string, string> namespace_prefixes = new Dictionary<string, string> { { "test", "http://example.com/schemas/test" } };

         public override void Context()
         {
            replacement_items.Add("//test:configuration/test:connectionStrings/test:add[@name='bob']/@connectionString", "bob");
            replacement_items.Add("//test:configuration/test:connectionStrings/test:add[@name='bob']/@providerName", "System.Data.OleDb");

            insert_items.Add("//test:configuration/test:connectionStrings/test:add[@name='newDB']/@connectionString", Tuple.Create("newConnectionString", false));
            insert_items.Add("//test:configuration/test:connectionStrings/test:add[@name='newDB']/@providerName", Tuple.Create("System.Data.SqlClient", false));

            insert_items.Add("//test:configuration/test:connectionStrings/test:add[@name='newConnection2']/@connectionString", Tuple.Create("newConnectionString2", false));
            insert_items.Add("//test:configuration/test:connectionStrings/test:add[@name='newConnection2']/@providerName", Tuple.Create("System.Data.OleDb", false));

            task = new XmlPokeTask(file_path_insert_with_namespace, replacement_items, insert_items, new DotNetPath(), namespace_prefixes);
         }

         [Fact]
         public void should_have_updated_the_file()
         {
            string updated_text = File.ReadAllText(file_path_insert_with_namespace);
            updated_text.ShouldBeEqualTo(XmlContextResult.ResultInsertWithNamespace);
         }
      }

      [ConcernFor("Xml")]
      [Category("Integration")]
      public class when_adding_Elmah_to_web_config_file : XmlPokeTaskSpecsBase {
         protected IDictionary<string, string> replacement_items = new Dictionary<string, string>();


         public override void Context()
         {
            string xmlNameSpacePrefix = String.Empty;
            string errorLogXmlPath = "~/App_Data/Logs";

            var real_insert_items = XmlPokeTaskSpecs.AddElmahCore(xmlNameSpacePrefix, errorLogXmlPath);

            task = new XmlPokeTask(file_path_TestAddingElmahToWebConfig, replacement_items, real_insert_items, new DotNetPath());
         }



         [Fact]
         public void should_have_updated_the_file()
         {
            //string updated_text = File.ReadAllText(file_path_TestAddingElmahToWebConfig);
            var updatedXml = XElement.Load(file_path_TestAddingElmahToWebConfig);
            var expectedElmahXml = XElement.Load(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(XmlContextResult.ResultWebConfigWithElmah)));

            //comparing two xml files of this size as plain text would be a "bit" fragile...
            //updated_text.ShouldBeEqualTo(XmlContextResult.ResultElmahWithNamespace);
            Assert.True(System.Xml.Linq.XNode.DeepEquals(updatedXml, expectedElmahXml));
         }
      }

      [ConcernFor("Xml")]
      [Category("Integration")]
      public class when_adding_Elmah_to_web_config_file_with_namespace : XmlPokeTaskSpecsBase {
         protected IDictionary<string, string> replacement_items = new Dictionary<string, string>();


         public override void Context()
         {
            string xmlNameSpacePrefix = "test";
            string errorLogXmlPath = "~/App_Data/Logs";

            var real_insert_items = XmlPokeTaskSpecs.AddElmahCore(xmlNameSpacePrefix, errorLogXmlPath);

            task = new XmlPokeTask(file_path_TestAddingElmahToWebConfigWithNamespace, replacement_items, real_insert_items, new DotNetPath(), new Dictionary<string, string> { { "test", "http://schemas.microsoft.com/.NetConfiguration/v2.0" } });
         }

         [Fact]
         public void should_have_updated_the_file()
         {
            var updatedXml = XElement.Load(file_path_TestAddingElmahToWebConfig);
            var expectedElmahXml = XElement.Load(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(XmlContextResult.ResultWebConfigWithElmah)));

            //comparing two xml files of this size as plain text would be a "bit" fragile...
            Assert.True(System.Xml.Linq.XNode.DeepEquals(updatedXml, expectedElmahXml));
         }

         /// <summary>
         /// mainly to prevent a false positive...
         /// </summary>
         [Fact]
         public void throws_if_namespace_not_set_properly()
         {
            string xmlNameSpacePrefix = String.Empty;
            string errorLogXmlPath = "~/App_Data/Logs";

            var real_insert_items = XmlPokeTaskSpecs.AddElmahCore(xmlNameSpacePrefix, errorLogXmlPath);

            task = new XmlPokeTask(file_path_TestAddingElmahToWebConfigWithNamespace__fails_if_namespace_not_set_properly, replacement_items, real_insert_items, new DotNetPath());

            var thrown = Assert.Throws<Exception>(() => {
               task.Execute();
            });

            Assert.IsInstanceOf<InvalidOperationException>(thrown.InnerException);
            Assert.IsTrue(thrown.Message.Contains("xPath:"), "UpdateOrInsertValueInFile should have added the xPath to the message.");
         }
      }



      internal static Dictionary<string, Tuple<string, bool>> AddElmahCore(string xmlNameSpacePrefix, string errorLogXmlPath)
      {
         IDictionary<string, Tuple<string, bool>> tmp_insert_items = new Dictionary<string, Tuple<string, bool>>();

         if(!string.IsNullOrWhiteSpace(xmlNameSpacePrefix)) {
            xmlNameSpacePrefix = xmlNameSpacePrefix.Trim(':').Trim();
            xmlNameSpacePrefix += ":";
         } else { xmlNameSpacePrefix = String.Empty; }


         // = = = adding sectionGroups
         var sectionGroups = new Dictionary<string, string>{
             {"//{0}configuration/{0}configSections/{0}sectionGroup[@name='elmah']/{0}section[@name='security']", "Elmah.SecuritySectionHandler, Elmah"},
                  {"//{0}configuration/{0}configSections/{0}sectionGroup[@name='elmah']/{0}section[@name='errorLog']", "Elmah.ErrorLogSectionHandler, Elmah"},
                  {"//{0}configuration/{0}configSections/{0}sectionGroup[@name='elmah']/{0}section[@name='errorMail']", "Elmah.ErrorMailSectionHandler, Elmah"},
                  {"//{0}configuration/{0}configSections/{0}sectionGroup[@name='elmah']/{0}section[@name='errorFilter']", "Elmah.ErrorFilterSectionHandler, Elmah"}
              };

         foreach(var sg in sectionGroups) {
            tmp_insert_items.Add(sg.Key + "/@requirePermission", Tuple.Create("false", false));
            tmp_insert_items.Add(sg.Key + "/@type", Tuple.Create(sg.Value, false));
         }

         //adding the whole thing under location=".", so child applications won't inherit it.
         string locationDotXpath = @"//{0}configuration/{0}location[@path='.']";
         tmp_insert_items.Add(locationDotXpath + "/@inheritInChildApplications", Tuple.Create("false", false));

         // = = = adding the following to system.web/httpModules
         //<add name="ErrorLog" type="Elmah.ErrorLogModule, Elmah" />
         //<add name="ErrorMail" type="Elmah.ErrorMailModule, Elmah" />
         //<add name="ErrorFilter" type="Elmah.ErrorFilterModule, Elmah" />
         string systemWeb_httpModulesXpath = locationDotXpath + @"/{0}system.web/{0}httpModules";
         tmp_insert_items.Add(systemWeb_httpModulesXpath + "/{0}add[@name='ErrorLog']/@type", Tuple.Create("Elmah.ErrorLogModule, Elmah", false));
         tmp_insert_items.Add(systemWeb_httpModulesXpath + "/{0}add[@name='ErrorMail']/@type", Tuple.Create("Elmah.ErrorMailModule, Elmah", false));
         tmp_insert_items.Add(systemWeb_httpModulesXpath + "/{0}add[@name='ErrorFilter']/@type", Tuple.Create("Elmah.ErrorFilterModule, Elmah", false));


         // = = = adding the following to system.webServer/modules:
         //<add name="ErrorLog" type="Elmah.ErrorLogModule, Elmah" preCondition="managedHandler" />
         //<add name="ErrorMail" type="Elmah.ErrorMailModule, Elmah" preCondition="managedHandler" />
         //<add name="ErrorFilter" type="Elmah.ErrorFilterModule, Elmah" preCondition="managedHandler" />
         string systemWebServer_modulesXpath = locationDotXpath + @"/{0}system.webServer/{0}modules";
         tmp_insert_items.Add(systemWebServer_modulesXpath + "/{0}add[@name='ErrorLog']/@type", Tuple.Create("Elmah.ErrorLogModule, Elmah", false));
         tmp_insert_items.Add(systemWebServer_modulesXpath + "/{0}add[@name='ErrorLog']/@preCondition", Tuple.Create("managedHandler", false));
         tmp_insert_items.Add(systemWebServer_modulesXpath + "/{0}add[@name='ErrorMail']/@type", Tuple.Create("Elmah.ErrorMailModule, Elmah", false));
         tmp_insert_items.Add(systemWebServer_modulesXpath + "/{0}add[@name='ErrorMail']/@preCondition", Tuple.Create("managedHandler", false));
         tmp_insert_items.Add(systemWebServer_modulesXpath + "/{0}add[@name='ErrorFilter']/@type", Tuple.Create("Elmah.ErrorFilterModule, Elmah", false));
         tmp_insert_items.Add(systemWebServer_modulesXpath + "/{0}add[@name='ErrorFilter']/@preCondition", Tuple.Create("managedHandler", false));


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
         string elmahXpath = @"//{0}configuration/{0}elmah";
         tmp_insert_items.Add(elmahXpath + "/{0}security/@allowRemoteAccess", Tuple.Create("1", false));
         tmp_insert_items.Add(elmahXpath + "/{0}errorLog/@type", Tuple.Create("Elmah.XmlFileErrorLog, Elmah", false));
         tmp_insert_items.Add(elmahXpath + "/{0}errorLog/@logPath", Tuple.Create(errorLogXmlPath, false));

         string equalXpath = elmahXpath + "/{0}errorFilter/{0}test/{0}and/{0}equal";
         tmp_insert_items.Add(equalXpath + "/@binding", Tuple.Create("HttpStatusCode", false));
         tmp_insert_items.Add(equalXpath + "/@value", Tuple.Create("404", false));
         tmp_insert_items.Add(equalXpath + "/@type", Tuple.Create("Int32", false));

         // = = = adding elmah.axd securely 
         // - this part was wrong (XmlPokeTask was right), because updated all <location path="anything"> blocks in the web.config.
         //   fixed, and added a different location section to the source xml to prevent it from happening
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
         string elmahAxdPath = "elmah.axd";
         string locationXpath = @"//{0}configuration/{0}location[@path='" + elmahAxdPath + "']";
         string allowedUsers = @"testDomain\admin, testDomain\otherAdmin";
         string allowedGroups = @"testDomain\admins";
         string ELMAH_ERRORLOGPAGEFACTORY = @"Elmah.ErrorLogPageFactory, Elmah";
         string GET_POST_HEAD = @"POST,GET,HEAD";

         //adding stuff to system.web
         if(!string.IsNullOrWhiteSpace(allowedUsers)) { tmp_insert_items.Add(locationXpath + "/{0}system.web/{0}authorization/{0}allow/@users", Tuple.Create(allowedUsers, false)); }
         if(!string.IsNullOrWhiteSpace(allowedGroups)) { tmp_insert_items.Add(locationXpath + "/{0}system.web/{0}authorization/{0}allow/@groups", Tuple.Create(allowedGroups, false)); }
         tmp_insert_items.Add(locationXpath + "/{0}system.web/{0}authorization/{0}deny/@users", Tuple.Create("*", false));
         tmp_insert_items.Add(locationXpath + "/{0}system.web/{0}httpHandlers/{0}add/@verb", Tuple.Create(GET_POST_HEAD, false));
         tmp_insert_items.Add(locationXpath + "/{0}system.web/{0}httpHandlers/{0}add/@path", Tuple.Create(elmahAxdPath, false));
         tmp_insert_items.Add(locationXpath + "/{0}system.web/{0}httpHandlers/{0}add/@type", Tuple.Create(ELMAH_ERRORLOGPAGEFACTORY, false));

         ////addign stuff to system.webserver
         tmp_insert_items.Add(locationXpath + "/{0}system.webServer/{0}security/{0}authorization/{0}clear", Tuple.Create("", false));
         if(!string.IsNullOrWhiteSpace(allowedUsers)) {
            tmp_insert_items.Add(locationXpath + "/{0}system.webServer/{0}security/{0}authorization/{0}add[@users='" + allowedUsers + "']/@accessType", Tuple.Create("Allow", false));
         }
         if(!string.IsNullOrWhiteSpace(allowedGroups)) {
            tmp_insert_items.Add(locationXpath + "/{0}system.webServer/{0}security/{0}authorization/{0}add[@groups='" + allowedGroups + "']/@accessType", Tuple.Create("Allow", false));
         }

         tmp_insert_items.Add(locationXpath + "/{0}system.webServer/{0}handlers/{0}add/@name", Tuple.Create("Elmah", false));
         tmp_insert_items.Add(locationXpath + "/{0}system.webServer/{0}handlers/{0}add/@path", Tuple.Create(elmahAxdPath, false));
         tmp_insert_items.Add(locationXpath + "/{0}system.webServer/{0}handlers/{0}add/@verb", Tuple.Create(GET_POST_HEAD, false));
         tmp_insert_items.Add(locationXpath + "/{0}system.webServer/{0}handlers/{0}add/@type", Tuple.Create(ELMAH_ERRORLOGPAGEFACTORY, false));
         tmp_insert_items.Add(locationXpath + "/{0}system.webServer/{0}handlers/{0}add/@preCondition", Tuple.Create("integratedMode", false));


         var real_insert_items = new Dictionary<string, Tuple<string, bool>>();
         //add an empty Elmah section group to the begining of the web.config.
         real_insert_items.Add(String.Format("//{0}configuration/{0}configSections/{0}sectionGroup[@name='elmah']", xmlNameSpacePrefix), Tuple.Create("", true));
         foreach(var item in tmp_insert_items) {
            real_insert_items.Add(string.Format(item.Key, xmlNameSpacePrefix), Tuple.Create(item.Value.Item1, item.Value.Item2));
         }
         return real_insert_items;
      }
   }
}