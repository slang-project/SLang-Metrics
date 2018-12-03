using System;
using System.Diagnostics;
using LanguageElements;

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

        public MetricCollector(string fileName)
        {
            this.parsedModule = SLangParser.Parser.parseProgram(fileName);
            if (parsedModule == null)
            {
                throw new ParsingFailedException();
            }
        }

        public void Debug()
        {
            Console.WriteLine(parsedModule.getCC());
        }
    }
}
