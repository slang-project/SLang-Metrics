using SLangParser;
using System;
using System.Collections.Generic;
using System.IO;

namespace SLangLookaheadScanner
{
    internal sealed class ScannerFlags
    {
        public bool isInsideUnit { get; set; }

        public ScannerFlags()
        {
            this.isInsideUnit = false;
        }
    }

    internal sealed partial class Scanner : ScanBase
    {
        private SLangScanner.Scanner origScanner;
        private Queue<int> queue;
        private ScannerFlags scannerFlags;

        internal Scanner(Stream file, ref ScannerFlags scannerFlags)
        {
            this.origScanner = new SLangScanner.Scanner(file);
            this.queue = new Queue<int>();
            this.scannerFlags = scannerFlags;
        }

        private int LookNext()
        {
            int looked = origScanner.yylex();
            queue.Enqueue(looked);
            return looked;
        }

        private int LookNextNonNewLine()
        {
            int looked = LookNext();
            return looked == (int)Tokens.NEW_LINE ? LookNext() : looked;
        }

        // TODO: think about need of lookahead inside the buffer
        public override int yylex()
        {
            if (queue.Count > 1)
            {
                while (queue.Count > 0 && queue.Peek() == (int)Tokens.NEW_LINE)
                {
                    queue.Dequeue();
                }
                if (queue.Count > 1)
                {
                    return queue.Dequeue();
                }
            }

            int curToken = queue.Count > 0 ? queue.Dequeue() : origScanner.yylex();
            int looked;

            if (scannerFlags.isInsideUnit && IsOperatorSign(curToken))
            {
                int resolved = ResolveIdentifier(curToken);
                return resolved == (int)Tokens.FUNCTION_ID ?
                        (int)Tokens.OP_AS_ROUTINE_NAME : resolved;
            }

            switch (curToken)
            {
                case (int)Tokens.IDENTIFIER:
                    return ResolveIdentifier(curToken);

                case (int)Tokens.WHILE:
                    do
                    {
                        looked = LookNext();
                    } while (!IsEOF(looked) && !IsAfterWhileExpression(looked));
                    return looked == (int)Tokens.END ? (int)Tokens.WHILE_POSTTEST : curToken;

                case (int)Tokens.BREAK:
                    if (LookNext() != (int)Tokens.IDENTIFIER)
                    {
                        return (int)Tokens.JUST_BREAK;
                    }
                    return curToken;

                case (int)Tokens.RETURN:
                    looked = LookNext();
                    if (looked == (int)Tokens.NEW_LINE || looked == (int)Tokens.SEMICOLON)
                    {
                        return (int)Tokens.JUST_RETURN;
                    }
                    return curToken;

                case (int)Tokens.RAISE:
                    looked = LookNext();
                    if (looked == (int)Tokens.NEW_LINE || looked == (int)Tokens.SEMICOLON)
                    {
                        return (int)Tokens.JUST_RAISE;
                    }
                    return curToken;

                case (int)Tokens.NEW_LINE:
                    return yylex();

                default:
                    return curToken;
            }
        }

        private bool IsOperatorSign(int token)
        {
            return token == (int)Tokens.PLUS
                    || token == (int)Tokens.MINUS
                    || token == (int)Tokens.ASTERISK
                    || token == (int)Tokens.SLASH
                    || token == (int)Tokens.DBL_ASTERISK
                    || token == (int)Tokens.VERTICAL
                    || token == (int)Tokens.DBL_VERTICAL
                    || token == (int)Tokens.AMPERSAND
                    || token == (int)Tokens.DBL_AMPERSAND
                    || token == (int)Tokens.CARET
                    || token == (int)Tokens.TILDE
                    || token == (int)Tokens.LESS
                    || token == (int)Tokens.LESS_EQUALS
                    || token == (int)Tokens.GREATER
                    || token == (int)Tokens.GREATER_EQUALS
                    || token == (int)Tokens.EQUALS
                    || token == (int)Tokens.SLASH_EQUALS
                    || token == (int)Tokens.LESS_GREATER;
//                  || token == (int)Tokens.COLON_EQUALS
        }

        private int ResolveIdentifier(int curToken)
        {
            int looked = LookNextNonNewLine();
            if (looked == (int)Tokens.ALIAS)
            {
                return (int)Tokens.OP_AS_ROUTINE_NAME;
            }
            if (looked == (int)Tokens.COLON)
            {
                looked = LookNextNonNewLine();
                return looked == (int)Tokens.WHILE || looked == (int)Tokens.LOOP
                        ? (int)Tokens.LOOP_ID : curToken;
            }
            if (looked == (int)Tokens.LBRACKET)
            {
                int bracket_counter = 1;
                do
                {
                    looked = LookNext();
                    // TODO: functional object declaration consideration
                    if (looked == (int)Tokens.LBRACKET)
                    {
                        ++bracket_counter;
                    }
                    else if (looked == (int)Tokens.RBRACKET)
                    {
                        --bracket_counter;
                    }
                } while (!IsEOF(looked) && bracket_counter != 0);
                looked = LookNextNonNewLine();
            }
            if (looked == (int)Tokens.LPAREN)
            {
                int parentheses_counter = 1;
                do
                {
                    looked = LookNext();
                    // TODO: functional object declaration consideration
                    if (looked == (int)Tokens.LPAREN)
                    {
                        ++parentheses_counter;
                    }
                    else if (looked == (int)Tokens.RPAREN)
                    {
                        --parentheses_counter;
                    }
                } while (!IsEOF(looked) && parentheses_counter != 0);
                looked = LookNextNonNewLine();
            }
            else
            {
                return curToken;
            }
            return IsFunctionBodyBeginning(looked) ? (int)Tokens.FUNCTION_ID : curToken;
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
