using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Globalization;
using CompilerProject;
using CompilerProject.Exceptions;
using Antlr.Runtime;

namespace CompilerProject.GrammarElements
{
    public class Token : BaseSymbol
    {
        #region Fields
        #endregion

        #region Properties
        public TokenType TypeToken { get; protected set; }
        public int IntTypeToken { get; protected set; }
        public object ValueObject { get; protected set; }
        public IToken AntlrToken { get; protected set; }
        public int CountArrayDimention { get; set; }

        public override GrammarMemberType GrammarMember
        {
            get
            {
                return GrammarMemberType.Token;
            }
        }

        public Type TypeOf
        {
            get
            {
                if (!GrammarHelper.TokenTypeToTypeDictionary.ContainsKey(TypeToken))
                {
                    throw new CodeGenerationException(CodeGenerationExceptionType.UnknownType);
                }
                Type type = GrammarHelper.TokenTypeToTypeDictionary[TypeToken];
                if (IsArray())
                {
                    type = type.MakeArrayType(CountArrayDimention);
                }
                return type;
            }
        }

        public Type TypeOfWithoutArray
        {
            get
            {
                if (!GrammarHelper.TokenTypeToTypeDictionary.ContainsKey(TypeToken))
                {
                    throw new CodeGenerationException(CodeGenerationExceptionType.UnknownType);
                }
                return GrammarHelper.TokenTypeToTypeDictionary[TypeToken];
            }
        }

        #endregion

        #region Constructors

        public Token(TokenType tokenType, string value, IToken antlrToken) :
            base(value)
        {
            TypeToken = tokenType;

            CountArrayDimention = 0;

            try
            {
                switch (tokenType)
                {
                    case TokenType.INT:
                        ValueObject = int.Parse(value);
                        break;
                    case TokenType.INTEGER:
                        ValueObject = int.Parse(value);
                        break;
                    case TokenType.FLOAT:
                        ValueObject = double.Parse(value, CultureInfo.InvariantCulture.NumberFormat);
                        break;
                    case TokenType.BOOL:
                        ValueObject = bool.Parse(value);
                        break;
                    case TokenType.TRUE:
                        ValueObject = bool.Parse(value);
                        break;
                    case TokenType.FALSE:
                        ValueObject = bool.Parse(value);
                        break;
                    case TokenType.NULL:
                        ValueObject = null;
                        break;
                    default:
                        ValueObject = value;
                        break;
                }
                AntlrToken = antlrToken;
            }
            catch (OverflowException ex)
            {
                throw new ParserException(MessagesHelper.TokenParseOverflowEx, value, tokenType, antlrToken, ex);
            }
        }

        /*public Token(TokenType tokenType, string value) :
            this(tokenType, value, null)
        {
            
        }*/


        public Token(TokenType tokenType, IToken antlrToken) :
            base(string.Empty)
        {
            TypeToken = tokenType;
            AntlrToken = antlrToken;
        }

        /*public Token(int tokenType, string value) :
            base(value)
        {
            IntTypeToken = tokenType;
        }

        public Token(int tokenType) :
            base(string.Empty)
        {
            IntTypeToken = tokenType;
        }*/

        #endregion

        #region Methods

        public bool IsArray()
        {
            return CountArrayDimention > 0;
        }

        /*public static Token CreateTypeToken()
        {
            return new Token(
        }
        */
        public override string ToStringTree()
        {
            return ToString();
        }

        public override string ToStringLineTree()
        {
            string result = string.Empty;
            result += string.Format(" {0}", Value);
            return result;
        }

        protected override string FindFirstToken(ref bool isFound)
        {
            string result = string.Empty;
            isFound = true;
            result += string.Format(" '{0}'. Позиция в тексте {1} : {2}. № токена {3}. ", Value, 
                AntlrToken.Line, AntlrToken.CharPositionInLine, AntlrToken.TokenIndex, AntlrToken);
            return result;
        }
        protected override string FindLastToken(ref bool isFound)
        {
            return FindFirstToken(ref isFound);
        }

        public override string ToStringInfo()
        {
            string result = string.Empty;
            result += string.Format(" '{0}'. Позиция в тексте {1} : {2}. № токена {3}. ", Value,
                AntlrToken.Line, AntlrToken.CharPositionInLine, AntlrToken.TokenIndex, AntlrToken);
            return result;
        }

        public override XElement ToXmlTree()
        {
            XElement result = null;
            if (!string.IsNullOrEmpty(Value))
            {
                 result = new XElement(TypeToken.ToString(), new XAttribute("Value", Value));
            }
            else
            {
                 result = new XElement(TypeToken.ToString());
            }
            //return new XElement(TypeToken.ToString(), new XAttribute("Value", Value));
            return result;
        }

        public override string ToString()
        {
            string result = string.Empty;
            string arrayResult = string.Empty;
            for (int i = 0; i < CountArrayDimention; i++)
            {
                arrayResult += " []";
            }
            result += string.Format(" |{1}{2}| '{0}'", Value, TypeToken, arrayResult);
            return result;
        }
        #endregion

    }
}
