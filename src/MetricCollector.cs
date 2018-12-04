using LanguageElements;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Metrics
{
    public class MetricCollector
    {
        // TODO: for now it accessible for testing purposes.
        // If future this class should provide features to extract info from Module @parsedModule
        internal Module parsedModule { get; }

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

/* TODO: use this in CLI
            Traverse traverse = new Traverse(parsedModule);
            InheritanceWrapper inheritance = new InheritanceWrapper(traverse.unitList);

            Console.WriteLine("Max Inheritance: " + inheritance.getMaxHierarchyHeight());
            Console.WriteLine("Avg Inheritance: " + inheritance.getAverageHierarchyHeight());

            foreach (string unitName in inheritance.getUnitNames() ?? Enumerable.Empty<string>())
            {
                Console.WriteLine(String.Format("Unit: <{0}>, descendants: {1}, inheritance height: {2}", unitName, inheritance.getDescendantsCount(unitName), inheritance.getHierachyHeight(unitName)));
                foreach (string path in inheritance.getHierarchyPaths(unitName) ?? Enumerable.Empty<string>())
                {
                    Console.WriteLine("  " + path);
                }
            }

            inheritance.printTreeRepresentation();*/
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
                    case "tree":
                        // TODO: print inheritance tree
                        break;

                    case "help":
                        PrintHelp();
                        break;

                    case "exit":
                        goto exit;

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
            string metricName = split[0].ToLower();
            string[] args = split.Skip(1).ToArray();
            switch (metricName)
            {
                case "cc":
                case "cyclomaticcomplexity":
                    // TODO: show
                    break;

                case "ss":
                case "softwaresciences":
                    // TODO: show
                    break;

                case "wru":
                case "weightedroutines":
                case "weightedroutinesperunit":
                    // TODO: show
                    break;

                case "npwru":
                case "nonprivatewru":
                case "nonprivateweightedroutines":
                case "nonprivateweightedroutinesperunit":
                    // TODO: show
                    break;

                case "dit":
                case "depthofinheritancetree":
                    // TODO: show
                    break;

                case "nod":
                case "numberofdescendants":
                    // TODO: show
                    break;

                case "mhh":
                case "maximumhierarchyheight":
                    // TODO: show
                    break;

                case "ahh":
                case "averagehierarchyheight":
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
