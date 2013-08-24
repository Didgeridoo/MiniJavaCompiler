using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompilerProject
{
    public static class MessagesHelper
    {
        public static string CannotCreateDirectory
        {
            get { return "Невозможно создать директорию для записи выходного файла"; }
        }

        public static string AboutProgram
        {
            get { return "MiniJava Compiler. AF (c) 2013"; }
        }

        public static string HelpUsage
        {
            get { return "Использование: -i <входной файл> [-o <выходная директория>]"; }
        }

        public static string Output
        {
            get { return "Выходной файл"; }
        }

        public static string InfoLexer
        {
            get { return "Получение потока токенов..."; }
        }
        
        public static string InfoParser
        {
            get { return "Получение абстрактного синтаксического дерева..."; }
        }

        public static string InfoCompiler
        {
            get { return "Запуск компиляции..."; }
        }

        public static string SuccessCompiler
        {
            get { return "Компиляция завершена: OK."; }
        }

        public static string ErrorParser
        {
            get { return "Ошибка парсера!"; }
        }
        public static string ErrorCompiler
        {
            get { return "Ошибка компиляции!"; }
        }


        #region Exception messages

        public static string NotFoundVariableEx
        {
            get { return "Переменная не объявлена"; }
        }

        public static string TypeMismatchEx
        {
            get { return "Несоответствие типов"; }
        }

        public static string InvalidOperationEx
        {
            get { return "Несоответствие типов бинарной операции"; }
        }

        public static string TokenParseOverflowEx
        {
            get { return "Значение не лежит в границах для типа"; }
        }

        public static string MissingMethodEx
        {
            get { return "Вызываемый метод отсутствует"; }
        }

        public static string LocalVariableIsAlreadyDefinedEx
        {
            get { return "Переменная уже объявлена в этой области видимости"; }
        }

        public static string ClassNotDefined
        {
            get { return "Класс не объявлен"; }
        }

        public static string AssignTypeMismatchEx
        {
            get { return "Несоответствие типов в операции присваивания"; }
        }

        public static string VariableUsingWithoutInitializing
        {
            get { return "Переменная используется без инициализации"; }
        }

        public static string ArrayUsingWithoutInitializingEx
        {
            get { return "Массив используется без инициализации"; }
        }

        public static string IndexCountMismatchEx
        {
            get { return "Несоответсвие размерности массива"; }
        }

        #endregion


        public static string InputFileNotGiven
        {
            get { return "Входной файл не задан"; }
        }

        public static string WrongCommandsArguments 
        {
            get { return "Неверные аргументы командной строки"; }
        }

        public static string UnhandledError 
        {
            get { return "Ошибка"; }
        }

    }
}
