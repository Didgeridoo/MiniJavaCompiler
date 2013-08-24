using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CompilerProject.GrammarElements;
using TriAxis.RunSharp;

namespace CompilerProject.Logs
{
    public class CompilerLogger : ICompilerLogger
    {
        #region Fields
        private StreamWriter _stream;
        #endregion

        #region Properties
        public string FileName { get; private set; }
        #endregion

        #region Constructors
        public CompilerLogger(string fileName)
        {
            FileName = fileName;
            _stream = new StreamWriter(fileName);
        }
        #endregion

        #region Methods

        public void PrintGenerateNonTerm(NonTerm nonTerm)
        {
            _stream.WriteLine("Генерация инструкций для {0}", nonTerm);
        }

        public void Save()
        {
            _stream.Flush();
        }


        public void PrintAddLocalVariables(IList<string> currentBlockVariableList)
        {
            if (currentBlockVariableList.Count > 0)
            {
                string vars = currentBlockVariableList.Aggregate((one, two) => string.Format("{0}, {1}", one, two));
                _stream.WriteLine("Добавлены переменные для блока{0}{1}", GrammarHelper.NewLineSymbol, vars);
            }
            else
            {
                _stream.WriteLine("В данном блоке не встречаются локальные переменные");
            }
        }


        public void PrintDeleteLocalVariables(IList<string> currentBlockVariableList)
        {
            if (currentBlockVariableList.Count > 0)
            {
                string vars = currentBlockVariableList.Aggregate((one, two) => string.Format("{0}, {1}", one, two));
                _stream.WriteLine("Удалены переменные {0}{1}", GrammarHelper.NewLineSymbol, vars);
            }
        }


        public void PrintClassesMethods(Dictionary<string, Dictionary<string, MethodGen>> methodsTables)
        {
            _stream.WriteLine("Генерирование классов и сигнатур методов");
            foreach (KeyValuePair<string, Dictionary<string, MethodGen>> classDecl in methodsTables)
            {
                _stream.WriteLine("Класс {0}", classDecl.Key);

                foreach (var method in classDecl.Value)
                {
                    _stream.WriteLine(" Метод {0} {1}", method.Key, method.Value.ReturnParameter.Type);
                }
            }
            _stream.WriteLine();
        }


        public void PrintAddtFormalArgumentList(List<string> currentFormalArgumentList)
        {
            if (currentFormalArgumentList.Count > 0)
            {
                string vars = currentFormalArgumentList.Aggregate((one, two) => string.Format("{0}, {1}", one, two));
                _stream.WriteLine("Сгенерированы аргументы метода{0}{1}", GrammarHelper.NewLineSymbol, vars);
            }
        }
        public void PrintRefreshFormalArgumentList(List<string> currentFormalArgumentList)
        {
            if (currentFormalArgumentList.Count > 0)
            {
                string vars = currentFormalArgumentList.Aggregate((one, two) => string.Format("{0}, {1}", one, two));
                _stream.WriteLine("Обновлен список текущих аргументов метода{0}{1}", GrammarHelper.NewLineSymbol, vars);
            }
        }

        #endregion

    }
}
