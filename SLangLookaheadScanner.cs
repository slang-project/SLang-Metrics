using SLangParser;
using System.Collections.Generic;
using System.IO;

namespace SLangLookaheadScanner
{
    internal sealed partial class Scanner : ScanBase
    {
        SLangScanner.Scanner origScanner;
        Queue<int> queue;

        internal Scanner(Stream file)
        {
            origScanner = new SLangScanner.Scanner(file);
            queue = new Queue<int>();
        }

        public override int yylex()
        {
            int curToken, looked;
            if (queue.Count > 0)
            {
                curToken = queue.Dequeue();
            }
            else
            {
                curToken = origScanner.yylex();
            }
            switch (curToken)
            {
                case (int)Tokens.WHILE:
                    looked = origScanner.yylex();
                    while (!IsAfterWhileExpression(looked))
                    {
                        queue.Enqueue(looked);  // FIXME do not enqueue if already stored this token
                        looked = origScanner.yylex();
                    }
                    return (int)(looked == (int)Tokens.END ? Tokens.WHILE_POSTTEST : Tokens.WHILE);

                case (int)Tokens.IDENTIFIER:
                    looked = origScanner.yylex();
                    if (looked == (int)Tokens.RBRACKET)
                    {
                        CollectBracketContents();
                        looked = origScanner.yylex();
                    }
                    if (looked == (int)Tokens.LPAREN)
                    {
                        CollectParenthesesContent();
                        looked = origScanner.yylex();
                    }
                    else
                    {
                        return curToken;
                    }
                    while ()
                    {
                        queue.Enqueue(looked);
                        looked = origScanner.yylex();
                    }
                    return (int)(looked == (int)Tokens.END ? Tokens.WHILE_POSTTEST : Tokens.WHILE);

                default:
                    return curToken;
            }
        }

        private bool IsAfterWhileExpression(int token)
        {
            return token == (int)Tokens.DO || token == (int)Tokens.LOOP || token == (int)Tokens.END;
        }
    }
}
