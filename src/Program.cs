using Metrics;
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
                Environment.Exit(Testing.runTests() ? 0 : 1);
            }

            MetricCollector collector;
            try
            {
                collector = new MetricCollector(args[0]);
                collector.Debug();  // TODO: change metric revealence interface
            }
            catch (ParsingFailedException)
            {
                Console.WriteLine("Parsing failed!");
                Console.WriteLine("No metrics will be provided");
                Console.WriteLine("Terminating process...");
                Environment.Exit(1);
            }
        }
    }
}
