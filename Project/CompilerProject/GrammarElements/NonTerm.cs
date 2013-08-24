using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompilerProject.GrammarElements
{
    public class NonTerm : BaseSymbol
    {

        #region Fields
        #endregion


        #region Properties
        //public string Value { get; private set; }
        public NonTermType TypeNonTerm { get; set; }
        
        public override GrammarMemberType GrammarMember
        {
            get
            {
                return GrammarMemberType.NonTerm;
            }
        }

        #endregion

        #region Constructors

        public NonTerm(NonTermType nonTermType, List<BaseSymbol> symbols)
            : base(string.Empty, symbols)
        {
            TypeNonTerm = nonTermType;
        }

        public NonTerm(NonTermType nonTermType, BaseSymbol symbol) :
            base(string.Empty, new List<BaseSymbol>() { symbol })
        {
            TypeNonTerm = nonTermType;
        }

        public NonTerm(NonTermType nonTermType)
            : base(string.Empty, new List<BaseSymbol>())
        {
            TypeNonTerm = nonTermType;
        }

        public NonTerm(string value)
            : base(value)
        {

        }

        public NonTerm()
            : base(string.Empty)
        {

        }

        public NonTerm(NonTerm nonTerm)
            : base(nonTerm.Value, nonTerm.Symbols)
        {
            TypeNonTerm = nonTerm.TypeNonTerm;
        }


        #endregion



        #region Methods


        public override string ToString()
        {
            string result = string.Empty;
            result += TypeNonTerm;
            return result;
        }

        public static IList<NonTerm> FromStringList(IList<string> lineNonTerms)
        {
            var nonTerms = from element
                        in lineNonTerms
                        select new NonTerm(element);
            return nonTerms.ToList();
        }



        #endregion
    }
}
