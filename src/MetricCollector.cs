using System;
using System.Diagnostics;
using LanguageElements;

namespace Metrics
{
    class MetricCollector
    {
        private Module parsedModule;

        internal MetricCollector(string fileName)
        {
            this.parsedModule = SLangParser.Parser.parseProgram(fileName);
            debug();
        }

        // TODO: hello, human
        private void debug()
        {
            Console.WriteLine(parsedModule.getCC());
        }
    }
}
