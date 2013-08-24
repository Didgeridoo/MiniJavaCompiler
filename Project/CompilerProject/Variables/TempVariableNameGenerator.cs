using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompilerProject.Variables
{
    public class TempVariableNameGenerator : DummyVariable
    {
        #region Constructors
        public TempVariableNameGenerator()
            : base("_temp_")
        {
        }
        #endregion

    }
}
