/**
 * Parser spec for SLang
 * Process with > GPPG /gplex SLangParser.y
 */
%output=src\SLangParser.cs

%using LanguageElements
%using SLangLookaheadScanner
%using System.IO;

%namespace SLangParser
%visibility internal

%start Module

%union
{
    public string s;
    public string[] sa;
    public CompoundName cn;
    public LanguageElements.Type t;
    public UnitTypeName utn;
    public Block bl;
    public BlockMember bm;
    public Statement st;
    public LinkedList<BlockMember> llbm;
    public LinkedList<Block> llb;
    public VariableDeclaration vd;
    public RoutineDeclaration rd;
    public UnitDeclaration ud;
    public LinkedList<UnitName> llun;
    public UnitName un;
    public LinkedList<Declaration> lld;
    public Declaration dc;
}

// ========== TYPE ASSIGNMENTS ==========

%type <cn> CompoundName
%type <t> Type
%type <t> UnitType
%type <utn> UnitTypeName
%type <t> AnchorType
%type <bl> Block
%type <llbm> BlockMemberSeqOpt
%type <bm> BlockMember
%type <bl> NestedBlock
%type <llbm> NestedBlockMemberSeqOpt
%type <bm> NestedBlockMember
%type <st> Statement
%type <st> IfStatement
%type <llb> ElseIfClauseSeqOpt
%type <bl> ElseIfClause
%type <bl> ElseClauseOpt
%type <st> LoopStatement
%type <rd> RoutineDeclaration
%type <rd> OperatorRoutineDeclaration
%type <s> RoutineName
%type <sa> OperatorRoutineName
%type <s> AliasNameOpt
%type <s> OperatorSign
%type <bl> RoutineBody
%type <ud> UnitDeclaration
%type <llun> UnitDeclarationAdditions  // TODO change actual type
%type <llun> InheritClauseOpt
%type <llun> BaseUnitSeq
%type <un> BaseUnitName
%type <lld> UnitMemberSeqOpt
%type <dc> UnitMember
%type <vd> VariableDeclaration

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
//%token NOT  // TODO remove
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
%token <s> IDENTIFIER
%token <s> LITERAL

// Tokens for lookahead
%token NEW_LINE
%token WHILE_POSTTEST
%token <s> FUNCTION_ID
%token LOOP_ID
%token JUST_BREAK
%token JUST_RETURN
%token JUST_RAISE

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

Module
    : UseDirectiveSeqOpt BlockMemberSeqOpt
    {
        // UseDirectiveSeqOpt is not used for now
        this.parsedProgram = new Module($2);
    }
    ;

// Primitives ***

IdentifierSeq
    :                     IDENTIFIER
    | IdentifierSeq COMMA IDENTIFIER
    ;

CompoundName
    :                  IDENTIFIER
    {
        $$ = new CompoundName($1);
    }
    | CompoundName DOT IDENTIFIER
    {
        $1.AddLast($3);
        $$ = $1;
    }
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
    : UnitType  { $$ = $1; }
    | AnchorType  { $$ = $1; }/*
    | MultiType
    | TupleType
    | RangeType
    | RoutineType*/
    ;  // FIXME a lot of conflicts!

UnitType
    :            UnitTypeName  { $$ = $1; }
    | REF        UnitTypeName  { $$ = $2; }
    | VAL        UnitTypeName  { $$ = $2; }
    | CONCURRENT UnitTypeName  { $$ = $2; }
    ;  // TODO specifiers

UnitTypeName
    : IDENTIFIER
    {
        $$ = new UnitTypeName($1, null);
    }
    | IDENTIFIER GenericArgumentClause
    {
        $$ = new UnitTypeName($1, null);  // TODO generics
    }
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
    {
        $$ = null;  // TODO
    }
    | AS IDENTIFIER
    {
        $$ = null;  // TODO
    }
//  | AS IDENTIFIER RoutineParameters  // TODO review
    ;
/*
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
    : UnitTypeName  // TODO review, probably just Type
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
    : PreconditionOpt DO BlockMemberSeqOpt PostconditionOpt ExceptionHandlerSeqOpt END
    {
        $$ = new Block($3);
    }
    ;

BlockMemberSeqOpt
    : /* empty */
    {
        $$ = new LinkedList<BlockMember>();
    }
    | BlockMemberSeqOpt BlockMember
    {
        $1.AddLast($2);
        $$ = $1;
    }
    ;

