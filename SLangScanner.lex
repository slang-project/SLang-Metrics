/* 
 *  SLangScanner.lex file, Version 0.1.0
 *
 *  Expected file format is Unicode. In the event that no 
 *  byte order mark prefix is found, revert to raw bytes.
 */
%option classes, codepage:raw, stack, unicode, out:SLangScanner.cs

%using SLangParser;
%namespace SLangScanner

%visibility internal

%%

a {
    return (int) Tokens.A;
}

%%
