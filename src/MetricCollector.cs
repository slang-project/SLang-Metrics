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

        internal MetricCollector(string fileName)
        {
            this.parsedModule = SLangParser.Parser.parseProgram(fileName);
            Console.WriteLine("------------");
            Traverse traverse = new Traverse(parsedModule);
            foreach(UnitDeclaration ud in traverse.unitList ?? Enumerable.Empty<UnitDeclaration>())
            {
                Console.WriteLine(ud.name.ToString());
            }
        }
        private void renameAndCollect()
        {
            foreach(BlockMember bm in parsedModule.members)
            {
                Console.WriteLine(bm.ToString());
            }
        }
        internal bool IsParsingSuccessful()
        {
            return this.parsedModule != null;
        }
    }
}
