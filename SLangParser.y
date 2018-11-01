/**
 * Parser spec for SLang
 * Process with > GPPG /gplex gplex.y
 */
%output=SLangParser.cs

%using SLangScanner

%namespace SLangParser
%visibility internal

%start module

%union {
    public string val;
}

// =============== TOKENS ===============

// Separators
%token COMMA
%token DOT
%token SEMICOLON
%token COLON
%token LPAREN
%token RPAREN
%token LBRACKET
%token RBRACKET
//%token ASSIGNMENT
%token GENERATOR
%token RIGHT_ARROW
%token QUESTION

// Operators
%token PLUS
%token MINUS
%token ASTERISK
%token SLASH
%token DBL_ASTERISK
%token VERTICAL
%token DBL_VERTICAL
%token AMPERSAND
%token DBL_AMPERSAND
%token CARET
%token TILDE
%token LESS
%token LESS_EQUALS
%token GREATER
%token GREATER_EQUALS
%token EQUALS
%token SLASH_EQUALS
%token LESS_GREATER
%token COLON_EQUALS

// Keywords
%token AND_THEN
%token OR_ELSE
%token ABSTRACT
%token ALIAS
%token AS
%token BREAK
%token CHECK
%token CONCURRENT
%token CONST
%token DEEP
%token DO
%token ELSE
%token ELSIF
%token END
%token ENSURE
%token EXTEND
%token EXTERNAL
%token FINAL
%token FOREIGN
%token HIDDEN
%token IF
%token IN
%token INIT
%token INVARIANT
%token IS
%token LOOP
%token NEW
%token NOT
%token OLD
%token OVERRIDE
%token PURE
%token RAISE
%token REF
%token REQUIRE
%token RETURN
%token ROUTINE
%token SAFE
%token SUPER
%token THEN
%token THIS
%token UNIT
%token USE
%token VAL
%token WHILE

// Value-dependent tokens
%token IDENTIFIER
%token LITERAL

// ========== TYPE ASSIGNMENTS ==========

//%type

// ===== ASSOCIATIVITY & PRECEDENCE =====

//%left %right %nonassoc

%%

module
        : /* empty */
		| module IDENTIFIER  { Console.WriteLine($2.val); }
		| module LITERAL  { Console.WriteLine($2.val); }
        ;

%%

internal Parser(Scanner scnr) : base(scnr) { }
