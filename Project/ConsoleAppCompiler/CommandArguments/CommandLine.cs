using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleAppCompiler.CommandArguments
{
    /// <summary>
    /// –азбор аргументов командной строки
    /// </summary>
    public class CommandLine
    {
        private static char[] EqualSymbol = new char[] {'='};
        private static char[] ArgStartSymbols = new char[] {'-', '\\'};

        /// <summary>
        /// –азбирает аргументы командной строки в CommandArgs
        /// </summary>
        /// <example>
        /// ѕример вида командной строки:
        /// command.exe {([param]) | (-|\argument[=]value)}*
        /// </example>
        /// <param name="args">ћассив аргуметнов командной строки</param>
        /// <returns>–азобранные элементы командной строки</returns>
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

                    // если значение отделено знаком "=" от аргумента
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
                                // если следующий параметр - аргумент, то измен€ем позицию на шаг назад, чтобы на следующем шаге обработать
                                position--;
                            }
                            else if (next != "=")
                            {
                                value = next.TrimStart(EqualSymbol);
                            }
                        }
                    }

                    // пара ключ-значение
                    commandArgs.AddArgPair(arg, value);
                }
                else if (token != string.Empty)
                {
                    // параметр без ключа
                    commandArgs.AddParam(token);
                }

                token = NextToken(args, ref position);
            }

            return commandArgs;
        }

        /// <summary>
        /// явл€етс€ ли строка аргуметном (начинаетс€ ли с '-' или '\')
        /// </summary>
        /// <param name="arg">ѕровер€ема€ строка</param>
        /// <returns>True, если аргумент</returns>
        private static bool IsArgument(string arg)
        {
            return (arg.StartsWith("-") || arg.StartsWith("\\"));
        }

        /// <summary>
        /// —ледующа€ строка в списке аргументов командной строки
        /// </summary>
        /// <param name="args">—писок аргументов командной строки</param>
        /// <param name="position">»ндекс текущей строки</param>
        /// <returns>—ледующа€ строка. ≈сли строки нет, то null</returns>
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
