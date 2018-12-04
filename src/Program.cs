using Metrics;
using System;
using System.Linq;

namespace SLangMetrics
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine(
                        "Usage: <ProgramName> <InputFileName> <InterfaceArgs>\n" +
                        "    OR <ProgramName> /test"
                );
                Environment.Exit(1);
                return;
            }

            if (Testing.isTestingMode || args.Any(s => s.ToLower() == "/test"))
            {
                Environment.Exit(Testing.runTests() ? 0 : 1);
                return;
            }

            MetricCollector collector;
            try
            {
                collector = new MetricCollector(args[0]);
            }
            catch (ParsingFailedException)
            {
                Console.WriteLine(
                        "Parsing failed!" +
                        "No metrics will be provided" +
                        "Terminating process..."
                );
                Environment.Exit(1);
                return;
            }

            collector.ActivateInterface(args.Where(s => s.StartsWith("/")).ToArray());
        }
    }
}
