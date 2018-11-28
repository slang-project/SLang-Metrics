/**
 * Parser spec for SLang
 * Process with > GPPG /gplex gplex.y
 */
%output=SLangParser.cs

%using SLangLookaheadScanner

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

// Tokens for lookahead
%token WHILE_POSTTEST
%token FUNCTION_ID
%token LOOP_ID

// ========== TYPE ASSIGNMENTS ==========

//%type

// ===== ASSOCIATIVITY & PRECEDENCE =====

// Lower priority

%nonassoc JUST_HIDDEN
%nonassoc FINAL

%nonassoc JUST_EXPRESSION
%nonassoc COLON_EQUALS

%left LESS_GREATER
%left DBL_VERTICAL
%left DBL_AMPERSAND
%left VERTICAL
%left CARET
%left AMPERSAND
%left EQUALS SLASH_EQUALS
%left LESS LESS_EQUALS GREATER GREATER_EQUALS
%left PLUS MINUS
%left ASTERISK SLASH
%left DBL_ASTERISK

// Higher priority

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
//  | PredicateSeq SEMICOLON Predicate
    ;  // TODO check, shift/reduce of semicolon

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
//  |                          Type
    | GenericArgumentSeq COMMA Expression
//  | GenericArgumentSeq COMMA Type
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
    : UnaryExpression
    | Expression PLUS           Expression
    | Expression MINUS          Expression
    | Expression ASTERISK       Expression
    | Expression SLASH          Expression
    | Expression DBL_ASTERISK   Expression
    | Expression VERTICAL       Expression
    | Expression DBL_VERTICAL   Expression
    | Expression AMPERSAND      Expression
    | Expression DBL_AMPERSAND  Expression
    | Expression CARET          Expression
    | Expression LESS           Expression
    | Expression LESS_EQUALS    Expression
    | Expression GREATER        Expression
    | Expression GREATER_EQUALS Expression
    | Expression EQUALS         Expression
    | Expression SLASH_EQUALS   Expression
    | Expression LESS_GREATER   Expression
    ;  // TODO probably shift operations

UnaryExpression
    : SecondaryExpression
    | TILDE UnaryExpression
    | PLUS  UnaryExpression
    | MINUS UnaryExpression
    ;

SecondaryExpression
    : PrimaryExpression
    | SecondaryExpression LPAREN RPAREN
    | SecondaryExpression Tuple
    | SecondaryExpression DOT PrimaryExpression
    ;  // TODO remove shift/reduce (shift is correct)

PrimaryExpression
    : LITERAL  // TODO
    | TypeOrIdentifier
//  | OperatorSign  // TODO consider later
    | THIS
    | SUPER
//  | SUPER UnitTypeName  // TODO
//  | RETURN  // TODO review
    | OLD
//  | OLD Expression  // TODO review
    | Tuple
//  | LPAREN Expression RPAREN  // reduce/reduce with Tuple
    ;

TypeOrIdentifier
    : Type
//  | IDENTIFIER  // reduce/reduce conflict with `Type: IDENTIFIER;`
    ;

Tuple
    : LPAREN TupleElementSeq RPAREN
    ;

TupleElementSeq
    :                           TupleElement
    | TupleElementSeq COMMA     TupleElement
    | TupleElementSeq SEMICOLON TupleElement
    ;

TupleElement
    : Expression
    | Expression TypeAndInit  // TODO remove this Ad-Hoc
//  | VariableDeclaration  // TODO consider later
    ;

// Statement ///////////////////////////////////////////////////////////////////////////////////////////////////////////

Block
    : PreconditionOpt DO NONE              PostconditionOpt ExceptionHandlerSeqOpt END
    | PreconditionOpt DO BlockMemberSeqOpt PostconditionOpt ExceptionHandlerSeqOpt END
    ;

BlockMemberSeqOpt
    : /* empty */
    | BlockMemberSeqOpt BlockMember
    ;

