/**
 * Parser spec for SLang
 * Process with > GPPG /gplex gplex.y
 */
%output=SLangParser.cs

%using SLangScanner

%namespace SLangParser
%visibility internal

%start CompilationUnit

%YYSTYPE string

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
%token SINGLE_ARROW  // TODO add to specification
%token DOUBLE_ARROW
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
%token NONE  // TODO add to specification
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
%token WHEN
%token WHILE

// Value-dependent tokens
%token IDENTIFIER
%token LITERAL

// ========== TYPE ASSIGNMENTS ==========

//%type

// ===== ASSOCIATIVITY & PRECEDENCE =====

//%left %right %nonassoc

%%

// General /////////////////////////////////////////////////////////////////////////////////////////////////////////////

CompilationUnit
    : UseDirectiveSeqOpt BlockMemberSeqOpt
    ;

// Primitives ***

IdentifierSeq
    :                     IDENTIFIER
    | IdentifierSeq COMMA IDENTIFIER
    ;

CompoundName
    :                  IDENTIFIER
    | CompoundName DOT IDENTIFIER
    ;

// Use directive ***

UseDirectiveSeqOpt
    : /* empty */
    | UseDirectiveSeqOpt USE UsedUnitSeq
    ;

UseClauseOpt
    : /* empty */
    | USE       UsedUnitSeq
    | USE CONST UsedUnitSeq
    ;

UsedUnitSeq
    :                   UsedUnit
    | UsedUnitSeq COMMA UsedUnit
    ;

UsedUnit
    : UnitTypeName
    | UnitTypeName AS IDENTIFIER
    ;

// Generic formals ***

GenericFormalsOpt
    : /* empty */
    | LBRACKET GenericFormalSeq RBRACKET
    ;

GenericFormalSeq
    :                            GenericFormal
    | GenericFormalSeq COMMA     GenericFormal
    | GenericFormalSeq SEMICOLON GenericFormal
    ;

GenericFormal
    : IDENTIFIER
    | IDENTIFIER SINGLE_ARROW UnitTypeName
    | IDENTIFIER SINGLE_ARROW UnitTypeName INIT
    | IDENTIFIER SINGLE_ARROW UnitTypeName INIT RoutineParameters
    | IDENTIFIER COLON Type
    ;

// Contracts ***

PreconditionOpt
    : /* empty */
    | REQUIRE      PredicateSeq
    | REQUIRE ELSE PredicateSeq
    ;

PostconditionOpt
    : /* empty */
    | ENSURE      PredicateSeq
    | ENSURE THEN PredicateSeq
    ;

InvariantOpt
    : /* empty */
    | INVARIANT PredicateSeq
    ;

PredicateSeq
    :                        Predicate
    | PredicateSeq COMMA     Predicate
    | PredicateSeq SEMICOLON Predicate
    ;

Predicate
    :                  Expression
    | IDENTIFIER COLON Expression
    ;

// Type ***

Type
    : UnitType/*
    | AnchorType
    | MultiType
    | TupleType
    | RangeType
    | RoutineType*/
    ;  // TODO a lot of conflicts!

UnitType
    :            UnitTypeName
    | REF        UnitTypeName
    | VAL        UnitTypeName
    | CONCURRENT UnitTypeName
    ;

UnitTypeName
    : IDENTIFIER
    | IDENTIFIER GenericArgumentClause
    ;

GenericArgumentClause
    : LBRACKET GenericArgumentSeq RBRACKET
    ;

GenericArgumentSeq
    :                          Expression
    | GenericArgumentSeq COMMA Expression
    ;
/*
AnchorType
    : AS THIS
    | AS IDENTIFIER
    | AS IDENTIFIER RoutineParameters
    ;

MultiType
    :                    UnitType
    | MultiType VERTICAL UnitType
    ;

TupleType
    : LPAREN               RPAREN
    | LPAREN TupleFieldSeq RPAREN
    ;

TupleFieldSeq
    :                         TypeField
    | TupleFieldSeq COMMA     TypeField
    | TupleFieldSeq SEMICOLON TypeField
    ;

TypeField
    : IdentifierSeq                IS Expression
    | IdentifierSeq COLON UnitType
    | IdentifierSeq COLON UnitType IS Expression
    ;

RangeType
    : Expression GENERATOR Expression
    ;

RoutineType
    : LPAREN                RPAREN Block
    | LPAREN RoutineFormals RPAREN Block
    ;  // TODO return type
*/
// Expression //////////////////////////////////////////////////////////////////////////////////////////////////////////

Expression
    : LITERAL  // TODO
    | Type
    ;

// Statement ///////////////////////////////////////////////////////////////////////////////////////////////////////////

Block
    : PreconditionOpt DO NONE              ExceptionHandlerSeqOpt PostconditionOpt END
    | PreconditionOpt DO BlockMemberSeqOpt ExceptionHandlerSeqOpt PostconditionOpt END
    ;

