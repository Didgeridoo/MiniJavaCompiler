using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompilerProject.Variables
{
    public class WhileVariableNameGenerator : DummyVariable
    {
        #region Constructors
        public WhileVariableNameGenerator()
            : base("_while_")
        {
        }
        #endregion

    }
}
