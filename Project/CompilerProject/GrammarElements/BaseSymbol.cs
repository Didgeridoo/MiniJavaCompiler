using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime.Tree;
using MiniJavaSyntax;
using System.Xml.Linq;

namespace CompilerProject.GrammarElements
{
    public class BaseSymbol
    {
        #region Fields
        #endregion
        
        #region Properties
        public List<BaseSymbol> Symbols { get; set; }
        public string Value { get; protected set; }
        #endregion

        #region Constructors
        public BaseSymbol(string value)
            : this(value, new List<BaseSymbol>())
        {
        }

        public BaseSymbol(string value, List<BaseSymbol> symbols)
        {
            Value = value;
            Symbols = new List<BaseSymbol>(symbols);
        }
        #endregion

        #region Methods
        public virtual GrammarMemberType GrammarMember
        {
            get { return GrammarMemberType.None; }
        }

        public override string ToString()
        {
            return Value;
        }


        public override bool Equals(object obj)
        {
            bool result = false;
            if (obj is BaseSymbol)
            {
                result = Value == ((BaseSymbol)obj).Value;
            }
            return result;
        }


        public virtual string ToStringLineTree()
        {
            string result = string.Empty;
            //result += ToString();
            //result += GrammarHelper.Space;
            string innerSymbols = string.Empty;
            //Symbols.Aggregate((one, two) => one.ToStringLineTree() + GrammarHelper.Space + ToString() + GrammarHelper.Space + two.ToStringLineTree());
            Symbols.ForEach(symbol =>
                {
                    if (symbol != null)
                    {
                        //innerSymbols += GrammarHelper.LeftBracket + symbol.ToStringTree() + GrammarHelper.RightBracket;
                        string subSymbols = symbol.ToStringLineTree();
                            innerSymbols += subSymbols ;
                    }
                    if (Symbols.IndexOf(symbol) < Symbols.Count - 1)
                    {
                        innerSymbols += GrammarHelper.Space;
                        innerSymbols += ToString();
                        innerSymbols += GrammarHelper.Space;
                    }
                });

                result +=  innerSymbols ;

            return result;

        }

        protected virtual string FindFirstToken(ref bool isFound)
        {
            string result = string.Empty;
            string innerSymbols = string.Empty;
            foreach (BaseSymbol symbol in Symbols)
            {
                if (symbol != null)
                {
                    string subSymbols = symbol.FindFirstToken(ref isFound);
                    if (isFound)
                    {
                        result = subSymbols;
                        break;
                    }
                    innerSymbols += subSymbols;
                }
                if (Symbols.IndexOf(symbol) < Symbols.Count - 1)
                {
                    innerSymbols += GrammarHelper.Space;
                    innerSymbols += ToString();
                    innerSymbols += GrammarHelper.Space;
                }
            }
            return result;
        }
        protected virtual string FindLastToken(ref bool isFound)
        {
            string result = string.Empty;
            foreach (BaseSymbol symbol in Symbols)
            {
                if (symbol != null)
                {
                    string subSymbols = symbol.FindLastToken(ref isFound);
                    if (isFound)
                    {
                        result = subSymbols;
                        isFound = false;
                    }
                }
            }
            return result;
        }

        public virtual string ToStringInfo()
        {
            string result = string.Empty;
            bool isFound = false;
            result = string.Format("Начало {0}", FindFirstToken(ref isFound));
            isFound = false;
            string resultLast = string.Format("{0}Конец {1}", GrammarHelper.NewLineSymbol, FindLastToken(ref isFound));
            return result + resultLast;

        }

        public virtual string ToStringTree()
        {
            string result = string.Empty;
            result += ToString();
            result += GrammarHelper.Space;
            string innerSymbols = string.Empty;
            Symbols.ForEach(symbol =>
                {
                    if (symbol != null)
                    {
                        //innerSymbols += GrammarHelper.LeftBracket + symbol.ToStringTree() + GrammarHelper.RightBracket;
                        string subSymbols = symbol.ToStringTree();
                        if (!string.IsNullOrEmpty(subSymbols))
                        {
                            innerSymbols += GrammarHelper.LeftBracket + subSymbols + GrammarHelper.RightBracket;
                        }
                    }
                    innerSymbols += GrammarHelper.Space;
                });
            if (!string.IsNullOrEmpty(innerSymbols))
            {
                result += GrammarHelper.LeftBracket + innerSymbols + GrammarHelper.RightBracket;
            }

            return result;

        }


        public virtual XElement ToXmlTree()
        {
            XElement elements = new XElement(ToString());
            Symbols.ForEach(symbol =>
            {
                if (symbol != null)
                {
                    XElement el = symbol.ToXmlTree();
                    elements.Add(el);
                }
            });

            return elements;

        }


        public void SaveToFile(string fileName)
        {
            XElement elements = ToXmlTree();
            elements.Save(fileName);
        }
        
        #endregion

    }
}
