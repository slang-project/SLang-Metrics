/**
 * SLangScanner.lex file, Version 0.1.0
 *
 * Expected file format is Unicode. In the event that no
 * byte order mark prefix is found, revert to raw bytes.
 */
%option classes, codepage:raw, stack, unicode, out:src\SLangScanner.cs

%using SLangParser;

%namespace SLangScanner
%visibility internal

%x COMMENT

WS	    [\u0020\u0009\u000D\u000A]

ND      [[:IsLetter:]_]
D       [[:IsDigit:]]
ID      {ND}({ND}|{D}|$)*

DEC     {D}({D}|_)*
INT     (0[ob])?{DEC}|0x({D}|[A-Fa-f])({D}|[A-Fa-f_])*

EXP     [Ee][+\-]?{DEC}
REAL    {DEC}({EXP}|\.{DEC}({EXP})?)

CHAR    '(\\.|[^\\'])+'
STR     \"(\\.|[^\\"])*\"

%%

{WS}+ {
    if (yytext.Contains("\u000D") || yytext.Contains("\u000A"))
    {
        ++ssMetrics.LOC;
        return (int)Tokens.NEW_LINE;
    }
    /* else ignore whitespaces */
}

// ============== COMMENTS ==============

"//"[^\n\r]*$      { ++ssMetrics.commLines; }

"/*"               { yy_push_state(COMMENT); }
<COMMENT> {
  "/*"             {
    yy_push_state(COMMENT);
    commentBegin = yylloc.StartLine;
  }
  "*/"             {
    yy_pop_state();
    ssMetrics.commLines += yylloc.EndLine - commentBegin + 1;
  }
  (.|\n)           { /* ignore block comment content */ }
}

// ============= SEPARATORS =============

","                { ssMetrics.addOperator(yytext); return (int)Tokens.COMMA; }
"."                { ssMetrics.addOperator(yytext); return (int)Tokens.DOT; }
";"                { ssMetrics.addOperator(yytext); return (int)Tokens.SEMICOLON; }
":"                { ssMetrics.addOperator(yytext); return (int)Tokens.COLON; }
"("                { ssMetrics.addOperator(yytext); return (int)Tokens.LPAREN; }
")"                { ssMetrics.addOperator(yytext); return (int)Tokens.RPAREN; }
"["                { ssMetrics.addOperator(yytext); return (int)Tokens.LBRACKET; }
"]"                { ssMetrics.addOperator(yytext); return (int)Tokens.RBRACKET; }
".."               { ssMetrics.addOperator(yytext); return (int)Tokens.GENERATOR; }
"->"               { ssMetrics.addOperator(yytext); return (int)Tokens.SINGLE_ARROW; }
"=>"               { ssMetrics.addOperator(yytext); return (int)Tokens.DOUBLE_ARROW; }
"?"                { ssMetrics.addOperator(yytext); return (int)Tokens.QUESTION; }

// ============= OPERATORS ==============

