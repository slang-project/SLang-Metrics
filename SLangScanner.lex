/**
 * SLangScanner.lex file, Version 0.1.0
 *
 * Expected file format is Unicode. In the event that no 
 * byte order mark prefix is found, revert to raw bytes.
 */
%option classes, codepage:raw, stack, unicode, out:SLangScanner.cs

%using SLangParser;
%namespace SLangScanner

%visibility internal

%x COMMENT

WS	[\u0020\u0009\u000D\u000A]

%%

{WS}               { /* ignore whitespaces */ }

"//"[^\n]*$        { /* ignore inline comment */ }

"/*"               { yy_push_state(COMMENT); }
<COMMENT> {
  "/*"             { yy_push_state(COMMENT); }
  "*/"             { yy_pop_state(); }
  (.|\n)           { /* ignore block comment content */ }
}

<COMMENT><<EOF>>   |
.                  { return (int) Tokens.error; }

%%
