using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleAppCompiler.CommandArguments
{
    /// <summary>
    /// ������ ���������� ��������� ������
    /// </summary>
    public class CommandLine
    {
        private static char[] EqualSymbol = new char[] {'='};
        private static char[] ArgStartSymbols = new char[] {'-', '\\'};

        /// <summary>
        /// ��������� ��������� ��������� ������ � CommandArgs
        /// </summary>
        /// <example>
        /// ������ ���� ��������� ������:
        /// command.exe {([param]) | (-|\argument[=]value)}*
        /// </example>
        /// <param name="args">������ ���������� ��������� ������</param>
        /// <returns>����������� �������� ��������� ������</returns>
        public static CommandArgs Parse(string[] args)
        {
            CommandArgs commandArgs = new CommandArgs();
            int position = -1;
            string token = NextToken(args, ref position);
            while (token != null)
            {
                if (IsArgument(token))
                {
                    string arg = token.TrimStart(ArgStartSymbols).TrimEnd(EqualSymbol);
                    string value = null;

                    // ���� �������� �������� ������ "=" �� ���������
                    if (arg.Contains("="))
                    {
                        string[] r = arg.Split(EqualSymbol, 2);
                        if (r.Length == 2 && r[1] != string.Empty)
                        {
                            arg = r[0];
                            value = r[1];
                        }
                    }

                    if (value == null)
                    {
                        string next = NextToken(args, ref position);
                        if (next != null)
                        {
                            if (IsArgument(next))
                            {
                                // ���� ��������� �������� - ��������, �� �������� ������� �� ��� �����, ����� �� ��������� ���� ����������
                                position--;
                            }
                            else if (next != "=")
                            {
                                value = next.TrimStart(EqualSymbol);
                            }
                        }
                    }

                    // ���� ����-��������
                    commandArgs.AddArgPair(arg, value);
                }
                else if (token != string.Empty)
                {
                    // �������� ��� �����
                    commandArgs.AddParam(token);
                }

                token = NextToken(args, ref position);
            }

            return commandArgs;
        }

        /// <summary>
        /// �������� �� ������ ���������� (���������� �� � '-' ��� '\')
        /// </summary>
        /// <param name="arg">����������� ������</param>
        /// <returns>True, ���� ��������</returns>
        private static bool IsArgument(string arg)
        {
            return (arg.StartsWith("-") || arg.StartsWith("\\"));
        }

        /// <summary>
        /// ��������� ������ � ������ ���������� ��������� ������
        /// </summary>
        /// <param name="args">������ ���������� ��������� ������</param>
        /// <param name="position">������ ������� ������</param>
        /// <returns>��������� ������. ���� ������ ���, �� null</returns>
        private static string NextToken(string[] args, ref int position)
        {
            string next = null;
            while (++position < args.Length && next == null)
            {
                string cur = args[position].Trim();
                if (cur != string.Empty)
                {
                    next = cur;
                    position--;
                }
            }


            return next;
        }

    }
}
