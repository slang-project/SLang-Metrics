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
                InheritanceWrapper inheritance = new InheritanceWrapper(traverse.unitList);
                
                Console.WriteLine("Max Inheritance: " + inheritance.getMaxHierarchyHeight());
                Console.WriteLine("Avg Inheritance: " + inheritance.getAverageHierarchyHeight());
                
                foreach(string unitName in inheritance.getUnitNames() ?? Enumerable.Empty<string>())
                {
                    Console.WriteLine(String.Format("Unit: <{0}>, descendants: {1}, inheritance height: {2}", unitName, inheritance.getDescendantsCount(unitName), inheritance.getHierachyHeight(unitName)));
                    foreach(string path in inheritance.getHierarchyPaths(unitName) ?? Enumerable.Empty<string>())
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
