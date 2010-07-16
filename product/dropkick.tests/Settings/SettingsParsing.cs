namespace dropkick.tests.Settings
{
    using System.IO;
    using dropkick.Settings;
    using Magnum.Reflection;
    using NUnit.Framework;

    [TestFixture]
    public class SettingsParsing
    {
        [Test]
        public void ParseTheContentToObject()
        {
            var commandLine = @"-Website:cue -Database:cue_db -YesNo:true";

            var p = new SettingsParser();
            var r = p.Parse<TestSettings>(new FileInfo("."), commandLine, "test");

            Assert.AreEqual("cue_db",r.Database);
            Assert.AreEqual("cue",r.Website);
            Assert.IsTrue(r.YesNo);
        }

        [Test]
        public void ViaFastInvoke()
        {
            var commandLine = @"-Website:cue -Database:cue_db -YesNo:true";

            var p = new SettingsParser();
            var r = (TestSettings)p.FastInvoke<SettingsParser, object>(new []{typeof(TestSettings)}, "Parse", new FileInfo("."), commandLine, "test");

            Assert.AreEqual("cue_db", r.Database);
            Assert.AreEqual("cue", r.Website);
            Assert.IsTrue(r.YesNo);
        }
    }

    public class TestSettings
    {
        public string Website { get; set; }
        public string Database { get; set; }
        public bool YesNo { get; set; }
    }
}