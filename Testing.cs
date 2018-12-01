using SLangLookaheadScanner;
using SLangParser;
using System;
using System.IO;

namespace SLangMetrics
{
    internal class Testing
    {
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
                    if (checkTestDir(d, resExt) && checkTestDir(d, testExt))
                    {
                        // Expected result
                        String res = File.ReadAllText(d.GetFiles(resExt)[0].FullName).Trim();
                        // Parsing
                        bool ret = Program.parseProgram(d.GetFiles(testExt)[0].FullName.Trim());
                        if (res.ToLower().Equals(ret.ToString().ToLower()))
                        {
                            Console.WriteLine("Test {0} SUCCED!", d.Name);
                        }
                        else
                        {
                            Console.WriteLine("Test {0} FAILED!", d.Name);
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
