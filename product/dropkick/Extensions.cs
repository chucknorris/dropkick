namespace dropkick
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Magnum.CommandLineParser;

    public static class Extensions
    {
        public static string FormatWith(this string input, params object[] args)
        {
            return string.Format(input, args);
        }
        public static T ToEnum<T>(this string input)
        {
            return (T) Enum.Parse(typeof(T), input, true);
        }

        public static string GetDefinition(this IEnumerable<ICommandLineElement> elements, string key)
        {
            return elements
                .Where(x => typeof(IDefinitionElement).IsAssignableFrom(x.GetType()))
                .Select(x => x as IDefinitionElement)
                .Where(x => x.Key == key || x.Key[0] == key[0])
                .Select(x => x.Value)
                .Single();
        }

        public static string GetDefinition(this IEnumerable<ICommandLineElement> elements, string key, string defaultValue)
        {
            return elements
                .Where(x => typeof(IDefinitionElement).IsAssignableFrom(x.GetType()))
                .Select(x => x as IDefinitionElement)
                .Where(x => x.Key == key || x.Key[0] == key[0])
                .Select(x => x.Value)
                .DefaultIfEmpty(defaultValue)
                .Single();
        }

        public static T GetDefinition<T>(this IEnumerable<ICommandLineElement> elements, string key,
                                         Func<string, T> converter)
        {
            return elements
                .Where(x => typeof(IDefinitionElement).IsAssignableFrom(x.GetType()))
                .Select(x => x as IDefinitionElement)
                .Where(x => x.Key == key)
                .Select(x => converter(x.Value))
                .Single();
        }
    }
}