using SLangLookaheadScanner;
using SLangParser;
using System;
using System.IO;

namespace SLangMetrics
{
    class Program
    {
        /*
         * Use argument /tests in console command.
         *
         * Тестом является сабдиректория в директории tests\ корня проэкта. в сабдириктории должны содержаться файлы расширений .res и .test
         * .res - результат парсинга кода в .test файле. Результат может быть true или false.
         * .test - входные данные для parser. Может быть как положительным, так и отрицательным тестом.
         *
         * Все входные тесты (написанные корректно) должны содержать SUCCED в выходной строке, иначе содержат FAILED.
         * Название файлов, сабдиректорий может быть произвольным
         */
        static bool testing = false; // make true while release!

        static void Main(string[] args)
        {
            if (testing || Array.Exists<String>(args, s => s.ToLower().Contains("/test")))
            {
                Testing.runTests();
            }
            else
            {
                if (args.Length < 1)
                {
                    Console.WriteLine("Usage: <ProgramName> <InputFileName>");
                    Environment.Exit(1);
                }
                Console.WriteLine(parseProgram(args[0]));
            }
            Console.Read();
        }

        public static bool parseProgram(String filePath)
        {
            FileStream file = new FileStream(filePath, FileMode.Open);
            Scanner scanner = new Scanner(file);
            Parser parser = new Parser(scanner);
            return parser.Parse();
        }
    }
}
