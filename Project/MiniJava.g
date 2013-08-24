grammar MiniJava;

options {
	language = CSharp3;

	output = AST;
	ASTLabelType = CommonTree;
}


tokens {
	ELIST;
	SLIST;
	METHOD_CALL;
	UNARY_MINUS;
	PROGRAM;
	MAINCLASSDECL;
	CLASSVARDECL;
	CLASSVARDECLLIST;
	CLASSDECL;
	METHODDECL;
	METHODDECLLIST;
	FORMALLIST;
	VARDECL;
	TYPE;
	STMT;
	BOOL;
	
	HIDDEN;
}


@parser::namespace { MiniJavaSyntax }
@lexer::namespace  { MiniJavaSyntax }

@header { 
	using System;
	using GrammarElements;
}

@members  {

	public ProgramStatement result;
	private bool flagDebug = false;
	
	public const int NONE = -1;
	
	private List<RecognitionException> exceptions = new List<RecognitionException>();

	public override void ReportError(RecognitionException e)
	{
	    exceptions.Add(e);
	}
	
	public bool HasError
	{
		get { return exceptions.Count > 0; }
	}
	
	public string ErrorMessage
	{
		get { return this.GetErrorMessage(exceptions[0], this.TokenNames); }
	}
	
	public string ErrorPosition
	{
		get { return this.GetErrorHeader(exceptions[0]); }
	}

}


public
program returns [NonTerm value]
	:
		mainClass=mainClassDecl classes=classDeclList EOF!
		{ 
			List<BaseSymbol> symbols = new List<BaseSymbol>();
			symbols.Add(mainClass.value);
			symbols.AddRange(classes.valueList);
			$value = NonTermFactory.CreateProgramStatement(mainClass.value, classes.valueList);
			if (flagDebug) Console.WriteLine("program");
		}
	;
	
classDeclList returns [ List<BaseSymbol> valueList ]
	@init
	{
		$valueList = new List<BaseSymbol>();
	}
	:	
		( classD=classDecl 
		{
			$valueList.Add(classD.value);
		}
		 )*
		{ 
		}	
	;

mainClassDecl returns [NonTerm value]
	:
		CLASS^ id=ID LCURLY!
			PUBLIC! STATIC! VOID! MAIN! LPAREN!
				STRING! LBRACK! RBRACK! ID
			RPAREN!
			LCURLY! statement=stmtList RCURLY!
		RCURLY!
		{ 
			$value = NonTermFactory.CreateMainClassDecl(new Token(TokenType.ID, id.Text, id), statement.valueList);
			if (flagDebug) Console.WriteLine("mainClassDecl");
		}
	;

stmtList returns [ List<BaseSymbol> valueList ]
	@init
	{
		$valueList = new List<BaseSymbol>();
	}
	:
		( statement = stmt
		{
			$valueList.Add(statement.value);
		}
		 )*
		{ 
		}	
	;

classVarDeclList returns [ List<BaseSymbol> valueList ]
	@init
	{
		$valueList = new List<BaseSymbol>();
	}
	:
		( classVar = classVarDecl 
		{ 
			$valueList.Add(classVar.value); 
		}
		 )*
	;

classVarDecl returns [NonTerm value]
	:
		typeDecl = type  id = ID  SEMI!
		{
			$value = NonTermFactory.CreateClassVarDecl(typeDecl.value, new Token(TokenType.ID, id.Text, id));
		}
	;

extendsClause returns [NonTerm value]
	:
		( EXTENDS! id = ID 
		{
			$value = NonTermFactory.CreateExtendsClause(new Token(TokenType.ID, id.Text, id));
		}
		| 
		{ 
		}
		 )
	;

classDecl returns [ NonTerm value ]
	:
		CLASS^  id = ID  extends = extendsClause LCURLY!
			classVars = classVarDeclList
			methods = methodDeclList
		RCURLY!
		{ 
			List<BaseSymbol> symbols = new List<BaseSymbol>() { };
			$value = NonTermFactory.CreateClassDecl(new Token(TokenType.ID, id.Text, id), extends.value, classVars.valueList, methods.valueList);
		}
	;


methodDecl returns [ NonTerm value ]
	:
		PUBLIC^ typeDecl = type id = ID LPAREN! formals = formalList RPAREN! LCURLY!
			statements = stmtList
			RETURN! expression = expr SEMI!
		RCURLY!
		{ 
			List<BaseSymbol> symbols = new List<BaseSymbol>() ;
			$value = NonTermFactory.CreateMethodDecl(typeDecl.value, new Token(TokenType.ID, id.Text, id), formals.valueList, statements.valueList, expression.value);
		}
	;
methodDeclList returns [ List<BaseSymbol> valueList ]
	@init
	{
		$valueList = new List<BaseSymbol>();
	}
	:
		( method = methodDecl 
		{
			$valueList.Add(method.value);
		}
		 )*
		{ 
		}
	;