BlockMemberSeqOpt
    : /* empty */
    | BlockMemberSeqOpt BlockMember
    ;

BlockMember
    : Statement
    | UnitDeclaration
    | RoutineDeclaration
    | VariableDeclaration
    ;

ExceptionHandlerSeqOpt
    : /* empty */
    | WHEN Expression NestedBlock
    ;  // TODO review

NestedBlock
    : PreconditionOpt    BlockMemberSeqOpt ExceptionHandlerSeqOpt PostconditionOpt
    | PreconditionOpt DO BlockMemberSeqOpt ExceptionHandlerSeqOpt PostconditionOpt
    ;  // TODO shift/reduce with postcondition of usual block

Statement
    : SEMICOLON/*
    | Assignment
    | Expression
    | If
    | Loop
    | Break
    | ValueLoss
    | Return
    | Raise
    | Block*/
    ;  // TODO review

// Variable ////////////////////////////////////////////////////////////////////////////////////////////////////////////

VariableDeclaration
    :                   IdentifierSeq TypeAndInit
    | VariableSpecifier IdentifierSeq TypeAndInit
    ;

VariableSpecifier
    : CONST
    | CONST DEEP
    ;

TypeAndInit
    :                         IS Expression
    | COLON          Type
    | COLON QUESTION UnitType
    | COLON          Type     IS Expression
    | COLON QUESTION UnitType IS Expression
    ;  // TODO review

// Routine /////////////////////////////////////////////////////////////////////////////////////////////////////////////

RoutineDeclaration
    :                  RoutineName GenericFormalsOpt RoutineParameters ReturnTypeOpt UseDirectiveSeqOpt RoutineBody
    | RoutineSpecifier RoutineName GenericFormalsOpt RoutineParameters ReturnTypeOpt UseDirectiveSeqOpt RoutineBody
    ;  // TODO change UseSeq to just Use

RoutineSpecifier
    : PURE
    | SAFE
    | ABSTRACT
    | OVERRIDE
    ;  // TODO review

RoutineName
    : IDENTIFIER
    | OperatorSign AliasNameOpt
    | COLON_EQUALS
    | LPAREN RPAREN
    ;

OperatorSign
    : PLUS  // TODO
    ;

RoutineParameters
    : LPAREN                RPAREN
    | LPAREN RoutineFormals RPAREN
    ;

RoutineFormals
    :                          VariableDeclaration
    | RoutineFormals COMMA     VariableDeclaration
    | RoutineFormals SEMICOLON VariableDeclaration
    ;

ReturnTypeOpt
    : /* empty */
    | COLON Type
    ;

RoutineBody
    : Block
    | DOUBLE_ARROW Statement
    | IS ABSTRACT
    | IS FOREIGN
    ;

// Unit ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

UnitDeclaration
    : UnitSpecifiersOpt UNIT CompoundName UnitDeclarationAdditions    UnitMemberSeqOpt InvariantOpt END
    | UnitSpecifiersOpt UNIT CompoundName UnitDeclarationAdditions IS UnitMemberSeqOpt InvariantOpt END
    ;

UnitSpecifiersOpt
    : /* empty */
    |       UnitSpecifier
    | FINAL UnitSpecifier
    ;

UnitSpecifier
    : REF
    | VAL
    | CONCURRENT
    | ABSTRACT
//  | EXTEND  // TODO review
    ;

UnitDeclarationAdditions
    : AliasNameOpt GenericFormalsOpt InheritClauseOpt UseClauseOpt
    ;

AliasNameOpt
    : /* empty */
    | ALIAS IDENTIFIER
    ;

InheritClauseOpt
    : /* empty */
    | EXTEND BaseUnitSeq
    ;

BaseUnitSeq
    :                   BaseUnitName
    | BaseUnitSeq COMMA BaseUnitName
    ;

BaseUnitName
    :       Type
    | TILDE Type
    ;

UnitMemberSeqOpt
    : /* empty */
    | UnitMemberSeqOpt                     UnitMember
    | UnitMemberSeqOpt UnitMemberSpecifier UnitMember
    ;

UnitMemberSpecifier
    : HIDDEN
    | HIDDEN FINAL  // TODO shift/reduce with FINAL of UnitDeclaration
    ;

UnitMember
    : SEMICOLON  // All other statements are restricted
    | UnitDeclaration
    | RoutineDeclaration
    | VariableDeclaration
    | ConstObjectDeclaration
    | InitializerDeclaration
    ;

ConstObjectDeclaration
    : CONST IS ConstObjectSeq END
    ;

ConstObjectSeq
    :                      ConstObject
    | ConstObjectSeq COMMA ConstObject
    ;

ConstObject
    : Expression
    ;

InitializerDeclaration
    : GENERATOR  // TODO
    ;

%%

internal Parser(Scanner scnr) : base(scnr) { }
