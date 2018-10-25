%using SLangScanner

%namespace SLangParser
%visibility internal
%output=SLangParser.cs

%start module

%token A

%%

module
        : A
        ;

%%

internal Parser(Scanner scnr) : base(scnr) { }
