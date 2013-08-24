using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompilerProject.GrammarElements
{
    public static class NonTermFactory
    {
        public static NonTerm CreateProgramStatement(BaseSymbol mainClass, List<BaseSymbol> classes)
        {
            List<BaseSymbol> declarations = new List<BaseSymbol>() { mainClass };
            declarations.AddRange(classes);
            return new NonTerm(NonTermType.Program, declarations);
        }

        public static NonTerm CreateMainClassDecl(Token id, List<BaseSymbol> statements)
        {
            List<BaseSymbol> declarations = new List<BaseSymbol>() { id };
            declarations.AddRange(statements);
            return new NonTerm(NonTermType.MainClass, declarations);
        }

        public static void GetMainClassDecl(NonTerm nonTerm, out Token idClass, out List<BaseSymbol> statements)
        {
            idClass = nonTerm.Symbols[0] as Token;
            statements = nonTerm.Symbols.Skip(1).ToList();
        }


        public static NonTerm CreateClassVarDecl(BaseSymbol typeDeclaration, BaseSymbol id)
        {
            List<BaseSymbol> declarations = new List<BaseSymbol>() { typeDeclaration, id };

            return new NonTerm(NonTermType.ClassVarDecl, declarations);
        }

        public static void GetClassVarDecl(NonTerm nonTerm, out Token typeDeclaration, out Token id)
        {
            typeDeclaration = nonTerm.Symbols[0] as Token;
            id = nonTerm.Symbols[1] as Token;
        }


        public static NonTerm CreateClassDecl(BaseSymbol classId, BaseSymbol extendsClause, List<BaseSymbol> classes, List<BaseSymbol> methods)
        {
            List<BaseSymbol> declarations = new List<BaseSymbol>() { classId };
            declarations.Add(extendsClause);
            declarations.AddRange(classes);
            declarations.AddRange(methods);

            return new NonTerm(NonTermType.Class, declarations);
        }

        public static void GetClassId(NonTerm nonTerm, out Token classId)
        {
            classId = nonTerm.Symbols[0] as Token;
        }

        public static void GetClassDecl(NonTerm nonTerm, out Token classId, /*out BaseSymbol extendsClause, */out List<BaseSymbol> declarations)
        {
            GetClassId(nonTerm, out classId);
            //extendsClause = nonTerm.Symbols[1];
            declarations = nonTerm.Symbols.Skip(2).ToList();
        }

        public static void GetClassDecl(NonTerm nonTerm, out Token classId, out NonTerm extendsClause)
        {
            GetClassId(nonTerm, out classId);
            extendsClause = nonTerm.Symbols[1] as NonTerm;
        }


        public static NonTerm CreateMethodDecl(BaseSymbol typeDeclaration, BaseSymbol methodId,
                                               List<BaseSymbol> variables, List<BaseSymbol> statements, BaseSymbol expression)
        {
            List<BaseSymbol> declarations = new List<BaseSymbol>() { typeDeclaration, methodId };
            declarations.Add(CreateFormalArgumentListDeclaration(variables));
            declarations.Add(CreateStatementList(statements));
            declarations.Add(expression);

            return new NonTerm(NonTermType.Method, declarations);
        }


        public static void GetMethodSignature(BaseSymbol nonTerm, out Token typeDeclaration, out Token methodId, out List<BaseSymbol> variables)
        {
            typeDeclaration = nonTerm.Symbols[0] as Token;
            methodId = nonTerm.Symbols[1] as Token;
            variables = nonTerm.Symbols[2].Symbols;
        }

        public static void GetMethodDecl(BaseSymbol nonTerm, out Token typeDeclaration, out Token methodId,
                                         out List<BaseSymbol> variables, out BaseSymbol statements, out BaseSymbol returnExpression)
        {
            GetMethodSignature(nonTerm, out typeDeclaration, out methodId, out variables);
            statements = nonTerm.Symbols[3];
            returnExpression = nonTerm.Symbols[4];
        }


        public static NonTerm CreateExtendsClause(BaseSymbol id)
        {
            return new NonTerm(NonTermType.ExtendsClause, id);
        }

        public static BaseSymbol CreateArrayDecl(BaseSymbol type, int arrayDimention)
        {
            (type as Token).CountArrayDimention = arrayDimention;
            return type;
        }

        public static NonTerm CreateFormalArgumentListDeclaration(List<BaseSymbol> variables)
        {
            return new NonTerm(NonTermType.FormalArgumentList, variables);
        }

        public static void GetFormalArgumentDeclaration(BaseSymbol nonTerm, out Token typeDeclaration, out Token argumentId)
        {
            typeDeclaration = nonTerm.Symbols[0] as Token;
            argumentId = nonTerm.Symbols[1] as Token;
        }

        public static NonTerm CreateStatementList(List<BaseSymbol> statements)
        {
            return new NonTerm(NonTermType.StatementList, statements);
        }

        public static NonTerm CreateIfStatement(BaseSymbol expression, BaseSymbol statementIf)
        {
            List<BaseSymbol> declarations = new List<BaseSymbol>() { expression, statementIf };

            return new NonTerm(NonTermType.IfStatement, declarations);
        }

        public static NonTerm CreateIfElseStatement(BaseSymbol expression, BaseSymbol statementIf, BaseSymbol statementElse)
        {
            List<BaseSymbol> declarations = new List<BaseSymbol>() { expression, statementIf, statementElse };

            return new NonTerm(NonTermType.IfElseStatement, declarations);
        }

        public static void GetIfStatement(BaseSymbol nonTerm, out BaseSymbol expression, out BaseSymbol statement)
        {
            expression = nonTerm.Symbols[0];
            statement = nonTerm.Symbols[1];
        }
        public static void GetElseStatement(BaseSymbol nonTerm, out BaseSymbol statement)
        {
            statement = nonTerm.Symbols[2];
        }


        public static NonTerm CreateWhileStatement(BaseSymbol expression, BaseSymbol statement)
        {
            List<BaseSymbol> declarations = new List<BaseSymbol>() { expression, statement };

            return new NonTerm(NonTermType.WhileStatement, declarations);
        }

        public static void GetWhileStatement(BaseSymbol nonTerm, out BaseSymbol expression, out BaseSymbol statement)
        {
            expression = nonTerm.Symbols[0];
            statement = nonTerm.Symbols[1];
        }


        public static NonTerm CreatePrintStatement(BaseSymbol expression)
        {
            return new NonTerm(NonTermType.PrintStatement, expression);
        }

        public static void GetPrintStatement(BaseSymbol nonTerm, out BaseSymbol expression)
        {
            expression = nonTerm.Symbols[0];
        }


        public static NonTerm CreateAssignVarStatementt(BaseSymbol variable, BaseSymbol expression)
        {
            List<BaseSymbol> declarations = new List<BaseSymbol>() { variable, expression };

            return new NonTerm(NonTermType.VarStatement, declarations);
        }

        public static void GetAssignVarStatement(BaseSymbol nonTerm, out BaseSymbol variable, out BaseSymbol expression)
        {
            variable = nonTerm.Symbols[0];
            expression = nonTerm.Symbols[1];
        }


        public static NonTerm CreateAssignIdStatement(BaseSymbol id, BaseSymbol expression)
        {
            List<BaseSymbol> declarations = new List<BaseSymbol>() { id, expression };

            return new NonTerm(NonTermType.IdStatement, declarations);
        }

        public static void GetAssignIdStatement(BaseSymbol nonTerm, out Token idToken, out BaseSymbol expression)
        {
            idToken = nonTerm.Symbols[0] as Token;
            expression = nonTerm.Symbols[1];
        }


        public static NonTerm CreateNewStatement(BaseSymbol expression)
        {
            return new NonTerm(NonTermType.NewStatement, expression);
        }

        public static void GetNewStatement(BaseSymbol nonTerm, out Token expression)
        {
            expression = nonTerm.Symbols[0] as Token;
        }


        public static NonTerm CreateTypeDecl(BaseSymbol typeToken)
        {
            return new NonTerm(NonTermType.Type, typeToken);
        }

        public static NonTerm CreateVarDecl(BaseSymbol typeDeclaration, BaseSymbol id)
        {
            List<BaseSymbol> declarations = new List<BaseSymbol>() { typeDeclaration, id };

            return new NonTerm((typeDeclaration as Token).IsArray() ? NonTermType.ArrayVariable : NonTermType.Variable, declarations);
        }

        public static void GetVarDecl(BaseSymbol nonTerm, out Token typeDeclaration, out Token id)
        {
            typeDeclaration = nonTerm.Symbols[0] as Token;
            id = nonTerm.Symbols[1] as Token;
        }


        public static NonTerm CreateUnaryMinusExpression(BaseSymbol expression)
        {
            return new NonTerm(NonTermType.UnaryMinusExpression, expression);
        }

        public static NonTerm CreateUnaryNotExpression(BaseSymbol expression)
        {
            return new NonTerm(NonTermType.UnaryNotExpression, expression);
        }

        /*public static NonTerm CreateSubArgumentListExpression(BaseSymbol id, List<BaseSymbol> argumentList)
        {
            List<BaseSymbol> declarations = new List<BaseSymbol>() { id };
            declarations.AddRange(argumentList);

            return new NonTerm(NonTermType.SubArgumentListExpression, declarations);
        }

        public static NonTerm CreateArgumentListExpression(BaseSymbol expression, List<BaseSymbol> argumentList)
        {
            List<BaseSymbol> declarations = new List<BaseSymbol>() { expression };
            declarations.AddRange(argumentList);

            return new NonTerm(NonTermType.ArgumentListExpression, declarations);
        }*/

        public static NonTerm CreateArgumentListExpression(List<BaseSymbol> argumentList)
        {
            List<BaseSymbol> declarations = new List<BaseSymbol>(argumentList);

            return new NonTerm(NonTermType.ArgumentListExpression, declarations);
        }

        public static NonTerm CreateMethodCallListExpression(BaseSymbol expression, BaseSymbol id, BaseSymbol methodArguments)
        {
            NonTerm result = null;
            if ((expression is Token) && (expression as Token).Value.Equals(GrammarHelper.This))
            {
                result = CreateMethodCallListExpression(id, methodArguments);
            }
            else
            {
                List<BaseSymbol> declarations = new List<BaseSymbol>() { expression, id, methodArguments };
                result = new NonTerm(NonTermType.MethodCallExpression, declarations);
            }
            return result;
        }

        public static NonTerm CreateMethodCallListExpression(BaseSymbol id, BaseSymbol methodArguments)
        {
            List<BaseSymbol> declarations = new List<BaseSymbol>() { id, methodArguments };
            return new NonTerm(NonTermType.MethodThisCallExpression, declarations);
        }


        public static void GetMethodCallExpression(NonTerm nonTerm, out BaseSymbol symbol, out Token idMethodName, out List<BaseSymbol> methodArguments)
        {
            symbol = nonTerm.Symbols[0];
            idMethodName = nonTerm.Symbols[1] as Token;
            methodArguments = nonTerm.Symbols[2].Symbols;
        }

        public static void GetMethodCallExpression(NonTerm nonTerm, out Token idMethodName, out List<BaseSymbol> methodArguments)
        {
            idMethodName = nonTerm.Symbols[0] as Token;
            methodArguments = nonTerm.Symbols[1].Symbols;
        }



        public static BaseSymbol CreateLogicalOrExpression(List<BaseSymbol> expressions)
        {
            if (expressions.Count == 1)
            {
                return expressions.First();
            }
            else
            {
                return new NonTerm(NonTermType.LogicalOrExpression, expressions);
            }
        }

        public static BaseSymbol CreateLogicalAndExpression(List<BaseSymbol> expressions)
        {
            BaseSymbol symbol = null;
            if (expressions.Count == 1)
            {
                symbol = expressions.First();
            }
            else
            {
                symbol = new NonTerm(NonTermType.LogicalAndExpression, expressions);
            }
            return symbol;
        }

        public static BaseSymbol CreateEqualityExpression(int tokenType, List<BaseSymbol> expressions)
        {
            BaseSymbol symbol = null;
            if (expressions.Count == 1)
            {
                symbol = expressions.First();
            }
            else
            {
                NonTermType nonTermType = GrammarHelper.TokenTypeToNonTermDictionary.ContainsKey(tokenType) ?
                    GrammarHelper.TokenTypeToNonTermDictionary[tokenType] : NonTermType.None;
                symbol = new NonTerm(nonTermType, expressions);
            }
            return symbol;
        }

        public static BaseSymbol CreateExpression(int tokenType, BaseSymbol firstExpression, BaseSymbol secondExpression)
        {
            List<BaseSymbol> declarations = new List<BaseSymbol>() { firstExpression, secondExpression };

            NonTermType nonTermType = GrammarHelper.TokenTypeToNonTermDictionary.ContainsKey(tokenType) ?
                GrammarHelper.TokenTypeToNonTermDictionary[tokenType] : NonTermType.None;

            return new NonTerm(nonTermType, declarations);
        }

        public static NonTerm CreateExpressionDecl()
        {
            return new NonTerm(NonTermType.Expression);
        }


        public static NonTerm CreateFieldExpression(BaseSymbol expression, Token fieldId)
        {
            List<BaseSymbol> declarations = new List<BaseSymbol>() { expression, fieldId };

            return new NonTerm(NonTermType.FieldExpression, declarations);
        }


        public static NonTerm CreateArrayAssignIdStatement(Token id, BaseSymbol expression, List<BaseSymbol> bracketExpressionList)
        {
            List<BaseSymbol> declarations = new List<BaseSymbol>() { id, expression };

            declarations.AddRange(bracketExpressionList);

            return new NonTerm(NonTermType.ArrayIdStatement, declarations);
        }

        public static void GetArrayAssignIdStatement(BaseSymbol nonTerm, out Token id,
                                                     out BaseSymbol expression, out List<BaseSymbol> bracketExpressionList)
        {
            id = nonTerm.Symbols[0] as Token;
            expression = nonTerm.Symbols[1];
            bracketExpressionList = nonTerm.Symbols.Skip(2).ToList();
        }


        public static NonTerm CreateLengthFunctionExpression(BaseSymbol expression)
        {
            return new NonTerm(NonTermType.LengthFunctionExpression, expression);
        }

        public static void GetLengthFunctionExpression(BaseSymbol nonTerm, out Token lengthFunction)
        {
            lengthFunction = nonTerm.Symbols[0] as Token;
        }


        public static NonTerm CreateNewArrayStatement(BaseSymbol typeDeclaration, List<BaseSymbol> bracketExpressionList)
        {
            List<BaseSymbol> declarations = new List<BaseSymbol>() { typeDeclaration };

            declarations.AddRange(bracketExpressionList);

            return new NonTerm(NonTermType.NewArrayStatement, declarations);
        }

        public static void GetNewArrayStatement(BaseSymbol nonTerm, out Token typeDeclaration, out List<BaseSymbol> bracketExpressionList)
        {
            typeDeclaration = nonTerm.Symbols[0] as Token;
            bracketExpressionList = nonTerm.Symbols.Skip(1).ToList();
        }


        public static NonTerm CreateBracketExpression(BaseSymbol expression)
        {
            return new NonTerm(NonTermType.BracketExpression, expression);
        }


        public static NonTerm CreateArrayIndiciesStatement(BaseSymbol id, List<BaseSymbol> bracketExpressionList)
        {
            List<BaseSymbol> declarations = new List<BaseSymbol>() { id };

            declarations.AddRange(bracketExpressionList);

            return new NonTerm(NonTermType.ArrayIndiciesStatement, declarations);
        }

        public static void GetArrayIndiciesStatement(BaseSymbol nonTerm, out Token id, out List<BaseSymbol> bracketExpressionList)
        {
            id = nonTerm.Symbols[0] as Token;
            bracketExpressionList = nonTerm.Symbols.Skip(1).ToList();
        }


        public static void GetBinaryExpression(BaseSymbol nonTerm, out BaseSymbol firstExpression, out BaseSymbol secondExpression)
        {
            firstExpression = nonTerm.Symbols[0];
            secondExpression = nonTerm.Symbols[1];
        }

        public static void GetUnaryExpression(BaseSymbol nonTerm, out BaseSymbol expression)
        {
            expression = nonTerm.Symbols[0];
        }

    }
}
