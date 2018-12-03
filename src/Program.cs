using Metrics;
using System;
using SLangTests;

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
                Testing testing = new Testing();
                Environment.Exit(testing.runTests() ? 0 : 1);
            }

            MetricCollector coll = new MetricCollector(args[0]);
            Console.Write("Is parsing successful: ");
            Console.WriteLine(coll.IsParsingSuccessful() ? "yes" : "no");

            if (!coll.IsParsingSuccessful())
            {
                Console.WriteLine("No metrics will be provided");
                Console.WriteLine("Terminating process...");
                Environment.Exit(1);
            }

            coll.debug();  // TODO: change metric revealence interface
        }
    }
}
