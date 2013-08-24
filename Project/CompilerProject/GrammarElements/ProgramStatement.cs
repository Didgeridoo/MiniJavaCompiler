using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompilerProject.GrammarElements
{
    public class ProgramStatement : NonTerm
    {
        public ProgramStatement(MainClassDecl mainClass, List<ClassDecl> classes)
            : base(NonTermType.Program)
        {
            MainClass = mainClass;
            Classes = classes;
        }

        public MainClassDecl MainClass { get; set; }

        public List<ClassDecl> Classes { get; set; }
    }


    public class MainClassDecl : NonTerm
    {
        public MainClassDecl(List<StatementDecl> statements)
            :base(NonTermType.MainClass)
        {
            Statements = statements;
        }

        public List<StatementDecl> Statements { get; set; }
    }


    public class ClassDecl : NonTerm
    {
        public ClassDecl(Token classId, List<ClassDecl> classes, List<MethodDecl> methods)
            :base(NonTermType.Class)
        {
            ClassId = classId;
            Classes = classes;
            Methods = methods;
        }

        public Token ClassId { get; set; }
        public List<ClassDecl> Classes { get; set; }
        public List<MethodDecl> Methods { get; set; }
    }


    /*
     PUBLIC^ type ID LPAREN! formalList RPAREN! LCURLY!
			stmtList
			RETURN! expr SEMI!
		RCURLY!
     */
    public class MethodDecl : NonTerm
    {
        public MethodDecl(TypeDecl typeDeclaration, Token methodId, 
            List<VarDecl> variables, List<StatementDecl> statements, ExpressionDecl expression)
            : base(NonTermType.Method)
        {
            TypeDeclaration = typeDeclaration;
            MethodId = methodId;
            Variables = variables;
            Statements = statements;
            Expression = expression;
        }

        public TypeDecl TypeDeclaration { get; set; }
        public Token MethodId { get; set; }
        public List<VarDecl> Variables { get; set; }
        public List<StatementDecl> Statements { get; set; }
        public ExpressionDecl Expression { get; set; }
    }

    public class ExtendsClause : NonTerm
    {
        public ExtendsClause(Token id)
            : base(NonTermType.ExtendsClause)
        {
            Id = id;
        }

        public Token Id { get; set; }
    }

    /*
     stmt
	:
		LCURLY! stmtList RCURLY!
	| 
		IF^ LPAREN! expr RPAREN! stmt ELSE! stmt
	| 
		WHILE^ LPAREN! expr RPAREN! stmt
	| 
		PRINT^ LPAREN! expr RPAREN! SEMI!
	| 
		(varDecl | ID) (ASSIGN^ expr)? SEMI!
	;
     */
    public class StatementDecl : NonTerm
    {
        public StatementDecl()
            : base(NonTermType.Statement)
        { }

        public StatementDecl(NonTermType nonTermType)
            : base(nonTermType)
        { }

    }
    public class StatementList : StatementDecl
    {
        public StatementList(List<StatementDecl> statements)
            : base(NonTermType.StatementList)
        {
            Statements = statements;
        }

        public List<StatementDecl> Statements { get; set; }
    }
    public class IfStatement : StatementDecl
    {
        public IfStatement(ExpressionDecl expression, StatementDecl statementIf, StatementDecl statementElse)
            : base(NonTermType.IfStatement)
        {
            Expression = expression;
            StatementIf = statementIf;
            StatementElse = statementElse;
        }

        public ExpressionDecl Expression { get; set; }
        public StatementDecl StatementIf { get; set; }
        public StatementDecl StatementElse { get; set; }
    }
    public class WhileStatement : StatementDecl
    {
        public WhileStatement(ExpressionDecl expression, StatementDecl statement)
            : base(NonTermType.WhileStatement)
        {
            Expression = expression;
            Statement = statement;
        }

        public ExpressionDecl Expression { get; set; }
        public StatementDecl Statement { get; set; }
    }
/*    public class AssignStatement : StatementDecl
    {
        public AssignStatement(VarDecl variable, ExpressionDecl expression)
            : base(NonTermType.VarStatement)
        {
            Expression = expression;
            Variable = variable;
        }

        public AssignStatement(Token id, ExpressionDecl expression)
            : base(NonTermType.IdStatement)
        {
            Expression = expression;
            Id = id;
        }

        public VarDecl Variable { get; set; }
        Token Id { get; set; }
        public ExpressionDecl Expression { get; set; }
    }*/

    public class AssignVarStatement : StatementDecl
    {
        public AssignVarStatement(VarDecl variable, ExpressionDecl expression)
            : base(NonTermType.VarStatement)
        {
            Expression = expression;
            Variable = variable;
        }

        public VarDecl Variable { get; set; }
        public ExpressionDecl Expression { get; set; }
    }

    public class AssignIdStatement : StatementDecl
    {
        public AssignIdStatement(Token id, ExpressionDecl expression)
            : base(NonTermType.IdStatement)
        {
            Expression = expression;
            Id = id;
        }

        Token Id { get; set; }
        public ExpressionDecl Expression { get; set; }
    }



    public class TypeDecl : NonTerm
    {
        public TypeDecl(Token typeToken)
            : base(NonTermType.Type)
        {
            TypeToken = typeToken;
        }

        Token TypeToken { get; set; }
    }

    public class VarDecl : NonTerm
    {
        public VarDecl(TypeDecl typeDeclaration, Token id)
            :base(NonTermType.Variable)
        {
            TypeDeclaration = typeDeclaration;
            Id = id;
        }

        TypeDecl TypeDeclaration { get; set; }
        Token Id { get; set; }
    }

    public class ExpressionDecl : NonTerm
    {
        public ExpressionDecl()
            :base(NonTermType.Expression)
        { 
            
        }

    }



}
