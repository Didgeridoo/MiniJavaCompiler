using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompilerProject.GrammarElements
{
    public enum NonTermType
    {
        None,
        Program,
        MainClass,
        Class,
        Statement,
        ClassVarDecl,
        ExtendsClause,
        Method,
        Formal,
        Type,

        Expression,
        Constant,

        StatementList,
        IfStatement,
        IfElseStatement,
        WhileStatement,
        PrintStatement,
        VarStatement,
        IdStatement,
        ArrayIdStatement,
        NewStatement,

        NewArrayStatement,
        ArrayIndiciesStatement,


        Variable,
        ArrayVariable,

        LogicalOrExpression,
        LogicalAndExpression,

        //EqualityExpression,
        EqualExpression,
        NotEqualExpression,

        //RelationalExpression,
        LessExpression,
        LessEqExpression,
        MoreExpression,
        MoreEqExpression,

        //AdditiveExpression,
        PlusExpression,
        MinusExpression,

        //MultiplicativeExpression,
        MultiplyExpression,
        DivisionExpression,

        //DecoratedExpression,
        UnaryMinusExpression,
        UnaryNotExpression,
        //SubArgumentListExpression,
        ArgumentListExpression,
        MethodCallExpression,
        //PrimaryExpression,

        LengthFunctionExpression,
        BracketExpression,
        FieldExpression,
        FormalArgumentList,
        MethodThisCallExpression,

        //
        BinaryExpression,
        UnaryExpression,

    }
 
}
