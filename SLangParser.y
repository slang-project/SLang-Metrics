/**
 * Parser spec for SLang
 * Process with > GPPG /gplex gplex.y
 */

%output=SLangParser.cs

%using SLangScanner

%namespace SLangParser
%visibility internal

%start module

%token A

%%

module
        : A
        ;

%%

internal Parser(Scanner scnr) : base(scnr) { }
