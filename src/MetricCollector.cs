using System;
using System.Diagnostics;
using LanguageElements;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Metrics
{
    public class ParsingFailedException : Exception
    {
        public ParsingFailedException() : base()
        {
        }
    }

    class MaintainabilityIndex
    {
        private double value;

        // Get Maintainability index based on formula used by Microsoft Visual Studio (since 2008)
        public MaintainabilityIndex(double HV, double CC, double LOC)
        {
            this.value = Math.Max(0, (171 - 5.2 * Math.Log(HV, Math.E) - 0.23 * (CC) - 16.2 * Math.Log(LOC, Math.E)) * 100 / 171);
            // MAX(0,(171 - 5.2 * ln(Halstead Volume) - 0.23 * (Cyclomatic Complexity) - 16.2 * ln(Lines of Code))*100 / 171)
        }

        public double getValue()
        {
            return this.value;
        }
    }

    class MetricCollector
    {
        private Module parsedModule;
        private LinkedList<UnitDeclaration> units;
        private LinkedList<UnitDeclaration> leafUnits;

        public MetricCollector(string fileName)
        {
            this.parsedModule = SLangParser.Parser.parseProgram(fileName);

            if (parsedModule == null)
            {
                Console.WriteLine("Parsing Error");
            }
            else
            {
                Traverse traverse = new Traverse(parsedModule);

                Console.WriteLine(String.Format("HH max: {0}, avg: {1}", traverse.maxHH, traverse.averageHH));

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

                inheritance.printTreeRepresentation();
            }
        }

        public void Debug()
        {
            Console.WriteLine("CC is " + parsedModule.getCC());
        }
    }
}
