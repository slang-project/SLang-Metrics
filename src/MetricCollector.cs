using LanguageElements;

namespace Metrics
{
    class MetricCollector
    {
        private Module parsedModule;

        internal MetricCollector(string fileName)
        {
            this.parsedModule = SLangParser.Parser.parseProgram(fileName);
        }
    }
}
