%using SLangScanner

%namespace SLangParser
%visibility internal

%start module

%%

module
        : /* empty */
        ;

%%

Parser(Scanner scnr) : base(scnr) { }
