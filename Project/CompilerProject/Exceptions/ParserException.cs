using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime;
using CompilerProject.GrammarElements;

namespace CompilerProject.Exceptions
{
    public class ParserException : CompilerException
    {

        public ParserException(string message, string tokenValue, TokenType tokenType, IToken antlrToken, Exception innerException)
            : base(message, innerException)
        {
            string tokenInfo = string.Format(" Позиция в тексте {0} : {1}. № токена {2}. ",
                antlrToken.Line, antlrToken.CharPositionInLine, antlrToken.TokenIndex); 
            CompilerMessage = string.Format("{0}: {3} '{1}' {5} {2}Внутреннее исключение [{4}]",
                message, tokenValue, GrammarHelper.NewLineSymbol, tokenType, innerException.Message, tokenInfo);
        }

    }
}
