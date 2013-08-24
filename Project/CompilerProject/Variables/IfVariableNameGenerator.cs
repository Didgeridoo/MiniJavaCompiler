using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompilerProject.Variables
{
    public class IfVariableNameGenerator : DummyVariable
    {
        #region Constructors
        public IfVariableNameGenerator()
            : base("_if_")
        {
        }
        #endregion

    }
}
