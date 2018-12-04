using System;
using System.Diagnostics;
using LanguageElements;

namespace Metrics
{
    class MetricCollector
    {
        //TODO: for now it accessible. If future this class should provide features to extract info from Module @parsedModule
        internal Module parsedModule { get; }

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