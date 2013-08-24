using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using MiniJavaSyntax;

namespace CompilerProject.GrammarElements
{
    public static class GrammarHelper
    {
        public static Dictionary<int, NonTermType> TokenTypeToNonTermDictionary = new Dictionary<int, NonTermType>() { 
            {MiniJavaParser.EQUAL, NonTermType.EqualExpression},
            {MiniJavaParser.NEQUAL, NonTermType.NotEqualExpression},

            {MiniJavaParser.LESS, NonTermType.LessExpression},
            {MiniJavaParser.LESSEQ, NonTermType.LessEqExpression},
            {MiniJavaParser.MORE, NonTermType.MoreExpression},
            {MiniJavaParser.MOREEQ, NonTermType.MoreEqExpression},

            {MiniJavaParser.PLUS, NonTermType.PlusExpression},
            {MiniJavaParser.MINUS, NonTermType.MinusExpression},

            {MiniJavaParser.STAR, NonTermType.MultiplyExpression},
            {MiniJavaParser.DIV, NonTermType.DivisionExpression},

        };

        public static Dictionary<TokenType, Type> TokenTypeToTypeDictionary = new Dictionary<TokenType, Type>() { 
            {TokenType.INT, typeof(int)},
            {TokenType.INTEGER, typeof(int)},
            {TokenType.FLOAT, typeof(double)},
            {TokenType.STRING, typeof(string)},
            {TokenType.BOOL, typeof(bool)},
            {TokenType.BOOLEAN, typeof(bool)},
            {TokenType.TRUE, typeof(bool)},
            {TokenType.FALSE, typeof(bool)},
            {TokenType.NULL, typeof(Nullable)},
        };

        public static List<NonTermType> ExpressoinTypeList = new List<NonTermType>() { 
                    NonTermType.EqualExpression,
                    NonTermType.NotEqualExpression,
                    NonTermType.LessExpression,
                    NonTermType.LessEqExpression,
                    NonTermType.MoreExpression,
                    NonTermType.MoreEqExpression,

                    NonTermType.PlusExpression,
                    NonTermType.MinusExpression,

                    NonTermType.MultiplyExpression,
                    NonTermType.DivisionExpression,
        };

        public static List<NonTermType> StatementTypeList = new List<NonTermType>() { 
                    NonTermType.IfStatement,
                    NonTermType.IfElseStatement,
                    NonTermType.WhileStatement,
                    NonTermType.PrintStatement,
                    NonTermType.IdStatement,
                    NonTermType.VarStatement,

                    //NonTermType.StatementList,
                    NonTermType.Statement,

                    NonTermType.ArrayVariable,
                    NonTermType.ArrayIndiciesStatement,
                    NonTermType.ArrayIdStatement,
                    NonTermType.NewArrayStatement,
        };

        public static List<NonTermType> BlockVariablesList = new List<NonTermType>() { 
                    NonTermType.Class,
                    NonTermType.Method,
                    NonTermType.IfStatement,
                    NonTermType.IfElseStatement,
                    NonTermType.WhileStatement,

        };


        public static string NewLineSymbol
        {
            get { return Environment.NewLine; }
        }

        public static string NewSymbolSign
        {
            get { return "\'"; }
        }

        public static string Space
        {
            get { return " "; }
        }

        public static string LeftBracket
        {
            get { return "("; }
        }
        public static string RightBracket
        {
            get { return ")"; }
        }

        public static string This
        {
            get { return "this"; }
        }


        public static string[] WhiteSpaces
        {
            get
            {
                return new string[] { " ", "\n", "\r", "\t" };
            }
        }

        public static List<T> Union<T>(IList<T> one, IList<T> two)
        {
            List<T> union = new List<T>(one);
            union.AddRange(two);
            return union;
        }


        public static string ListToString(IEnumerable list)
        {
            string result = string.Empty;
            if (list != null)
            {
                foreach (object element in list)
                {
                    result += string.Format("{0}{1}", element, GrammarHelper.NewLineSymbol);
                }
            }

            return result;
        }

        public static string ListToLineString(IEnumerable list)
        {
            string result = string.Empty;
            if (list != null)
            {
                foreach (object element in list)
                {
                    result += string.Format("{0} ", element);
                }
                if (result != string.Empty)
                {
                    result = result.Remove(result.Length - 1);
                }
            }

            return result;
        }
    }

    public static class StringExtensions
    {
        public static string TokenToString(this string str)
        {
            string result = str;
            if (str.Equals("\r"))
            {
                result = "\\r";
            }
            else if (str.Equals("\n"))
            {
                result = "\\n";
            }
            else if (str.Equals("\t"))
            {
                result = "\\t";
            }
            /*else 

            for (int i = str.Length - 1; i >= 0; i--)
            {
                if (str[i] == '\\')
                    result = result.Insert(i, "\\");
            }*/
            return result;
        }
    }

}
