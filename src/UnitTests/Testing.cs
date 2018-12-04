using SLangLookaheadScanner;
using SLangParser;
using LanguageElements;
using Metrics;
using System;
using System.IO;
using System.Diagnostics;

namespace SLangTests
{
    public class Testing
    {
        private const string TESTS_FOLDER = "testCases\\";
        private const string FILE_EXTENSION = ".slang";

        /*
         * Use argument /tests in console command.
         */
        public static bool isTestingMode = false; // TODO make false while release!

        public static bool runTests()
        {
            ccTest1();
            commonProgramTest();
            slangUnitTest();
            javaCodeParsingTest();
            emptyFileTest();

            Console.WriteLine();
            // If program achieved this line, tests completed successfully
            return true;
        }

        public static void ccTest1()
        {
            String testName = "cyclomatic_complexity";
            //MetricCollector metrics = parseCode(testName + FILE_EXTENSION, false); //Example how to preview failed test case
            MetricCollector metrics = parseCode(testName + FILE_EXTENSION, true);
            Assert(6 == metrics.parsedModule.getCC(), testName, "Wrong CC value");
        }

        public static void commonProgramTest()
        {
            String testName = "usual_slang_program";
            MetricCollector metrics = parseCode(testName + FILE_EXTENSION, true);
        }

        public static void slangUnitTest()
        {
            String testName = "slang_unit";
            MetricCollector metrics = parseCode(testName + FILE_EXTENSION, true);
        }

        public static void javaCodeParsingTest()
        {
            String testName = "java_code.java";
            MetricCollector metrics = parseCode(testName, false);
        }

        public static void emptyFileTest()
        {
            String testName = "empty_program";
            MetricCollector metrics = parseCode(testName + FILE_EXTENSION, true);
        }

        private static MetricCollector parseCode(string fileName, bool mustSuccess)
        {
            string errorMsg = "Test: {0}\nFAILED".Replace("{0}", fileName);
            DirectoryInfo dir = new DirectoryInfo(TESTS_FOLDER);

            Assert(checkFileExitst(dir, fileName), fileName, errorMsg);
            Console.WriteLine("\nTest: {0}", fileName);

            String codeFile = dir.GetFiles(fileName)[0].FullName;

            MetricCollector metricCollector = null;
            try
            {
                metricCollector = new MetricCollector(codeFile);
                Assert(mustSuccess, fileName, errorMsg);
            }
            catch (ParsingFailedException e)
            {
                Assert(!mustSuccess, fileName, errorMsg);
            }
            return metricCollector;
        }

        private static void Assert(bool condition, string testName, string errorMsg)
        {
            Debug.Assert(condition, errorMsg);
            if (!condition)
                Console.WriteLine("FAILED");
            else
                Console.WriteLine("success assert");
        }

        /*
         * Check if file with @fileName exists in @dir directory
         */
        private static bool checkFileExitst(DirectoryInfo dir, String filename)
        {
            FileInfo[] files = dir.GetFiles(filename);
            if (files.Length == 1)
            {
                return true;
            }
            else
            {
                if (files.Length == 0)
                    Console.WriteLine("There are no file {0} in {1}!", filename, dir.FullName);
                else
                    Console.WriteLine("There are too many file {0} in {1}!", filename, dir.FullName);
                return false;
            }
        }
    }
}
