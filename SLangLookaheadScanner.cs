using SLangParser;
using System.Collections.Generic;
using System.IO;

namespace SLangLookaheadScanner
{
    internal sealed partial class Scanner : ScanBase
    {
        SLangScanner.Scanner origScanner;
        Stack<int> stack;
        internal Scanner(Stream file)
        {
            origScanner = new SLangScanner.Scanner(file);
            stack = new Stack<int>();
        }
        public override int yylex()
        {
            int curToken;
            if (stack.Count > 0)
            {
                curToken = stack.Pop();
            }
            else
            {
                curToken = origScanner.yylex();
            }
            switch (curToken)
            {
                case (int)Tokens.IDENTIFIER:
                    return curToken;  // TODO lookahead after IDENTIFIER
                default:
                    return curToken;
            }
        }
    }
}
