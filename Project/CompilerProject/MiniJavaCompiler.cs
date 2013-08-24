using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CompilerProject.CodeGeneration;
using CompilerProject.GrammarElements;
using MiniJavaSyntax;
using System.IO;
using CompilerProject.Exceptions;
using CompilerProject.Logs;

namespace CompilerProject
{
    /// <summary>
    /// Компилятор языка программирования MiniJava
    /// </summary>
    public class MiniJavaCompiler
    {
        #region Static Properties

        public static string LexerPostfix
        {
            get { return "_lexer.txt"; }
        }
        public static string ParserPostfix
        {
            get { return "_parser.txt"; }
        }
        public static string CodegenPostfix
        {
            get { return "_codegen.txt"; }
        }

        #endregion

        #region Properties
        public string FileName { get; private set; }
        public bool HasError { get; private set; }

        #endregion

        #region Constructors
        public MiniJavaCompiler(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException(fileName, MessagesHelper.InputFileNotGiven);
            }

            FileName = fileName;
        }
        #endregion

        #region Methods

        public void Compile()
        {
            Compile(string.Empty);
        }

        public void Compile(string outputPath)
        {
            Console.WriteLine(MessagesHelper.AboutProgram);

            HasError = false;
            string fileNameCommon = GetFileName(outputPath);
            Run(fileNameCommon);
        }

        private string GetFileName(string outputPath)
        {
            string fileNameCommon = Path.GetFileNameWithoutExtension(FileName);
            string path = !Path.IsPathRooted(outputPath) 
                ? Path.Combine(Directory.GetCurrentDirectory(), outputPath ?? string.Empty) 
                : outputPath;

            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("{0}: {1}",MessagesHelper.CannotCreateDirectory, ex);
                }
            }

            return path + Path.DirectorySeparatorChar + fileNameCommon;
        }

        private void Run(string fileNameCommon)
        {

            try
            {
                Lexer grammarLexer = new Lexer(FileName);

                Console.WriteLine(MessagesHelper.InfoLexer);

                minijava2Parser parser = new minijava2Parser(grammarLexer.TokenStream);
                //Antlr.Runtime.Tree.CommonTree tree = parser.program().Tree;
                //Console.WriteLine();
                //Console.WriteLine(tree.ToStringTree());
                //Console.WriteLine();
                //parser.Reset();

                BaseSymbol symbol = parser.program().value;

                Console.WriteLine(MessagesHelper.InfoParser);

                //Console.WriteLine(symbol.ToStringTree());

                if (parser.HasError)
                {
                    Console.WriteLine(MessagesHelper.ErrorParser);
                    Console.WriteLine(parser.ErrorMessage);
                    Console.WriteLine(parser.ErrorPosition);
                    HasError = true;
                }
                else
                {
                    //Console.WriteLine(symbol.ToXmlTree());
                    grammarLexer.SaveToFile(string.Format("{0}{1}", fileNameCommon, LexerPostfix));
                    symbol.SaveToFile(string.Format("{0}{1}", fileNameCommon, ParserPostfix));
                    Console.WriteLine(MessagesHelper.InfoCompiler);

                    try
                    {
                        ICompilerLogger logger = new CompilerLogger(string.Format("{0}{1}", fileNameCommon, CodegenPostfix));
                        string exeName = string.Format("{0}.exe", fileNameCommon);
                        SharpCodeGen codeGen = new SharpCodeGen(symbol, exeName, logger);
                        codeGen.GenerateCode();
                        logger.Save();
                        Console.WriteLine("{0}{1}{2}: [{3}]", MessagesHelper.SuccessCompiler, GrammarHelper.NewLineSymbol, MessagesHelper.Output, exeName);
                    }
                    catch (CodeGenerationException ex)
                    {
                        HasError = true;
                        Console.WriteLine("{0}{1}{2}", MessagesHelper.ErrorCompiler, GrammarHelper.NewLineSymbol, ex.CompilerMessage);
                    }

                }
            }
            catch (CompilerException ex)
            {
                Console.WriteLine("{0}{1}{2}", MessagesHelper.ErrorParser, GrammarHelper.NewLineSymbol, ex.CompilerMessage);
            }
        }

        #endregion
    }
}
