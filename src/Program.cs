﻿using SLangLookaheadScanner;
using SLangParser;
using System;
using System.IO;
using LanguageElements;

namespace SLangMetrics
{
    class Program
    {

        public static CompilationUnit parsedProgram;

        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Usage: <ProgramName> <InputFileName> OR <ProgramName> /testing");
                Environment.Exit(1);
            }
            if (Testing.isTestingMode || Array.Exists<String>(args, s => s.ToLower().Contains("/test")))
            {
                Testing.runTests();
            }
            else
            {
                Console.WriteLine(parseProgram(args[0]));
            }
        }

        public static bool parseProgram(String filePath)
        {
            FileStream file = new FileStream(filePath, FileMode.Open);
            Scanner scanner = new Scanner(file);
            Parser parser = new Parser(scanner);
            bool res = parser.Parse();
            file.Close();
            return res;
        }
    }
}