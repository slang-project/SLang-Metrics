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