"+" {
    yylval.s = yytext;
    ssMetrics.addOperator(yytext);
    return (int)Tokens.PLUS;
}
"-" {
    yylval.s = yytext;
    ssMetrics.addOperator(yytext);
    return (int)Tokens.MINUS;
}
"*" {
    yylval.s = yytext;
    ssMetrics.addOperator(yytext);
    return (int)Tokens.ASTERISK;
}
"/" {
    yylval.s = yytext;
    ssMetrics.addOperator(yytext);
    return (int)Tokens.SLASH;
}
"**" {
    yylval.s = yytext;
    ssMetrics.addOperator(yytext);
    return (int)Tokens.DBL_ASTERISK;
}
"|" {
    yylval.s = yytext;
    ssMetrics.addOperator(yytext);
    return (int)Tokens.VERTICAL;
}
"||" {
    yylval.s = yytext;
    ssMetrics.addOperator(yytext);
    return (int)Tokens.DBL_VERTICAL;
}
"&" {
    yylval.s = yytext;
    ssMetrics.addOperator(yytext);
    return (int)Tokens.AMPERSAND;
}
"&&" {
    yylval.s = yytext;
    ssMetrics.addOperator(yytext);
    return (int)Tokens.DBL_AMPERSAND;
}
"^" {
    yylval.s = yytext;
    ssMetrics.addOperator(yytext);
    return (int)Tokens.CARET;
}
"~" {
    yylval.s = yytext;
    ssMetrics.addOperator(yytext);
    return (int)Tokens.TILDE;
}
"<" {
    yylval.s = yytext;
    ssMetrics.addOperator(yytext);
    return (int)Tokens.LESS;
}
"<=" {
    yylval.s = yytext;
    ssMetrics.addOperator(yytext);
    return (int)Tokens.LESS_EQUALS;
}
">" {
    yylval.s = yytext;
    ssMetrics.addOperator(yytext);
    return (int)Tokens.GREATER;
}
">=" {
    yylval.s = yytext;
    ssMetrics.addOperator(yytext);
    return (int)Tokens.GREATER_EQUALS;
}
"=" {
    yylval.s = yytext;
    ssMetrics.addOperator(yytext);
    return (int)Tokens.EQUALS;
}
"/=" {
    yylval.s = yytext;
    ssMetrics.addOperator(yytext);
    return (int)Tokens.SLASH_EQUALS;
}
"<>" {
    yylval.s = yytext;
    ssMetrics.addOperator(yytext);
    return (int)Tokens.LESS_GREATER;
}
":=" {
    yylval.s = yytext;
    ssMetrics.addOperator(yytext);
    return (int)Tokens.COLON_EQUALS;
}

// ============== KEYWORDS ==============

"and"{WS}+"then"   { ssMetrics.addOperator(yytext); return (int)Tokens.AND_THEN; }
"or"{WS}+"else"    { ssMetrics.addOperator(yytext); return (int)Tokens.OR_ELSE; }

"abstract"         { ssMetrics.addOperator(yytext); return (int)Tokens.ABSTRACT; }
"alias"            { ssMetrics.addOperator(yytext); return (int)Tokens.ALIAS; }
"as"               { ssMetrics.addOperator(yytext); return (int)Tokens.AS; }
"break"            { ssMetrics.addOperator(yytext); return (int)Tokens.BREAK; }
"check"            { ssMetrics.addOperator(yytext); return (int)Tokens.CHECK; }
"concurrent"       { ssMetrics.addOperator(yytext); return (int)Tokens.CONCURRENT; }
"const"            { ssMetrics.addOperator(yytext); return (int)Tokens.CONST; }
"deep"             { ssMetrics.addOperator(yytext); return (int)Tokens.DEEP; }
"do"               { ssMetrics.addOperator(yytext); return (int)Tokens.DO; }
"else"             { ssMetrics.addOperator(yytext); return (int)Tokens.ELSE; }
"elsif"            { ssMetrics.addOperator(yytext); return (int)Tokens.ELSIF; }
"end"              { ssMetrics.addOperator(yytext); return (int)Tokens.END; }
"ensure"           { ssMetrics.addOperator(yytext); return (int)Tokens.ENSURE; }
"extend"           { ssMetrics.addOperator(yytext); return (int)Tokens.EXTEND; }
"external"         { ssMetrics.addOperator(yytext); return (int)Tokens.EXTERNAL; }
"final"            { ssMetrics.addOperator(yytext); return (int)Tokens.FINAL; }
"foreign"          { ssMetrics.addOperator(yytext); return (int)Tokens.FOREIGN; }
"hidden"           { ssMetrics.addOperator(yytext); return (int)Tokens.HIDDEN; }
"if"               { ssMetrics.addOperator(yytext); return (int)Tokens.IF; }
"in"               { ssMetrics.addOperator(yytext); return (int)Tokens.IN; }
"init"             { ssMetrics.addOperator(yytext); return (int)Tokens.INIT; }
"invariant"        { ssMetrics.addOperator(yytext); return (int)Tokens.INVARIANT; }
"is"               { ssMetrics.addOperator(yytext); return (int)Tokens.IS; }
"loop"             { ssMetrics.addOperator(yytext); return (int)Tokens.LOOP; }
"new"              { ssMetrics.addOperator(yytext); return (int)Tokens.NEW; }
"none"             { ssMetrics.addOperator(yytext); return (int)Tokens.NONE; }
"old"              { ssMetrics.addOperator(yytext); return (int)Tokens.OLD; }
"override"         { ssMetrics.addOperator(yytext); return (int)Tokens.OVERRIDE; }
"pure"             { ssMetrics.addOperator(yytext); return (int)Tokens.PURE; }
"raise"            { ssMetrics.addOperator(yytext); return (int)Tokens.RAISE; }
"ref"              { ssMetrics.addOperator(yytext); return (int)Tokens.REF; }
"require"          { ssMetrics.addOperator(yytext); return (int)Tokens.REQUIRE; }
"return"           { ssMetrics.addOperator(yytext); return (int)Tokens.RETURN; }
"routine"          { ssMetrics.addOperator(yytext); return (int)Tokens.ROUTINE; }
"safe"             { ssMetrics.addOperator(yytext); return (int)Tokens.SAFE; }
"super"            { ssMetrics.addOperator(yytext); return (int)Tokens.SUPER; }
"then"             { ssMetrics.addOperator(yytext); return (int)Tokens.THEN; }
"this"             { ssMetrics.addOperator(yytext); return (int)Tokens.THIS; }
"unit"             { ssMetrics.addOperator(yytext); return (int)Tokens.UNIT; }
"use"              { ssMetrics.addOperator(yytext); return (int)Tokens.USE; }
"val"              { ssMetrics.addOperator(yytext); return (int)Tokens.VAL; }
"when"             { ssMetrics.addOperator(yytext); return (int)Tokens.WHEN; }
"while"            { ssMetrics.addOperator(yytext); return (int)Tokens.WHILE; }

