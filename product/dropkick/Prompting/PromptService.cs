namespace dropkick.Prompting
{
    using System;
    using Magnum.Reflection;

    public interface PromptService
    {
        T Prompt<T>() where T : new();
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
                Console.WriteLine("{0}:", p.Name);
                var capturedValue = Console.ReadLine();
                new FastProperty<T>(p).Set(output, capturedValue);
            }

            return output;
        }
    }
}