BlockMember
    : NestedBlockMember  { $$ = $1; }
    | Block  { $$ = $1; }  // Because `Statement: Block;` was removed
    ;

ExceptionHandlerSeqOpt
    : /* empty */
    | WHEN Expression NestedBlock
    ;  // TODO review

NestedBlock
    : PreconditionOpt    NestedBlockMemberSeqOpt PostconditionOpt ExceptionHandlerSeqOpt
    {
        $$ = new Block($2);
    }
    | PreconditionOpt DO NestedBlockMemberSeqOpt PostconditionOpt ExceptionHandlerSeqOpt
    {
        $$ = new Block($3);
    }
    ;

NestedBlockMemberSeqOpt
    : /* empty */
    {
        $$ = new LinkedList<BlockMember>();
    }
    | NestedBlockMember BlockMemberSeqOpt
    {
        $2.AddFirst($1);
        $$ = $2;
    }
    ;

NestedBlockMember
    : Statement  { $$ = $1; }
    | UnitDeclaration  { $$ = $1; }
    | RoutineDeclaration  { $$ = $1; }
    | VariableDeclaration  { $$ = $1; }
    ;

Statement
    : SEMICOLON  { $$ = null; }
    | NONE  { $$ = null; }
//  | Block  // Conflicts when used in NestedBlock
    | Assignment  { $$ = null; }  // TODO
    | Expression %prec JUST_EXPRESSION
    | IfStatement  { $$ = $1; }
    | LoopStatement  { $$ = $1; }
    | BreakStatement  { $$ = null; }  // TODO
    | ValueLossStatement  { $$ = null; }  // TODO
    | ReturnStatement  { $$ = null; }  // TODO
    | RaiseStatement  { $$ = null; }  // TODO
    ;  // TODO review

Assignment
    : Expression COLON_EQUALS Expression
    ;

IfStatement
    : IF Expression            Block ElseIfClauseSeqOpt ElseClauseOpt END
    {
        $$ = new IfStatement($3, $4, $5);
    }
    | IF Expression THEN NestedBlock ElseIfClauseSeqOpt ElseClauseOpt END
    {
        $$ = new IfStatement($4, $5, $6);
    }
    ;

ElseIfClauseSeqOpt
    : /* empty */
    {
        $$ = new LinkedList<Block>();
    }
    | ElseIfClauseSeqOpt ElseIfClause
    {
        $1.AddLast($2);
        $$ = $1;
    }
    ;

ElseIfClause
    : ELSIF Expression            Block  { $$ = $3; }
    | ELSIF Expression THEN NestedBlock  { $$ = $4; }
    ;

ElseClauseOpt
    : /* empty */  { $$ = null; }
    | ELSE NestedBlock  { $$ = $2; }
    ;

LoopStatement
    : LoopIdOpt                  LOOP NestedBlock END
    {
        $$ = new LoopStatement($3);
    }
    | LoopIdOpt WHILE Expression LOOP NestedBlock END
    {
        $$ = new LoopStatement($5);
    }
    | LoopIdOpt WHILE Expression            Block END
    {
        $$ = new LoopStatement($4);
    }
    | LoopIdOpt LOOP NestedBlock WHILE_POSTTEST Expression END
    {
        $$ = new LoopStatement($3);
    }
    ;

LoopIdOpt
    : /* empty */
    | LOOP_ID COLON
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
    {
        $$ = new VariableDeclaration();
    }
    | VariableSpecifier IdentifierSeq TypeAndInit
    {
        $$ = new VariableDeclaration();
    }
    ;  // TODO content consideration

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
    {
        $$ = new RoutineDeclaration($1, null, $6);
    }
    | RoutineSpecifier RoutineName GenericFormalsOpt RoutineParameters ReturnTypeOpt UseDirectiveSeqOpt RoutineBody
    {
        $$ = new RoutineDeclaration($2, null, $7);
    }
    ;  // TODO change UseSeq to just Use

OperatorRoutineDeclaration
    :                  OperatorRoutineName GenericFormalsOpt RoutineParameters ReturnTypeOpt UseDirectiveSeqOpt RoutineBody
    {
        $$ = new RoutineDeclaration($1[0], $1[1], $6);
    }
    | RoutineSpecifier OperatorRoutineName GenericFormalsOpt RoutineParameters ReturnTypeOpt UseDirectiveSeqOpt RoutineBody
    {
        $$ = new RoutineDeclaration($2[0], $2[1], $7);
    }
    ;  // TODO change UseSeq to just Use; review

