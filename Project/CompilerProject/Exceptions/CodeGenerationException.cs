using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CompilerProject.GrammarElements;

namespace CompilerProject.Exceptions
{
    public enum CodeGenerationExceptionType
    {
        UnknownType,
    }

    public class CodeGenerationException : CompilerException
    {
        public CodeGenerationExceptionType ExceptionType { get; private set; }

        public CodeGenerationException(string message, string tokenValue, Exception innerException)
            : base(message, innerException)
        {

            CompilerMessage = string.Format("{0}: {1}", message, tokenValue);
            if (innerException != null)
            {
                CompilerMessage += string.Format("{0}Внутреннее исключение [{1}]{0}", GrammarHelper.NewLineSymbol, innerException.Message, innerException.StackTrace);
            }
        }


        public CodeGenerationException(CodeGenerationExceptionType exceptionType)
            : base("")
        {
            ExceptionType = exceptionType;
        }

    }
}
