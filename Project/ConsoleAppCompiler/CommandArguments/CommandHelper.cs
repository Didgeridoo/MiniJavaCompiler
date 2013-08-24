using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CompilerProject;

namespace ConsoleAppCompiler.CommandArguments
{
    /// <summary>
    /// Аргументы командной строки компилятора
    /// </summary>
    public enum CommandArgumentType
    {
        /// <summary>
        /// Выходная директория
        /// </summary>
        OutputDirectory,
        /// <summary>
        /// Входной файл
        /// </summary>
        Input,
    }

    /// <summary>
    /// Вспомогательный класс разбора аргументов командной строки компилятора
    /// </summary>
    public static class CommandHelper
    {
        public static Dictionary<string, CommandArgumentType> ArgumentsDictionary = new Dictionary<string, CommandArgumentType>() 
        {
            {"o", CommandArgumentType.OutputDirectory},
            {"i", CommandArgumentType.Input},
        };

        public static Dictionary<CommandArgumentType, string> ParseCommandArgs(string[] args)
        {
            Dictionary<CommandArgumentType, string> commands = new Dictionary<CommandArgumentType, string>();

            CommandArgs commandArgs = CommandLine.Parse(args);

            // Формируем входные параметры компилятора
            foreach (KeyValuePair<string, string> pair in commandArgs.ArgPairs)
            {
                if (ArgumentsDictionary.ContainsKey(pair.Key))
                {
                    commands.Add(ArgumentsDictionary[pair.Key], pair.Value);
                }
            }

            if (!commands.ContainsKey(CommandArgumentType.OutputDirectory))
            {
                commands.Add(CommandArgumentType.OutputDirectory, string.Empty);
            }


            if (!commands.ContainsKey(CommandArgumentType.Input))
            {
                Console.WriteLine(MessagesHelper.InputFileNotGiven);
            }
            Console.WriteLine("Аргументы командной строки:");
            foreach (KeyValuePair<CommandArgumentType, string> pair in commands)
            {
                Console.WriteLine(" {0} = {1}", pair.Key, pair.Value);
            }

            return commands;
        }

    }
}