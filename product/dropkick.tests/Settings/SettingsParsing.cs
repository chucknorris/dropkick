namespace dropkick.tests.Settings
{
    using dropkick.Settings;
    using NUnit.Framework;

    [TestFixture]
    public class SettingsParsing
    {
        [Test]
        public void ParseTheContentToObject()
        {
            var xml = @"-Website:cue
-Database:cue_db
-YesNo:true";

            var p = new SettingsParser();
            var r = p.Parse<TestSettings>(xml);

            Assert.AreEqual("cue_db",r.Database);
            Assert.AreEqual("cue",r.Website);
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