namespace dropkick.tests.Engine
{
    using System;
    using dropkick.Engine.CommandLineParsing;
    using NUnit.Framework;

    [TestFixture]
    public class ArgumentParsing_Specs
    {
        [Test]
        public void NAME()
        {
            var commandline = "verify -d:bill -e:test";
            var o =MonadicCommandLineParser.Parse(commandline);
        }
    }
}