BlockMember
    : NestedBlockMember
    | Block  // Because `Statement: Block;` was removed
    ;

ExceptionHandlerSeqOpt
    : /* empty */
    | WHEN Expression NestedBlock
    ;  // TODO review

NestedBlock
    : PreconditionOpt    NestedBlockMemberSeqOpt PostconditionOpt ExceptionHandlerSeqOpt
    | PreconditionOpt DO NestedBlockMemberSeqOpt PostconditionOpt ExceptionHandlerSeqOpt
    ;

NestedBlockMemberSeqOpt
    : /* empty */
    | NestedBlockMember BlockMemberSeqOpt
    ;

NestedBlockMember
    : Statement
    | UnitDeclaration
    | RoutineDeclaration
    | VariableDeclaration
    ;

Statement
    : SEMICOLON
//  | Block  // Conflicts when used in NestedBlock
    | Assignment
    | Expression %prec JUST_EXPRESSION
    | IfStatement
    | LoopStatement
    | BreakStatement
    | ValueLossStatement
    | ReturnStatement
    | RaiseStatement
    ;  // TODO review

Assignment
    : Expression COLON_EQUALS Expression
    ;

IfStatement
    : IF Expression            Block ElseIfClauseSeqOpt ElseClauseOpt END
    | IF Expression THEN NestedBlock ElseIfClauseSeqOpt ElseClauseOpt END
    ;

ElseIfClauseSeqOpt
    : /* empty */
    | ElseIfClauseSeqOpt ElseIfClause
    ;

ElseIfClause
    : ELSIF Expression            Block
    | ELSIF Expression THEN NestedBlock
    ;

ElseClauseOpt
    : /* empty */
    | ELSE NestedBlock
    ;

LoopStatement
    :                                LOOP NestedBlock END
    |               WHILE Expression LOOP NestedBlock END
    |               WHILE Expression            Block END
    |               LOOP NestedBlock WHILE_POSTTEST Expression END
    | LOOP_ID COLON                  LOOP NestedBlock END
    | LOOP_ID COLON WHILE Expression LOOP NestedBlock END
    | LOOP_ID COLON WHILE Expression            Block END
    | LOOP_ID COLON LOOP NestedBlock WHILE_POSTTEST Expression END
    ;

BreakStatement
    : BREAK
//  | BREAK IDENTIFIER  // FIXME shift/reduce
    ;

ValueLossStatement
    : QUESTION IDENTIFIER
    ;

ReturnStatement
    : RETURN
//  | RETURN Expression  // FIXME shift/reduce
    ;

RaiseStatement
    : RAISE
//  | RAISE Expression  // FIXME shift/reduce
    ;

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

OperatorRoutineDeclaration
    :                  OperatorRoutineName GenericFormalsOpt RoutineParameters ReturnTypeOpt UseDirectiveSeqOpt RoutineBody
    | RoutineSpecifier OperatorRoutineName GenericFormalsOpt RoutineParameters ReturnTypeOpt UseDirectiveSeqOpt RoutineBody
    ;  // TODO change UseSeq to just Use; review

RoutineSpecifier
    : PURE
    | SAFE
    | ABSTRACT
    | OVERRIDE
    ;  // TODO review

RoutineName
    : FUNCTION_ID
//  | OperatorSign AliasNameOpt
//  | COLON_EQUALS
    | LPAREN RPAREN
    ;

OperatorRoutineName
    : OperatorSign AliasNameOpt
    | COLON_EQUALS
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
    : HIDDEN %prec JUST_HIDDEN
    | HIDDEN FINAL
    ;

UnitMember
    : SEMICOLON  // All other statements are restricted
    | UnitDeclaration
    | RoutineDeclaration
    | VariableDeclaration
    | ConstObjectDeclaration
    | InitializerDeclaration
    | OperatorRoutineDeclaration  // TODO review
    ;  // TODO shift/reduce of OperatorRoutineDeclaration with Expression

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
