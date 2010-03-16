namespace dropkick.Settings
{
    using System;
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

            Set(typeof(T), result, po);

            return result;
        }

        public T Parse<T>(FileInfo file) where T : new()
        {
            return (T) Parse(typeof (T), file);
        }

        public object Parse(Type t, FileInfo file)
        {
            var result = FastActivator.Create(t);

            var contents = File.ReadAllText(file.FullName);
            var po = _parser.Parse(contents);

            Set(t, result, po);

            return result;
        }

        void Set(Type type, object result, IEnumerable<ICommandLineElement> enumerable)
        {
            foreach (IDefinitionElement argument in enumerable)
            {
                var pi = type.GetProperty(argument.Key);
                var fp = new FastProperty(pi);
                fp.Set(result, argument.Value);
            }
        }
    }
}