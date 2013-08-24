using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CompilerProject;

namespace ConsoleAppCompiler.CommandArguments
{
    /// <summary>
    /// ��������� ��������� ������ �����������
    /// </summary>
    public enum CommandArgumentType
    {
        /// <summary>
        /// �������� ����������
        /// </summary>
        OutputDirectory,
        /// <summary>
        /// ������� ����
        /// </summary>
        Input,
    }

    /// <summary>
    /// ��������������� ����� ������� ���������� ��������� ������ �����������
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

            // ��������� ������� ��������� �����������
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
            Console.WriteLine("��������� ��������� ������:");
            foreach (KeyValuePair<CommandArgumentType, string> pair in commands)
            {
                Console.WriteLine(" {0} = {1}", pair.Key, pair.Value);
            }

            return commands;
        }

    }
}