/**
 * Parser spec for SLang
 * Process with > GPPG /gplex gplex.y
 */

%output=SLangParser.cs

%using SLangScanner

%namespace SLangParser
%visibility internal

%start module

%%

module
        : /* empty */
        ;

%%

internal Parser(Scanner scnr) : base(scnr) { }
