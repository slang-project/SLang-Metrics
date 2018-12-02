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
                Testing.runTests();
            }
            else
            {
                MetricCollector coll = new MetricCollector(args[0]);
                // TODO call representation of metrics (toHTML or CliInterface)
            }
        }
    }
}