RoutineSpecifier
    : PURE
    | SAFE
    | ABSTRACT
    | OVERRIDE
    ;  // TODO review

RoutineName
    : FUNCTION_ID  { $$ = $1; }
//  | OperatorSign AliasNameOpt
//  | COLON_EQUALS
    | LPAREN RPAREN  { $$ = "()"; }  // TODO review
    ;

OperatorRoutineName
    : OperatorSign AliasNameOpt  { $$ = new string[] {$1, $2}; }
    | COLON_EQUALS  { $$ = new string[] {":=", null}; }
    ;

AliasNameOpt
    : /* empty */  { $$ = null; }
    | ALIAS FUNCTION_ID  { $$ = $2; }
    ;

OperatorSign
    : PLUS  { $$ = "+"; }
    | MINUS  { $$ = "-"; }
    | AMPERSAND  { $$ = "&"; }
    | VERTICAL  { $$ = "|"; }
    | CARET  { $$ = "^"; }
    | TILDE  { $$ = "~"; }
    ;  // TODO

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
    {
        $$ = $1;
    }
    | DOUBLE_ARROW Statement  // TODO review
    {
        LinkedList<BlockMember> list = new LinkedList<BlockMember>();
        list.AddFirst($2);
        $$ = new Block(list);
    }
    | IS ABSTRACT
    {
        $$ = null;
    }
    | IS FOREIGN
    {
        $$ = null;
    }
    ;

// Unit ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

UnitDeclaration
    : UnitSpecifiersOpt UNIT CompoundName UnitDeclarationAdditions    UnitMemberSeqOpt InvariantOpt END
    {
        $$ = new UnitDeclaration($3, $4, $5);
    }
    | UnitSpecifiersOpt UNIT CompoundName UnitDeclarationAdditions IS UnitMemberSeqOpt InvariantOpt END
    {
        $$ = new UnitDeclaration($3, $4, $6);
    }
    ;  // TODO specifiers and invariant consideration

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
    : UnitAliasNameOpt GenericFormalsOpt InheritClauseOpt UseClauseOpt
    {
        $$ = $3;  // TODO other declaration additions
    }
    ;

UnitAliasNameOpt
    : /* empty */
    | ALIAS IDENTIFIER
    ;

InheritClauseOpt
    : /* empty */  { $$ = null; }
    | EXTEND BaseUnitSeq  { $$ = $2; }
    ;

BaseUnitSeq
    :                   BaseUnitName
    {
        LinkedList<UnitName> list = new LinkedList<UnitName>();
        list.AddFirst($1);
        $$ = list;
    }
    | BaseUnitSeq COMMA BaseUnitName
    {
        $1.AddLast($3);
        $$ = $1;
    }
    ;

BaseUnitName
    :       Type
    {
        if ($1 == null)  // TODO remove nulls
        {
            $$ = null;
        }
        else
        {
            $$ = new UnitName($1, true);
        }
    }
    | TILDE Type
    {
        if ($2 == null)  // TODO remove nulls
        {
            $$ = null;
        }
        else
        {
            $$ = new UnitName($2, true);
        }
    }
    ;

UnitMemberSeqOpt
    : /* empty */
    {
        $$ = new LinkedList<Declaration>();
    }
    | UnitMemberSeqOpt                     UnitMember
    {
        $1.AddLast($2);
        $$ = $1;
    }
    | UnitMemberSeqOpt UnitMemberSpecifier UnitMember
    {
        $1.AddLast($3);  // TODO specifiers
        $$ = $1;
    }
    ;

UnitMemberSpecifier
    : HIDDEN %prec JUST_HIDDEN
    | HIDDEN FINAL
    ;

UnitMember
    : SEMICOLON  { $$ = null; }  // All other statements are restricted
    | UnitDeclaration  { $$ = $1; }
    | RoutineDeclaration  { $$ = $1; }
    | VariableDeclaration  { $$ = $1; }
    | ConstObjectDeclaration  { $$ = null; }  // TODO constant objects
    | InitializerDeclaration  { $$ = null; }  // TODO
    | OperatorRoutineDeclaration  { $$ = $1; }
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

private Parser(Scanner scnr) : base(scnr) { parsedProgram = null; }

private Module parsedProgram;

internal static Module parseProgram(String filePath)
{
    FileStream file = new FileStream(filePath, FileMode.Open);
    Scanner scanner = new Scanner(file);
    Parser parser = new Parser(scanner);
    bool res = parser.Parse();
    file.Close();
    return res ? parser.parsedProgram : null;
}
