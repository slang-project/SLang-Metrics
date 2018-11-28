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

        // TODO: think about need of lookahead inside the buffer
        public override int yylex()
        {
            if (queue.Count > 0)
            {
                // TODO skip newlines (if they will be considered)
                return queue.Dequeue();
            }

            int curToken = origScanner.yylex();
            int looked;

            switch (curToken)
            {
                case (int)Tokens.WHILE:
                    do
                    {
                        looked = origScanner.yylex();
                        queue.Enqueue(looked);
                    } while (!IsEOF(looked) && !IsAfterWhileExpression(looked));
                    return looked == (int)Tokens.END ? (int)Tokens.WHILE_POSTTEST : curToken;

                case (int)Tokens.IDENTIFIER:
                    looked = origScanner.yylex();
                    queue.Enqueue(looked);
                    if (looked == (int)Tokens.COLON)
                    {
                        looked = origScanner.yylex();
                        queue.Enqueue(looked);
                        // TODO skip newlines (if they will be considered)
                        return looked == (int)Tokens.WHILE || looked == (int)Tokens.LOOP
                                ? (int)Tokens.LOOP_ID : curToken;
                    }
                    if (looked == (int)Tokens.LBRACKET)
                    {
                        int bracket_counter = 1;
                        do
                        {
                            looked = origScanner.yylex();
                            queue.Enqueue(looked);
                            if (false)  // TODO functional object declaration
                            {
                                return curToken;
                            }
                            else if (looked == (int)Tokens.LBRACKET)
                            {
                                ++bracket_counter;
                            }
                            else if (looked == (int)Tokens.RBRACKET)
                            {
                                --bracket_counter;
                            }
                        } while (!IsEOF(looked) && bracket_counter != 0);
                        looked = origScanner.yylex();
                        queue.Enqueue(looked);
                    }
                    if (looked == (int)Tokens.LPAREN)
                    {
                        int parentheses_counter = 1;
                        do
                        {
                            looked = origScanner.yylex();
                            queue.Enqueue(looked);
                            if (false)  // TODO functional object declaration
                            {
                                return curToken;
                            }
                            else if (looked == (int)Tokens.LPAREN)
                            {
                                ++parentheses_counter;
                            }
                            else if (looked == (int)Tokens.RPAREN)
                            {
                                --parentheses_counter;
                            }
                        } while (!IsEOF(looked) && parentheses_counter != 0);
                        looked = origScanner.yylex();
                        queue.Enqueue(looked);
                    }
                    else
                    {
                        return curToken;
                    }
                    return IsFunctionBodyBeginning(looked) ? (int)Tokens.FUNCTION_ID : curToken;

                default:
                    return curToken;
            }
        }

        private bool IsEOF(int token)
        {
            return token == (int)Tokens.EOF;
        }

        private bool IsAfterWhileExpression(int token)
        {
            return token == (int)Tokens.DO || token == (int)Tokens.LOOP || token == (int)Tokens.END;
        }

        private bool IsFunctionBodyBeginning(int token)
        {
            if (token == (int)Tokens.IS)
            {
                int next = origScanner.yylex();
                queue.Enqueue(next);
                return next == (int)Tokens.ABSTRACT || next == (int)Tokens.FOREIGN;
            }
            return token == (int)Tokens.DO || token == (int)Tokens.COLON || token == (int)Tokens.DOUBLE_ARROW;
        }
    }
}
