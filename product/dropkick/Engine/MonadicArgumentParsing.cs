namespace dropkick.Engine
{
    using System.Linq;
    using Magnum.CommandLineParser;

    public class MonadicArgumentParsing
    {
        public object Parse(string commandLine)
        {
            ICommandLineParser parser = new MonadicCommandLineParser();
            var args = parser.Parse(commandLine);
            var ap = from a in args
                     select a;

            return null;
        }
    }
}