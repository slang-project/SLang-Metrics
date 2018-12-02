using SLangLookaheadScanner;
using SLangParser;
using System;
using System.IO;

namespace SLangMetrics
{
    internal class Testing
    {
        /*
         * Use argument /tests in console command.
         *
         * Тестом является сабдиректория в директории tests\ корня проекта. в сабдириктории должны содержаться файлы расширений .res и .test
         * .res - результат парсинга кода в .test файле. Результат может быть true или false.
         * .test - входные данные для parser. Может быть как положительным, так и отрицательным тестом.
         *
         * Все входные тесты (написанные корректно) должны содержать SUCCED в выходной строке, иначе содержат FAILED.
         * Название файлов, сабдиректорий может быть произвольным
         */
        public static bool isTestingMode = false;  // TODO make false while release!

        private static bool checkTestDir(DirectoryInfo dir, String pattern)
        {
            FileInfo[] files = dir.GetFiles(pattern);
            if (files.Length == 1)
            {
                return true;
            }
            else
            {
                if (files.Length == 0)
                    Console.WriteLine("There are no file {0} in {1}!", pattern, dir.FullName);
                else
                    Console.WriteLine("There are too many file {0} in {1}!", pattern, dir.FullName);
                return false;
            }
        }

        public static void runTests()
        {
            String resExt = "*.res";
            String testExt = "*.test";
            DirectoryInfo dir = new DirectoryInfo("tests\\");
            try
            {
                foreach (DirectoryInfo d in dir.GetDirectories())
                {
                    Console.WriteLine("\nTest: {0}", d.Name);
                    if (checkTestDir(d, resExt) && checkTestDir(d, testExt))
                    {
                        // Expected result
                        String res = File.ReadAllText(d.GetFiles(resExt)[0].FullName).Trim();
                        // Parsing
                        bool ret = Parser.parseProgram(d.GetFiles(testExt)[0].FullName.Trim()) == null;
                        if (res.ToLower().Equals(ret.ToString().ToLower()))
                        {
                            Console.WriteLine("SUCCED");
                        }
                        else
                        {
                            Console.WriteLine("FAILED");
                        }
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("IOException: " + e.GetBaseException());
            }
            Console.WriteLine("---------------- Tests finished ----------------\n\n");
        }
    }
}
