using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompilerProject.Variables
{
    public class ValueIncrementation
    {
        #region Fields
        private int _currentValue;
        #endregion

        #region Properties
        #endregion

        #region Constructors
        public ValueIncrementation()
        {
            Reset();
        }
        #endregion

        #region Methods

        public int GetNext()
        {
            return _currentValue++;
        }

        public int GetCurrent()
        {
            return _currentValue;
        }

        public void Reset()
        {
            _currentValue = 1;
        }

        #endregion

    }
}
