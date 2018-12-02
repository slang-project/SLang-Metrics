using System;

namespace SLangMetrics
{
    class Program
    {

        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Usage: <ProgramName> <InputFileName> OR <ProgramName> /test");
                Environment.Exit(1);
            }
            if (Testing.isTestingMode || Array.Exists<String>(args, s => s.ToLower().Contains("/test")))
            {
                Testing.runTests();
            }
            else
            {
                Console.WriteLine(SLangParser.Parser.parseProgram(args[0]) == null);
            }
        }
    }
}
