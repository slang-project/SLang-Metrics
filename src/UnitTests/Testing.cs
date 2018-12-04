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
            constTest();
            diamond_porblemTest();
            genericsTest();
            not_implemented_yetTest();
            some_codeTest();
            contractsTest();
            useTest(); // For now fails, but testing system working correctly!

            Console.WriteLine();
            // If program achieved this line, tests completed successfully
            return true;
        }
        
        private static void useTest()
        {
            string testName = "use";
            MetricCollector metrics = parseCode(testName + FILE_EXTENSION, true);
        }
        
        private static void contractsTest()
        {
            string testName = "contracts";
            MetricCollector metrics = parseCode(testName + FILE_EXTENSION, true);
        }
        
        private static void some_codeTest()
        {
            string testName = "some_code";
            MetricCollector metrics = parseCode(testName + FILE_EXTENSION, true);
        }
        
        private static void not_implemented_yetTest()
        {
            string testName = "not_implemented_yet";
            MetricCollector metrics = parseCode(testName + FILE_EXTENSION, false);
        }
        
        private static void genericsTest()
        {
            string testName = "generics";
            MetricCollector metrics = parseCode(testName + FILE_EXTENSION, true);
        }

        
        private static void diamond_porblemTest()
        {
            string testName = "diamond_problem";
            MetricCollector metrics = parseCode(testName + FILE_EXTENSION, true);
        }

        private static void constTest()
        {
            string testName = "const";
            MetricCollector metrics = parseCode(testName + FILE_EXTENSION, true);
        }

        private static void ccTest1()
        {
            string testName = "cyclomatic_complexity";
            //MetricCollector metrics = parseCode(testName + FILE_EXTENSION, false); //Example how to preview failed test case
            MetricCollector metrics = parseCode(testName + FILE_EXTENSION, true);
            Assert(6 == metrics.parsedModule.getCC(), testName, "Wrong CC value");
        }

        private static void commonProgramTest()
        {
            string testName = "usual_slang_program";
            MetricCollector metrics = parseCode(testName + FILE_EXTENSION, true);
        }

        private static void slangUnitTest()
        {
            string testName = "slang_unit";
            MetricCollector metrics = parseCode(testName + FILE_EXTENSION, true);
        }

        private static void javaCodeParsingTest()
        {
            string testName = "java_code.java";
            MetricCollector metrics = parseCode(testName, false);
        }

        private static void emptyFileTest()
        {
            string testName = "empty_program";
            MetricCollector metrics = parseCode(testName + FILE_EXTENSION, true);
        }

        private static MetricCollector parseCode(string fileName, bool mustSuccess)
        {
            string errorMsg = "Test: {0}\nFAILED".Replace("{0}", fileName);
            DirectoryInfo dir = new DirectoryInfo(TESTS_FOLDER);

            Assert(checkFileExitst(dir, fileName), fileName, errorMsg);
            Console.WriteLine("\nTest: {0}", fileName);

            string codeFile = dir.GetFiles(fileName)[0].FullName;

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
        private static bool checkFileExitst(DirectoryInfo dir, string filename)
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