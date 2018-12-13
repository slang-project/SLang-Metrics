/**
 * Parser spec for SLang
 * Process with > GPPG /gplex SLangParser.y
 *
 * Known conflicts:
 *   shift/reduce of PLUS/MINUS/LPAREN when consequent Expression constructs may appear
 *   shift/reduce of WHEN (exception handling inside exception handling)
 *   shift/reduce of COLON_EQUALS (probably, when assignment appears as function body)
 */

%start Module

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
%token SINGLE_ARROW  // TODO: add to the specification
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
%token AND_THEN  // TODO: consider (no concrete specification given)
%token OR_ELSE  // TODO: consider (no concrete specification given)
%token ABSTRACT
%token ALIAS
%token AS
%token BREAK
%token CHECK  // TODO: remove from the specification
%token CONCURRENT
%token CONST
%token DEEP
%token DO
%token ELSE
%token ELSIF
%token END
%token ENSURE
%token EXTEND
%token EXTERNAL  // TODO: remove from the specification
%token FINAL
%token FOREIGN
%token HIDDEN
%token IF
%token IN  // TODO: consider (no concrete specification given)
%token INIT
%token INVARIANT
%token IS
%token LOOP
%token NEW
%token NONE  // TODO: add to the specification
//%token NOT  // TODO: remove from the specification
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
%token CONTRACT_LABEL
%token LOOP_LABEL
%token WHILE_POSTTEST
%token JUST_BREAK
%token JUST_RETURN
%token JUST_RAISE
%token FUNCTION_ID
%token OP_AS_ROUTINE_NAME

// ===== ASSOCIATIVITY & PRECEDENCE =====

// Lower priority

// Unit may be member of another Unit (each UnitMember may have specifier FINAL),
// but UnitDeclaration may have keyword FINAL itself.
%nonassoc JUST_HIDDEN
%nonassoc FINAL

// Make Expression explicitly part of the next assignment (if appears)
%nonassoc JUST_EXPRESSION
%nonassoc COLON_EQUALS

// Expression binary operators' precedence
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

Module
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
    : CompoundName
    | CompoundName AS IDENTIFIER
    ;  // TODO: review (should contain CompoundUnitTypeName)

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
    | IDENTIFIER SINGLE_ARROW UnitTypeName INIT RoutineFormals
    | IDENTIFIER COLON UnitTypeName
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
    ;  // TODO: check, shift/reduce of semicolon

Predicate
    :                      Expression
    | CONTRACT_LABEL COLON Expression
    ;

// Type ***

Type
    : UnitType
    | AnchorType
    | MultiType
    | TupleType
//  | RangeType  // TODO: introduce
//  | RoutineType  // TODO: introduce
    ;

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

AnchorType
    : AS THIS
    | AS IDENTIFIER
//  | AS IDENTIFIER RoutineParameters  // TODO: review
    ;

MultiType
    :  UnitType VERTICAL UnitType
    | MultiType VERTICAL UnitType
    ;

TupleType
    : LPAREN                 RPAREN
    | LPAREN TupleElementSeq RPAREN
    ;

TupleElementSeq
    :                           TupleElement
    | TupleElementSeq COMMA     TupleElement
    | TupleElementSeq SEMICOLON TupleElement
    ;

TupleElement
    : Expression
    | Expression TypeAndInit
//  | VariableDeclaration  // shift/reduce (cannot decide what to do with IdentifierSeq)
    ;
/*
RangeType
    : Expression GENERATOR Expression
    ;  // TODO: review

RoutineType
    : RoutineFormals Block
    ;  // TODO: return type
*/
// Expression //////////////////////////////////////////////////////////////////////////////////////////////////////////

Expression
    : UnaryExpression
//  | UnaryExpression IS UnitTypeName  // TODO: introduce
//  | UnaryExpression IN UnitTypeName  // TODO: introduce
//  | UnaryExpression IN Range  // TODO: introduce
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
    ;  // TODO: probably bit shift operations

UnaryExpression
    : SecondaryExpression
    | TILDE UnaryExpression
    | PLUS  UnaryExpression
    | MINUS UnaryExpression
    ;  // FIXME: shift/reduce of PLUS and MINUS

SecondaryExpression
    : PrimaryExpression
    | SecondaryExpression LPAREN RPAREN
    | SecondaryExpression Tuple
    | SecondaryExpression DOT PrimaryExpression
    ;  // FIXME: shift/reduce of function call

PrimaryExpression
    : LITERAL  // TODO
    | TypeOrIdentifier
//  | OperatorSign  // TODO: consider later
    | INIT  // TODO: review
    | THIS
    | SUPER
//  | SUPER UnitTypeName  // TODO
//  | RETURN  // TODO: review
    | OLD IDENTIFIER  // TODO: review, Expression was used in the specification
    | Tuple
//  | LPAREN Expression RPAREN  // reduce/reduce with Tuple
    ;

TypeOrIdentifier
    : UnitTypeName  // TODO: review
//  | IDENTIFIER  // reduce/reduce conflict with `Type: IDENTIFIER;`
    ;

Tuple
    : TupleType
//  | TupleValue  // reduce/reduce, all tuples are types
    ;
/*
Range
    : Expression GENERATOR Expression
    ;  // TODO: introduce
*/
// Statement ///////////////////////////////////////////////////////////////////////////////////////////////////////////

Block
    : PreconditionOpt DO BlockMemberSeqOpt PostconditionOpt ExceptionHandlerSeqOpt END
    ;

