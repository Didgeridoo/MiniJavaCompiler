using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleAppCompiler.CommandArguments
{
    /// <summary>
    /// ��������� ��������� ������
    /// </summary>
    public class CommandArgs
    {
        #region Properties

        /// <summary>
        /// ������� ��������-��������  
        /// </summary>
        public Dictionary<string, string> ArgPairs { get; private set; }

        /// <summary>
        /// ������ ���������� ��� ������
        /// </summary>
        public List<string> Params { get; private set; }

        #endregion

        #region Constructors

        public CommandArgs()
        {
            ArgPairs = new Dictionary<string, string>();
            Params = new List<string>();
        }

        #endregion

        #region Methods

        public void AddArgPair(string key, string value)
        {
            ArgPairs.Add(key, value);
        }

        public void AddParam(string param)
        {
            Params.Add(param);
        }

        #endregion

    }
}