// ============= IDENTIFIER =============

{ID}               {
    yylval.s = yytext;
    ssMetrics.addOperand(yytext);  // TODO: review
    return (int)Tokens.IDENTIFIER;
}

// ============== LITERALS ==============

{INT}              |
{REAL}             |
{CHAR}             |
{STR}              {
    yylval.s = yytext;
    ssMetrics.addOperand(yytext);  // TODO: review
    return (int)Tokens.LITERAL;
}

// =========== LEXICAL ERRORS ===========

<COMMENT><<EOF>>   |
.                  { return (int)Tokens.error; }

// =========== END OF PARSING ===========

<<EOF>>            {
//    ssMetrics.LOC = yylloc.EndLine;  // FIXME: zero value for EOF
    ssMetrics.recalculate();
    return (int)Tokens.EOF;
}

%%

private int commentBegin;
internal HalsteadMetrics ssMetrics { get; } = new HalsteadMetrics();

/// Software Sciences metrics
public class HalsteadMetrics
{
    HashSet<string> operators;
    HashSet<string> operands;
    private int totalOperators;
    private int totalOperands;

    public int    LOC = 0;
    public int    commLines = 0;
    public int    vocabulary;
    public int    length;
    public double volume;
    public double difficulty;
    public double effort;
    public double timeSeconds;
    public double numberOfBugs;

    internal HalsteadMetrics()
    {
        // initialize containers

        operators = new HashSet<string>();
        operands  = new HashSet<string>();

        // set default values for metrics
        vocabulary     = 0;
        volume         = 0;
        difficulty     = 0;
        effort         = 0;
        timeSeconds    = 0;
        numberOfBugs   = 0;
        totalOperators = 0;
        totalOperands  = 0;
    }

    public void addOperator(string opr)
    {
        if (!operators.Contains(opr))
        {
            operators.Add(opr);
        }
        ++totalOperators;
    }

    public void addOperand(string opd)
    {
        if (!operands.Contains(opd))
        {
            operands.Add(opd);
        }
        ++totalOperands;
    }

    public void recalculate()
    {
        int N1  = totalOperators;
        int N2  = totalOperands;
        int nu1 = operators.Count;
        int nu2 = operands.Count;
        int N   = N1 + N2;
        int nu  = nu1 + nu2;

        vocabulary = nu;
        length = N;

        volume = N * Math.Log(nu, 2);
        difficulty = ((double)nu1 / 2.0) * ((double)N2 / (double)nu2);
        effort = difficulty * volume;
        timeSeconds = effort / 18;
        numberOfBugs = volume / 3000;
    }
}
