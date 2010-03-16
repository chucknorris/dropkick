namespace dropkick.Engine
{
    using System.Collections.Generic;
    using System.Linq;
    using Magnum.CommandLineParser;

    public static class DeploymentCommandLineParser
    {
        public static DeploymentArguments Parse(string commandline)
        {
            var result = new DeploymentArguments();

            Set( result, P(commandline));

            return result;
        }

        static void Set(DeploymentArguments arguments, IEnumerable<ICommandLineElement> commandLineElements)
        {
            var command = commandLineElements.Where(x => x is IArgumentElement)
                .Select(x => (IArgumentElement) x)
                .DefaultIfEmpty(new ArgumentElement("trace"))
                .Select(x=>x.Id)
                .SingleOrDefault();

            arguments.Command = command.ToEnum<DeploymentCommands>();


            var deployment = commandLineElements.GetDefinition("deployment", "SEARCH");
            arguments.Deployment = deployment;


            var part = commandLineElements.GetDefinition("part", "ALL");
            arguments.Role = part;

            var enviro = commandLineElements.GetDefinition("environment", "LOCAL");
            arguments.Environment = enviro;

            var config = commandLineElements.GetDefinition("settings", ".\\settings");
            arguments.SettingsDirectory = config;
        }

        static IEnumerable<ICommandLineElement> P(string commandLine)
        {
            var parser = new Magnum.CommandLineParser.MonadicCommandLineParser();

            return parser.Parse(commandLine);
        }
    }
}