formalList returns [ List<BaseSymbol> valueList ]
	@init
	{
		$valueList = new List<BaseSymbol>();
	}
	:
		( formalDecl = formal
		{
			$valueList.Add(formalDecl.value);
		}
		( COMMA! formalDecl = formal
		{
			$valueList.Add(formalDecl.value);
		}
		  )* )?
		{ 
		}
	;

formal returns [ NonTerm value ]
	:
		variable = varDecl
		{
			$value = variable.value;
		}
	;
	
	
primitiveType returns [ BaseSymbol value ]
	:
		token = ID
		{
			$value = new Token(TokenType.ID, token.Text, token);
		}
	| 
		BOOLEAN
		{
			$value = new Token(TokenType.BOOLEAN, token);
		}
	| 
		INT
		{
			$value = new Token(TokenType.INT, token);
		}
	|
		DOUBLE
		{
			$value = new Token(TokenType.FLOAT, token);
		}
	;
	
		
type returns [ BaseSymbol value ]
	@init
	{
		int indexArrayBrackets = 0;
	}
	:
		( typeDecl = primitiveType
		{
		
		}
		( LBRACK RBRACK
		{
			indexArrayBrackets++;
		}
		  )*
		 )
		{
			if (indexArrayBrackets == 0)
			{
				$value = typeDecl.value;
			}
			else
			{
				$value = NonTermFactory.CreateArrayDecl(typeDecl.value, indexArrayBrackets);
			}
		}
	;




constant returns [ BaseSymbol value ]
	:
		constantDecl = FLOAT
		{
			$value = new Token(TokenType.FLOAT, constantDecl.Text, constantDecl);
		}
	|
		constantDecl = INTEGER
		{
			$value = new Token(TokenType.INTEGER, constantDecl.Text, constantDecl);
		}
	|
		constantDecl = NULL
		{
			$value = new Token(TokenType.NULL, constantDecl.Text, constantDecl);
		}
	| 
		constantDecl = TRUE 
		{
			$value = new Token(TokenType.BOOL, constantDecl.Text, constantDecl);
		}
	| 
		constantDecl = FALSE 
		{
			$value = new Token(TokenType.BOOL, constantDecl.Text, constantDecl);
		}
	;



argList returns [ List<BaseSymbol> valueList ]
	:
		( expressionListDecl = expressionList 
		{
			$valueList = expressionListDecl.valueList;
		}
		|
		{ 
			$valueList = new List<BaseSymbol>();
		}
		 )
	;


stmt returns [ NonTerm value ]
	@init
	{
		List<BaseSymbol> bracketExpressionList = new List<BaseSymbol>();
	}
	:
		LCURLY! statementList = stmtList RCURLY!
		{
			$value = NonTermFactory.CreateStatementList(statementList.valueList);
		}
	| 
		IF^ LPAREN! expression = expr RPAREN! statementIf = stmt (ELSE^ statementElse = stmt)?
		{
			if (statementElse == null)
			{
				$value = NonTermFactory.CreateIfStatement(expression.value, statementIf.value);
			}
			else
			{
				$value = NonTermFactory.CreateIfElseStatement(expression.value, statementIf.value, statementElse.value);
			}
		}
	| 
		WHILE^ LPAREN! expression = expr RPAREN! statement = stmt
		{
			$value = NonTermFactory.CreateWhileStatement(expression.value, statement.value);
		}

	| 
		(PRINT^ | PRINT2^) LPAREN! expression = expr RPAREN! SEMI!
		{
			$value = NonTermFactory.CreatePrintStatement(expression.value);
		}
	| 
		(variable = varDecl) (ASSIGN^ expression = expr)? SEMI!
		{
			
			$value = NonTermFactory.CreateAssignVarStatementt(variable.value, expression != null ? expression.value : null);
		}
	| 
		(id = ID) 
		(
			LBRACK! expressionBracket = expr RBRACK!
			{
				bracketExpressionList.Add(expressionBracket.value);
			}
		 )* (ASSIGN^ expression = expr)? SEMI!
		{
			if (bracketExpressionList.Count == 0)
			{
				$value = NonTermFactory.CreateAssignIdStatement(new Token(TokenType.ID, id.Text, id), expression != null ? expression.value : null);
			}
			else
			{
				$value = NonTermFactory.CreateArrayAssignIdStatement(new Token(TokenType.ID, id.Text, id), expression != null ? expression.value : null, bracketExpressionList);
			}
		}
	;

varDecl returns [ NonTerm value ]
	:
		typeDecl = type  id = ID
		{
			$value = NonTermFactory.CreateVarDecl(typeDecl.value, new Token(TokenType.ID, id.Text, id));
		}
	;

