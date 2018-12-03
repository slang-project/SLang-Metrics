using System;
using System.Diagnostics;
using LanguageElements;

namespace Metrics
{
    class MetricCollector
    {
        private Module parsedModule;

        public MetricCollector(string fileName)
        {
            this.parsedModule = SLangParser.Parser.parseProgram(fileName);
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
