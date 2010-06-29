namespace dropkick.tests.StringInterpolation
{
    using dropkick.StringInterpolation;
    using NUnit.Framework;

    [TestFixture]
    public class TestIt
    {
        [Test]
        public void one_replacement()
        {
            CaseInsensitiveInterpolator i = new CaseInsensitiveInterpolator();

            var settings = new Bill() {Name = "dru"};
            var input = "hi {{Name}}";
            var output = i.ReplaceTokens(settings, input);

            Assert.AreEqual("hi dru", output);
        }

        [Test]
        public void case_insensitive()
        {
            CaseInsensitiveInterpolator i = new CaseInsensitiveInterpolator();

            var settings = new Bill() { Name = "dru" };
            var input = "hi {{name}}";
            var output = i.ReplaceTokens(settings, input);

            Assert.AreEqual("hi dru", output);
        }

        [Test]
        public void two_replacement()
        {
            CaseInsensitiveInterpolator i = new CaseInsensitiveInterpolator();

            var settings = new Bill() { Name = "dru", Greeting="hi" };
            var input = "{{Greeting}} {{Name}}";
            var output = i.ReplaceTokens(settings, input);

            Assert.AreEqual("hi dru", output);
        }
    }

    public interface Bob
    {
        string Name { get; set; }
        string Greeting { get; set; }
    }

    public class Bill : Bob
    {
        public string Name { get; set; }
        public string Greeting { get; set; }
    }
}