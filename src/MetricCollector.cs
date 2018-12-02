using LanguageElements;

namespace Metrics
{
    class MetricCollector
    {
        Module parsedModule;

        internal MetricCollector(string fileName)
        {
            this.parsedModule = SLangParser.Parser.parseProgram(fileName);
        }
    }
}
