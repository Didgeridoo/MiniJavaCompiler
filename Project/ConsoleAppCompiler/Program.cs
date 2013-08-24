using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleAppCompiler.CommandArguments;
using MiniJavaSyntax;
using System.IO;
using System.Linq.Expressions;
using System.Xml.Linq;
using System.Xml;
using CompilerProject;


namespace ConsoleAppCompiler
{
    class Program
    {

        static void Main(string[] args)
        {

            //string fileName = @"test5.txt";
            //string fileName = @"linkedlist.java";
            //string fileName = @"samples\treevisitor.java";
            //string fileName = @"root_calc.txt";
            //Dictionary<CommandArgumentType, string> commands = new Dictionary<CommandArgumentType, string>();
            //commands.Add(CommandArgumentType.Input, fileName);
            //commands.Add(CommandArgumentType.OutputDirectory, string.Empty);
            //commands.Add(CommandArgumentType.OutputDirectory, @"samples\results");

            Dictionary<CommandArgumentType, string> commands = CommandHelper.ParseCommandArgs(args);

            try
            {
                MiniJavaCompiler compiler = new MiniJavaCompiler(commands[CommandArgumentType.Input]);
                compiler.Compile(commands[CommandArgumentType.OutputDirectory]);

            }
            catch (Exception ex)
            {
                Console.WriteLine("{0}: {1}", MessagesHelper.UnhandledError, ex);
                Console.WriteLine("{0}", MessagesHelper.HelpUsage);
            }
        }

    }
}
