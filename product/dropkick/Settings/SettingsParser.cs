namespace dropkick.Settings
{
    using System.Collections.Generic;
    using System.IO;
    using Magnum.CommandLineParser;
    using Magnum.Reflection;

    public class SettingsParser
    {
        readonly ICommandLineParser _parser = new MonadicCommandLineParser();

        public T Parse<T>(string contents) where T : new()
        {
            var result = new T();

            var po = _parser.Parse(contents);

            Set(result, po);

            return result;
        }

        public T Parse<T>(FileInfo file) where T : new()
        {
            var result = new T();

            var contents = File.ReadAllText(file.FullName);
            var po = _parser.Parse(contents);

            Set(result, po);

            return result;
        }

        void Set<T>(T result, IEnumerable<ICommandLineElement> enumerable)
        {
            var type = typeof (T);
            
            foreach (IDefinitionElement argument in enumerable)
            {
                var pi = type.GetProperty(argument.Key);
                var fp = new FastProperty<T>(pi);
                fp.Set(result, argument.Value);
            }
        }
    }
}