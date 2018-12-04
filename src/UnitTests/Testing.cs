using SLangLookaheadScanner;
using SLangParser;
using System;
using System.IO;
using LanguageElements;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Metrics;

namespace SLangTests
{
    [TestClass]
    public class Testing
    {
        // Need to back in path due to tests run in such a way
        private const string TESTS_FOLDER = "..\\..\\..\\testCases\\";

        /*
         * Use argument /tests in console command.
         */
        public static bool isTestingMode = false; // TODO make false while release!

        public bool runTests()
        { 
            ccTest1();
            commonProgramTest();
            slangUnitTest();
            javaCodeParsingTest();
            emptyFileTest();

            // If program achieved this line, tests completed successfully
            return true;
        }

        [TestMethod]
        public void ccTest1()
        {
            Module m = parseCode("cyclomatic_complexity.slang", true);
            Assert.AreEqual(6, m.getCC());
        }

        [TestMethod]
        public void commonProgramTest()
        {
            Module m = parseCode("usual_slang_program.slang", true);
        }

        [TestMethod]
        public void slangUnitTest()
        {
            Module m = parseCode("slang_unit.slang", true);
        }

        [TestMethod]
        public void javaCodeParsingTest()
        {
            Module m = parseCode("java_code.java", false);
        }

        [TestMethod]
        public void emptyFileTest()
        {
            Module m = parseCode("empty_program.slang", true);
        }

        private static Module parseCode(string fileName, bool mustSuccess)
        {
            DirectoryInfo dir = new DirectoryInfo(TESTS_FOLDER);
            Assert.IsTrue(checkFileExitst(dir, fileName));
            Console.WriteLine("\nTest: {0}", fileName);

            String codeFile = dir.GetFiles(fileName)[0].FullName;

            MetricCollector metricCollector = new MetricCollector(codeFile);
            Assert.AreEqual(mustSuccess, metricCollector.IsParsingSuccessful());
            return metricCollector.parsedModule;
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