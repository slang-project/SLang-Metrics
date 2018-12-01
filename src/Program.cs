using SLangLookaheadScanner;
using SLangParser;
using System;
using System.IO;

namespace SLangMetrics
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Usage: <ProgramName> <InputFileName>");
                Environment.Exit(1);
            }
            if (Testing.isTestingMode || Array.Exists<String>(args, s => s.ToLower().Contains("/test")))
            {
                Testing.runTests();
            }
            else
            {
                Console.WriteLine(parseProgram(args[0]));
            }
            Console.Read();  // TODO remove in production
        }

        public static bool parseProgram(String filePath)
        {
            FileStream file = new FileStream(filePath, FileMode.Open);
            Scanner scanner = new Scanner(file);
            Parser parser = new Parser(scanner);
            return parser.Parse();
        }
    }
}
