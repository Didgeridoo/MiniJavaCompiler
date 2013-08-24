using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CompilerProject.GrammarElements;
using MiniJavaSyntax;
using Antlr.Runtime;
using System.Xml.Linq;

namespace CompilerProject
{
    public class Lexer
    {        
        #region Fields
        #endregion


        #region Properties
        public CommonTokenStream TokenStream { get; private set; }
        #endregion

        #region Constructors
        public Lexer(string fileName)
        {
            Antlr.Runtime.ANTLRFileStream fileStream = new Antlr.Runtime.ANTLRFileStream(fileName);
            minijava2Lexer lexer = new minijava2Lexer(fileStream);
            TokenStream = new Antlr.Runtime.CommonTokenStream(lexer);
            //SaveToFile("lexer1.txt");
        }

        #endregion


        #region Methods

        public void SaveToFile(string fileName)
        {
            List<IToken> tokens = TokenStream.GetTokens();

            XElement xElement = new XElement("Lexer", 
                from token in tokens
                            select new XElement("Token",
                                new XAttribute("Text", token.Text.TokenToString()),
                                new XAttribute("TokenIndex", token.TokenIndex),
                                new XAttribute("Type", token.Type),
                                new XAttribute("Line", token.Line),
                                new XAttribute("CharPositionInLine", token.CharPositionInLine),
                                new XAttribute("StartIndex", token.StartIndex),
                                new XAttribute("StopIndex", token.StopIndex)
                                )
                            );

            //Console.WriteLine(xElement);
            xElement.Save(fileName);
        }

        #endregion

    }
}
