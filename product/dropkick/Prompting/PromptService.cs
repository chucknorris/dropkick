namespace dropkick.Prompting
{
    using System;
    using Magnum.Reflection;

    public interface PromptService
    {
        T Prompt<T>() where T : new();
        string Prompt(string nameToDisplay);
    }

    public class ConsolePromptService :
        PromptService
    {
        public T Prompt<T>() where T : new()
        {
            var output = new T();

            Console.WriteLine("Please provide the following:");
            var t = typeof (T);
            foreach(var p in t.GetProperties())
            {
                var capturedValue = Prompt(p.Name);
                new FastProperty<T>(p).Set(output, capturedValue);
            }

            return output;
        }
        public string Prompt(string nameToDisplay)
        {
            Console.WriteLine("{0}:", nameToDisplay);
            var capturedValue = Console.ReadLine();
            return capturedValue;
        }
    }
}