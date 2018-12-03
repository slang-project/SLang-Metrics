using LanguageElements;
using System;
using System.Collections.Generic;

namespace Metrics
{
    class MetricCollector
    {
        private Module parsedModule;
        private LinkedList<UnitDeclaration> units;
        private LinkedList<UnitDeclaration> leafUnits;

        internal MetricCollector(string fileName)
        {
            this.parsedModule = SLangParser.Parser.parseProgram(fileName);
            Console.WriteLine("------------");
            this.parsedModule.PrintPretty("", true);
        }
        private void renameAndCollect()
        {
            foreach(BlockMember bm in parsedModule.members)
            {
                Console.WriteLine(bm.ToString());
            }
        }

    }
}
