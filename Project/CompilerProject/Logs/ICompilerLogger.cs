using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CompilerProject.GrammarElements;

namespace CompilerProject.Logs
{
    public interface ICompilerLogger
    {
        void PrintGenerateNonTerm(NonTerm nonTerm);
        void PrintAddLocalVariables(IList<string> currentBlockVariableList);
        void PrintDeleteLocalVariables(IList<string> currentBlockVariableList);

        void Save();

        void PrintClassesMethods(Dictionary<string, Dictionary<string, TriAxis.RunSharp.MethodGen>> methodsTables);

        void PrintAddtFormalArgumentList(List<string> currentFormalArgumentList);
        void PrintRefreshFormalArgumentList(List<string> currentFormalArgumentList);
    }
}
