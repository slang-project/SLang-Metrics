using System;

namespace SLangMetrics
{
    class Program
    {
        static void Main(string[] args)
        {
            string src = @"src.txt";
            FileStream file = new FileStream(src, FileMode.Open);
            Scanner scanner = new Scanner();
            scanner.SetSource(file, 0);
            Parser parser = new Parser(scanner);
        }
    }
}
