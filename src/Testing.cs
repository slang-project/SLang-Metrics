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

        public static bool runTests()
        {
            String resExt = "*.res";
            String testExt = "*.test";
            DirectoryInfo dir = new DirectoryInfo("tests\\");
            bool failed = false;
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
                        bool ret = Parser.parseProgram(d.GetFiles(testExt)[0].FullName.Trim()) != null;
                        string result = ret.ToString().ToLower();
                        Console.WriteLine("Returned: " + result);
                        if (res.ToLower().Equals(result))
                        {
                            Console.WriteLine("SUCCEED");
                        }
                        else
                        {
                            Console.WriteLine("FAILED");
                            failed = true;
                        }
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("IOException: " + e.GetBaseException());
            }
            Console.WriteLine("---------------- Tests finished ----------------\n\n");
            return !failed;
        }
    }
}
