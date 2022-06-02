using System;

using CommandParameterParse;
using Fluid;

namespace TranslationTemplateCommand
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            for (int i = 0; i < args.Length; i++)
            {
                Console.WriteLine($"args[{i}]: ({args[i]})");
            }
            Console.WriteLine("Print Complate~~~");

            var parser = new FluidParser();
            var model = new { Firstname = "Bill", Lastname = "Gates" };
            var source = "Hello {{ Firstname }} {{ Lastname }}";
            if (parser.TryParse(source, out var template, out var error))
            {
                var context = new TemplateContext(model);
                Console.WriteLine(template.Render(context));
            }
            else
            {
                Console.WriteLine($"Error: {error}");
            }
        }
    }
}
