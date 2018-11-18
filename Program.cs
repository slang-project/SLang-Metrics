using SLangParser;
using SLangLookaheadScanner;
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
            FileStream file = new FileStream(args[0], FileMode.Open);
            Scanner scanner = new Scanner(file);
            Parser parser = new Parser(scanner);
            Console.WriteLine(parser.Parse());
            Console.Read();
        }
    }
}
