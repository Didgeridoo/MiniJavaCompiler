using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CompilerProject;
using CompilerProject.CodeGeneration;

namespace ConsoleAppCompiler
{
    public class TestCode
    {
        #region Fields
        #endregion

        #region Properties
        public string InitDirectory { get; set; }
        public string OutputPath { get; set; }
        #endregion

        #region Constructors
        public TestCode(string initDirectory)
        {
            InitDirectory = initDirectory;
            OutputPath = "tests";
            //RunTest();
        }

        #endregion


        #region Methods



        public void RunTests()
        {
            string [] fileArray = Directory.GetFiles(InitDirectory, "*.java");

            int i = 0;
            foreach (string fileName in fileArray)
            {
                MiniJavaCompiler compiler = new MiniJavaCompiler(fileName);

                Console.WriteLine("BEGIN RUN TEST FOR  [{0}]", fileName);
                compiler.Compile(OutputPath);
                Console.WriteLine("END RUN TEST FOR  [{0}]", fileName);
                Console.WriteLine();
                Console.WriteLine();
                if (!compiler.HasError)
                {
                    i++;
                }
            }
            Console.WriteLine("{0} from {1} completed", i, fileArray.Length);

        }

        #endregion
    }
}