// thisLevel: nextHigher (OP nextHigher)*
// Precedence (lowest-to-highest):
// (7)  ||
// (6)  &&
// (5)  == !=
// (4)  < <= >= >
// (3)  +(binary) -(binary)
// (2)  * /
// (1)  -(unary) !
//      () (method call) . (dot -- identifier qualification) new 
//      new  () (explicit parenthesis)

expr returns [ BaseSymbol value ]
	:
		expression  = logicalOrExpression
		{
			$value = expression.value;
		}
	;

expressionList returns [ List<BaseSymbol> valueList ]
	@init
	{
		$valueList = new List<BaseSymbol>();
	}
	:
		expression = expr 
		{
			$valueList.Add(expression.value);
		}
		( COMMA!  expression = expr
		{ 
			$valueList.Add(expression.value);
		}
		 )*
		
	;

// logical or (||) (level 7)
logicalOrExpression returns [ BaseSymbol value ]
	@init
	{
		List<BaseSymbol> valueList = new List<BaseSymbol>();
	}
	:
		expression = logicalAndExpression
		{
			valueList.Add(expression.value);
		}
		( LOR^ expression = logicalAndExpression
		{
			valueList.Add(expression.value);
		}
		 )*
		{
			$value = NonTermFactory.CreateLogicalOrExpression(valueList);
		}
	;

// logical and (&&) (level 6)
logicalAndExpression returns [ BaseSymbol value ]
	@init
	{
		List<BaseSymbol> valueList = new List<BaseSymbol>();
	}
	:
		expression = equalityExpression 
		{
			valueList.Add(expression.value);
		}
		( LAND^ expression = equalityExpression
		{
			valueList.Add(expression.value);
		}
		 )*
		{
			$value = NonTermFactory.CreateLogicalAndExpression(valueList);
		}
	;

// equality/inequality (==/!=) (level 5)
// nonassociative
equalityExpression returns [ BaseSymbol value ]
	@init
	{
		List<BaseSymbol> valueList = new List<BaseSymbol>();
	}
	:
		expression = relationalExpression 
		{
			valueList.Add(expression.value);
		}
		( (token = EQUAL^ | token = NEQUAL^) expression = relationalExpression
		{
			valueList.Add(expression.value);
		}
		 )?
		{
			$value = NonTermFactory.CreateEqualityExpression(token == null ? NONE : token.Type, valueList);
		}
	;

// boolean relational expressions (level 4)
relationalExpression returns [ BaseSymbol value ]
	@init
	{
		List<BaseSymbol> valueList = new List<BaseSymbol>();
	}
	:
		expression = additiveExpression 
		{
			$value = expression.value;
		}
		((token = LESS^ | token = LESSEQ^ | token = MOREEQ^ | token = MORE^) expression = additiveExpression
		{
			$value = NonTermFactory.CreateExpression(token == null ? NONE : token.Type, $value, expression.value);
		}
		 )*
		{
		}
	;

// binary addition/subtraction (level 3)
additiveExpression returns [ BaseSymbol value ]
	@init
	{
		List<BaseSymbol> valueList = new List<BaseSymbol>();
	}
	:
		expression = multiplicativeExpression
		{
			$value = expression.value;
		} 
		((token = PLUS^ | token = MINUS^) expression = multiplicativeExpression
		{
			$value = NonTermFactory.CreateExpression(token == null ? NONE : token.Type, $value, expression.value);
		}
		 )*
		
	;

// multiplication/division (level 2)
multiplicativeExpression returns [ BaseSymbol value ]
	@init
	{
		List<BaseSymbol> valueList = new List<BaseSymbol>();
	}
	:
		expression = decoratedExpression
		{
			$value = expression.value;
		} 
		((token = STAR^ | token = DIV^) expression = decoratedExpression
		{
			$value = NonTermFactory.CreateExpression(token == null ? NONE : token.Type, $value, expression.value);
		}
		 )*
		
	;

decoratedExpression returns [ BaseSymbol value ]
	@init
	{
		BaseSymbol methodArguments = null;
	}	
	:
		MINUS^ decoratedExpressionDecl = decoratedExpression
		{
			$value = NonTermFactory.CreateUnaryMinusExpression(decoratedExpressionDecl.value);
		}
	| 
		LNOT^ decoratedExpressionDecl = decoratedExpression
		{
			$value = NonTermFactory.CreateUnaryNotExpression(decoratedExpressionDecl.value);
		}
	| 
		expression = primaryExpression 
			
			(( DOT^  id = ID 
				( LPAREN^ argListDecl = argList RPAREN! 
				{
					methodArguments = NonTermFactory.CreateArgumentListExpression(argListDecl.valueList);
				}
				 )?
			 
			{
				
			}
			)?
		
		{
			if (argListDecl == null)
			{
				if (id == null)
				{
					$value = expression.value;
				}
				else if (!id.Text.Equals(TokenNames[LENGTH].ToLower()))
				{
					$value = NonTermFactory.CreateFieldExpression(expression.value, new Token(TokenType.ID, id.Text, id));
				}
				else
				{
					$value = NonTermFactory.CreateLengthFunctionExpression(expression.value);
				}
			}
			else
			{
				$value = NonTermFactory.CreateMethodCallListExpression(expression.value, new Token(TokenType.ID, id.Text, id), methodArguments);
			}
		}
		|

			( LPAREN^ methodArgListDecl = argList RPAREN! )
			{
				{
					$value = NonTermFactory.CreateMethodCallListExpression(expression.value, 
						NonTermFactory.CreateArgumentListExpression(methodArgListDecl.valueList));
				}
				
			}
		)
	;

