namespace dropkick.Settings
{
    using System.IO;
    using Engine;
    using Magnum.CommandLineParser;

    public class ServerMapParser
    {
        readonly ICommandLineParser _parser = new MonadicCommandLineParser();

        public RoleToServerMap Parse(FileInfo file)
        {
            var contents = File.ReadAllText(file.FullName);
            var output = _parser.Parse(contents);

            var result = new RoleToServerMap();

            foreach (IDefinitionElement element in output)
            {
                result.AddMap(element.Key, element.Value);
            }

            return result;
        }
    }
}