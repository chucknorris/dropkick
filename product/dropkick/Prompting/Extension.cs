namespace dropkick.Prompting
{
    using Magnum.Extensions;

    public static class Extension
    {
        public static bool ShouldPrompt(this string input)
        {
            if (input.IsNotEmpty() && input.Equals("?")) 
                return true;
            return (!input.IsNotEmpty());
        }

    }
}