using SLangParser;
using SLangScanner;
using System.IO;

namespace SLangMetrics
{
    class Program
    {
        static void Main(string[] args)
        {
            string src = @"in.txt";
            FileStream file = new FileStream(src, FileMode.Open);
            Scanner scanner = new Scanner(file);
            Parser parser = new Parser(scanner);
        }
    }
}
