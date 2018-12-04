using System;
using System.Diagnostics;
using System.Linq;
using LanguageElements;

namespace Metrics
{
    public class MetricCollector
    {
        private Module parsedModule;

        public MetricCollector(string fileName)
        {
            this.parsedModule = SLangParser.Parser.parseProgram(fileName);
            if (parsedModule == null)
            {
                throw new ParsingFailedException();
            }
        }

        public void ActivateInterface(string[] args)
        {
            // TODO: parse args and choose an appropriate interface
            ActivateCLI();
        }

        private void ActivateCLI()
        {
            string input;
            PrintInvitation();
            while (true)
            {
                Console.Write(">>> ");
                input = Console.ReadLine();

                switch (input.ToLower())
                {
                    case "exit":
                        goto exit;

                    case "help":
                        PrintHelp();
                        break;

                    default:
                        ParseMetricQuery(input);
                        break;
                }
            }
            exit:
            Console.WriteLine("Terminating process...");
        }

        private void PrintInvitation()
        {
            Console.WriteLine(
                    "Your input was successfully parsed!\n" +
                    "Now you may ask for some metrics of the given code.\n"
            );
            PrintHelp();
            Console.WriteLine("\nWrite here your query:");
        }

        private void PrintHelp()
        {
            Console.WriteLine(
                "\thelp - print list of queries (this message)\n" +
                "\texit - quit and terminate this session\n" +
                "\t<MetricName> [<MetricArgs>] - print a value of a given metric"
            );
        }

        private void ParseMetricQuery(string input)
        {
            if (input.Length < 1)
            {
                return;
            }

            string[] split = input.Split().Where(s => s.Length > 0).ToArray();
            string metricName = split[0];
            string[] args = split.Skip(1).ToArray();
            switch (metricName)
            {
                case "CC":
                case "CyclomaticComplexity":
                    // TODO: show
                    break;

                case "SS":
                case "SoftwareSciences":
                    // TODO: show
                    break;

                case "WRU":
                case "WeightedRoutines":
                case "WeightedRoutinesPerUnit":
                    // TODO: show
                    break;

                case "NPWRU":
                case "NonPrivateWRU":
                case "NonPrivateWeightedRoutines":
                case "NonPrivateWeightedRoutinesPerUnit":
                    // TODO: show
                    break;

                case "DIT":
                case "DepthOfInheritanceTree":
                    // TODO: show
                    break;

                case "NOD":
                case "NumberOfDescendants":
                    // TODO: show
                    break;

                case "MHH":
                case "MaximumHierarchyHeight":
                    // TODO: show
                    break;

                case "AHH":
                case "AverageHierarchyHeight":
                    // TODO: show
                    break;

                default:
                    Console.WriteLine("Unknown metric: {0}", metricName);
                    break;
            }
        }
    }

    public class ParsingFailedException : Exception
    {
        public ParsingFailedException() : base() { }
    }
}