BlockMemberSeqOpt
    : /* empty */
    | BlockMemberSeqOpt BlockMember
    ;

BlockMember
    : NestedBlockMember
    | Block  // Because `Statement: Block;` was removed (see NestedBlock)
    ;

ExceptionHandlerSeqOpt
    : /* empty */
    | ExceptionHandlerSeqOpt ExceptionHandler
    ;

ExceptionHandler
    : WHEN Expression DO NestedBlock
    ;  // FIXME: shift/reduce when nested exception handling

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
    | NONE
//  | Block  // Conflicts when used in NestedBlock
    | Assignment
    | Expression %prec JUST_EXPRESSION  // TODO: change to function call
    | IfStatement
    | LoopStatement
    | BreakStatement
    | ValueLossStatement
    | ReturnStatement
    | RaiseStatement
    ;  // TODO: review

Assignment
    : SecondaryExpression COLON_EQUALS Expression
    ;  // FIXME: shift/reduce of COLON_EQUALS when `RoutineBody: DOUBLE_ARROW Statement;`

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
    : LoopLabelOpt                  LOOP NestedBlock END
    | LoopLabelOpt WHILE Expression LOOP NestedBlock END
    | LoopLabelOpt WHILE Expression      Block
    | LoopLabelOpt LOOP NestedBlock WHILE_POSTTEST Expression END
    ;

LoopLabelOpt
    : /* empty */
    | LOOP_LABEL COLON
    ;

BreakStatement
    : JUST_BREAK
    | BREAK IDENTIFIER
    ;

ValueLossStatement
    : QUESTION IDENTIFIER
    ;

ReturnStatement
    : JUST_RETURN
    | RETURN Expression
    ;

RaiseStatement
    : JUST_RAISE
    | RAISE Expression
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
    :                             IS Expression
    | COLON          Type
    | COLON QUESTION UnitType
    | COLON          Type IS Expression
    | COLON QUESTION UnitType     IS Expression
    ;  // TODO: review

// Routine /////////////////////////////////////////////////////////////////////////////////////////////////////////////

RoutineDeclaration
    :                  RoutineName GenericFormalsOpt RoutineFormals ReturnTypeOpt UseDirectiveSeqOpt RoutineBody
    | RoutineSpecifier RoutineName GenericFormalsOpt RoutineFormals ReturnTypeOpt UseDirectiveSeqOpt RoutineBody
    ;  // TODO: change UseDirectiveSeqOpt to UseClauseOpt?

OperatorRoutineDeclaration
    :                  OperatorRoutineName GenericFormalsOpt RoutineFormals ReturnTypeOpt UseDirectiveSeqOpt RoutineBody
    | RoutineSpecifier OperatorRoutineName GenericFormalsOpt RoutineFormals ReturnTypeOpt UseDirectiveSeqOpt RoutineBody
    ;  // TODO: review; change UseDirectiveSeqOpt to UseClauseOpt?

InitRoutineDeclaration
    :                  INIT GenericFormalsOpt RoutineFormals ReturnTypeOpt UseDirectiveSeqOpt RoutineBody
    | RoutineSpecifier INIT GenericFormalsOpt RoutineFormals ReturnTypeOpt UseDirectiveSeqOpt RoutineBody
    ;  // TODO: review; change UseDirectiveSeqOpt to UseClauseOpt?

RoutineSpecifier
    : PURE
    | SAFE
    | OVERRIDE
    | PURE OVERRIDE
    | SAFE OVERRIDE
    ;  // TODO: review

RoutineName
    : FUNCTION_ID
//  | OP_AS_ROUTINE_NAME AliasNameOpt
//  | COLON_EQUALS
    | LPAREN RPAREN  // TODO: review
    ;

OperatorRoutineName
    : OP_AS_ROUTINE_NAME AliasNameOpt
    | COLON_EQUALS
    ;

AliasNameOpt
    : /* empty */
    | ALIAS FUNCTION_ID
    ;

RoutineFormals
    : LPAREN                     RPAREN
    | LPAREN RoutineParameterSeq RPAREN
    ;

RoutineParameterSeq
    :                               VariableDeclaration
    | RoutineParameterSeq COMMA     VariableDeclaration
    | RoutineParameterSeq SEMICOLON VariableDeclaration
    ;

ReturnTypeOpt
    : /* empty */
    | COLON Type
    ;

RoutineBody
    : Block
    | DOUBLE_ARROW Statement  // TODO: review (conflicts with Assignment and Expression content)
    | IS ABSTRACT  // TODO: consider contracts
    | IS FOREIGN  // TODO: consider contracts
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
    ;

UnitDeclarationAdditions
    : UnitAliasNameOpt GenericFormalsOpt InheritClauseOpt UseClauseOpt
    ;

UnitAliasNameOpt
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
    :       UnitTypeName
    | TILDE UnitTypeName
    ;  // TODO: review (should contain CompoundUnitTypeName)

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
    | UnitMemberRoutineDeclaration
    | VariableDeclaration
    | ConstObjectDeclaration
    ;  // FIXME: shift/reduce of OperatorRoutineDeclaration with Expression

UnitMemberRoutineDeclaration
    : RoutineDeclaration
    | InitRoutineDeclaration
    | OperatorRoutineDeclaration
    ;

ConstObjectDeclaration
    : CONST IS                END
	| CONST IS ConstObjectSeq END
    ;

ConstObjectSeq
    :                      Expression
    | ConstObjectSeq COMMA Expression
    ;

%%
