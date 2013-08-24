using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompilerProject.GrammarElements
{
    public enum TokenType
    {
        None,

        /*Identifier,
        Integer,
        Number,
        String,*/

        // Define literal tokens for all of the reserved words.
        CLASS, 	//  'class';
        PUBLIC, 	//  'public';
        STATIC, 	//  'static';
        EXTENDS, 	//  'extends';
        VOID, 	//  'void';
        INT, 	//  'int';
        BOOLEAN, 	//  'boolean';
        IF, 	//  'if';
        ELSE, 	//  'else';
        WHILE, 	//  'while';
        RETURN, 	//  'return';
        NULL, 	//  'null';
        TRUE, 	//  'true';
        FALSE, 	//  'false';
        THIS, 	//  'this';
        NEW, 	//  'new';
        STRING, 	//  'String';
        MAIN, 	//  'main';
        PRINT, 	//  'printf';

        // Operators
        PLUS, 	//  '+';
        MINUS, 	//  '-';
        STAR, 	//  '*';
        DIV, 	//  '/';
        LESS, 	//  '<';
        LESSEQ, 	//  '<=';
        MOREEQ, 	//  '>=';
        MORE, 	//  '>';
        EQUAL, 	//  '==';
        NEQUAL, 	//  '!=';
        LAND, 	//  '&&';
        LOR, 	//  '||';
        LNOT, 	//  '!';

        // Delimiters
        SEMI, 	//  ';';
        DOT, 	//  '.';
        COMMA, 	//  ',';
        ASSIGN, 	//  '=';
        LPAREN, 	//  '(';
        RPAREN, 	//  ')';
        LCURLY, 	//  '{';
        RCURLY, 	//  '}';
        LBRACK, 	//  '[';
        RBRACK, 	//  ']';


        ID,
        INTEGER,
        BOOL,
        FLOAT,
        WHITESPACE,
        LINE_COMMENT,
        BLOCK_COMMENT,
    }
}
