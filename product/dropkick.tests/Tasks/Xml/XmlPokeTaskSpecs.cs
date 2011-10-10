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

            public override void Context()
            {
                result = new DeploymentResult();
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

            public override void Because()
            {
                result = task.Execute();
            }

            [Fact]
            public void should_execute_successfully()
            {
                System.Console.WriteLine(result.ToString());
            }

            [Fact]
            public void should_have_updated_the_file()
            {
                string updated_text = File.ReadAllText(file_path);
                updated_text.ShouldBeEqualTo(XmlContextResult.Result);
            }
        }


    }

}