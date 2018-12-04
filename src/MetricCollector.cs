using System;
using System.Diagnostics;
using LanguageElements;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Metrics
{
    class MetricCollector
    {
        private Module parsedModule;
        private LinkedList<UnitDeclaration> units;
        private LinkedList<UnitDeclaration> leafUnits;

        public MetricCollector(string fileName)
        {
            this.parsedModule = SLangParser.Parser.parseProgram(fileName);
            Console.WriteLine("------------");

            Traverse traverse = new Traverse(parsedModule);
            InheritanceWrapper inheritance = new InheritanceWrapper(traverse.unitList);
        }

        public bool IsParsingSuccessful()
        {
            return this.parsedModule != null;
        }

        public void debug()
        {
            Console.WriteLine(parsedModule.getCC());
        }
    }
}
