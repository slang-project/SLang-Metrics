using Metrics;
using SLangTests;
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
                int resCode = Testing.runTests() ? 0 : 1;
                
                if(resCode == 0)
                {
                    Console.WriteLine("All tests SUCCED");
                }
                else
                {
                    Console.WriteLine("Tests FAILED");
                }
                Console.ReadLine();
                Environment.Exit(resCode);
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
