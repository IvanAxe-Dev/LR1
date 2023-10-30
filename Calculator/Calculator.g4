grammar Calculator;

// Parser rules

compileUnit : expression EOF;

expression :
	operatorToken=(MOD | DIV) LPAREN expression ',' expression RPAREN #ParenthesizedExpr
	| operatorToken=(INC | DEC) LPAREN expression RPAREN #ParenthesizedExprOneArg
	| expression EXPONENT expression #ExponentialExpr
    | expression operatorToken=(MULTIPLY | DIVIDE) expression #MultiplicativeExpr
	| expression operatorToken=(ADD | SUBTRACT) expression #AdditiveExpr
	| NUMBER #NumberExpr
	| IDENTIFIER #IdentifierExpr
	; 

/*
 * Lexer Rules
 */

NUMBER : INT ('.' INT)?; 
IDENTIFIER : [a-zA-Z]+[1-9][0-9]*;

INT : ('0'..'9')+;

EXPONENT : '^';
MULTIPLY : '*';
DIVIDE : '/';
SUBTRACT : '-';
ADD : '+';
LPAREN : '(';
RPAREN : ')';
MOD : 'mod';
DIV : 'div';
INC : 'inc';
DEC : 'dec';

WS : [ \t\r\n] -> channel(HIDDEN);
