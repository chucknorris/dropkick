namespace dropkick.tests.Tasks.Xml
{
    using System.Collections.Generic;
    using System.IO;
    using Context;
    using dropkick.DeploymentModel;
    using dropkick.Tasks.Xml;
    using FileSystem;
    using NUnit.Framework;

    public class XmlPokeTaskSpecs
    {
        public abstract class XmlPokeTaskSpecsBase : TinySpec
        {
            protected DeploymentResult result;
            protected XmlPokeTask task;
            protected string file_path = @".\Tasks\Xml\Context\Test.xml";
            protected string file_path_with_namespace = @".\Tasks\Xml\Context\TestWithNamespace.xml";

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
        public class when_updating_the_settings_in_an_xml_file_with_namespaces : XmlPokeTaskSpecsBase
        {
            protected IDictionary<string, string> replacement_items = new Dictionary<string, string>();
            protected IDictionary<string, string> namespace_prefixes = new Dictionary<string, string> {{"test", "http://example.com/schemas/test"}};

            public override void Context()
            {
                replacement_items.Add("//test:configuration/test:connectionStrings/test:add[@name='bob']/@connectionString", "bob");
                task = new XmlPokeTask(file_path_with_namespace, replacement_items, new DotNetPath(), namespace_prefixes);
            }

            [Fact]
            public void should_have_updated_the_file()
            {
                string updated_text = File.ReadAllText(file_path_with_namespace);
                updated_text.ShouldBeEqualTo(XmlContextResult.ResultWithNamespace);
            }
        }
    }

}