primaryExpression returns [ BaseSymbol value ]
	@init
	{
		List<BaseSymbol> bracketExpressionList = new List<BaseSymbol>();
		List<BaseSymbol> bracketAssignmentExpressionList = new List<BaseSymbol>();
	}
	:
		token = ID 
		( LBRACK! bracketExpression = logicalOrExpression RBRACK!
		{
			bracketAssignmentExpressionList.Add(bracketExpression.value);
		}
		 )*
		{	
			if (bracketAssignmentExpressionList.Count == 0)
			{
				$value = new Token(TokenType.ID, token.Text, token);
			}
			else
			{
				$value = NonTermFactory.CreateArrayIndiciesStatement(new Token(TokenType.ID, token.Text, token), bracketAssignmentExpressionList);
			}
			
		}
	| 
		constantDecl = constant
		{
			$value = constantDecl.value;
		}
	| 
		NEW^ typeDecl = primitiveType LPAREN! RPAREN!
		{
			$value = NonTermFactory.CreateNewStatement(typeDecl.value);
		}
	| 
		NEW^ typeDecl = primitiveType 
		( LBRACK! expressionDecl = expr RBRACK!
			{
				bracketExpressionList.Add(expressionDecl.value);
			}
		 )+
		{
			$value = NonTermFactory.CreateNewArrayStatement(typeDecl.value, bracketExpressionList);
		}
	| 
		LPAREN! expression = logicalOrExpression RPAREN!
		{
			$value = expression.value;
		}
	;

// ----------------------------------------------------------------
// The MiniJava Lexer
// ----------------------------------------------------------------

//tokens

CLASS : 'class';
PUBLIC : 'public';
STATIC : 'static';
EXTENDS : 'extends';
VOID : 'void';
INT : 'int';
DOUBLE : 'double';
BOOLEAN : 'boolean';
IF : 'if';
ELSE : 'else';
WHILE : 'while';
RETURN : 'return';
NULL : 'null';
TRUE : 'true';
FALSE : 'false';
fragment
THIS : 'this';
NEW : 'new';
STRING : 'String';
MAIN : 'main';
PRINT : 'printf';
PRINT2 : 'System.out.println';

fragment
LENGTH : 'length';

// Operators
PLUS    : '+';
MINUS   : '-';
STAR    : '*';
DIV     : '/';
LESS    : '<';
LESSEQ  : '<=';
MOREEQ  : '>=';
MORE    : '>';
EQUAL   : '==';
NEQUAL  : '!=';
LAND    : '&&';
LOR     : '||';
LNOT    : '!';

// Delimiters
SEMI    : ';';
DOT     : '.';
COMMA   : ',';
ASSIGN  : '=';
LPAREN  : '(';
RPAREN  : ')';
LCURLY  : '{';
RCURLY  : '}';
LBRACK  : '[';
RBRACK  : ']';


// Helper tokens
fragment
LETTER:
	( 'a'..'z' | 'A'..'Z' | '_' )
	;
	
fragment
DIGIT:
	( '0'..'9' )
	;

ID 
	:
		('System.out.println' ~('a'..'z' | 'A'..'Z' | DIGIT) ) => 'System.out.println' 
	| 
		(  (LETTER) (LETTER  | DIGIT)* )
	;
	

INTEGER
	:
		( DIGIT )+
	;


FLOAT
    :   
		(DIGIT)+ '.' (DIGIT)* EXPONENT?
    |   
		'.' (DIGIT)+ EXPONENT?
    |   
		(DIGIT)+ EXPONENT
    ;


fragment
EXPONENT : ('e'|'E') ('+'|'-')? (DIGIT)+ ;


WHITESPACE
	:
	   ( ' '
        | '\t'
        | '\r'
        | '\n'
        ) {$channel=HIDDEN;}
    ;

LINE_COMMENT
	:
		'//' ~('\n'|'\r')* '\r'? '\n' {$channel=HIDDEN;}
	;
	
BLOCK_COMMENT
	:
		'/*' ( options {greedy=false;} : . )* '*/' {$channel=HIDDEN;}
	;
