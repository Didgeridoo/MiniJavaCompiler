using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CompilerProject.GrammarElements;
using TriAxis.RunSharp;
using System.Reflection.Emit;
using System.Reflection;
using CompilerProject.Variables;
using CompilerProject;
using CompilerProject.Exceptions;
using CompilerProject.Logs;

namespace CompilerProject.CodeGeneration
{

    /// <summary>
    /// Делегат на управляющую фукцию
    /// </summary>
    /// <param name="nonTerm"></param>
    public delegate void EmitMethodDelegate(NonTerm nonTerm);


    /// <summary>
    /// Генератор IL-кода
    /// </summary>
    public class SharpCodeGen
    {
        #region Fields

        private readonly BaseSymbol _root;
        private readonly string _moduleName;
        private ICompilerLogger _compilerLogger;

        /// <summary>
        /// список классов сборки
        /// </summary>
        private Dictionary<string, TypeGen> _classesTable;
        /// <summary>
        /// список методов классов сборки
        /// </summary>
        private Dictionary<string, Dictionary<string, MethodGen>> _methodsTables;

        /// <summary>
        /// список локальных переменных
        /// </summary>
        private Dictionary<string, Operand> _localVariablesTable;

        /// <summary>
        /// текущий класс
        /// </summary>
        private TypeGen _currentClass;
        
        /// <summary>
        /// текущий метод
        /// </summary>
        private MethodGen _currentMethod;
        
        /// <summary>
        /// текущий операнд
        /// </summary>
        private Operand _currentOperand;
        private Operand _currentOperandTempResult;        
        
        /// <summary>
        /// генерация кода
        /// </summary>
        private CodeGen _g;
        /// <summary>
        /// генерируемая сборка
        /// </summary>
        private AssemblyGen _asm;

        /// <summary>
        /// текущий список формальных переменных
        /// </summary>
        private List<string> _currentFormalArgumentList;

        /// <summary>
        /// текущие локальные переменные блока (для метода, if, while)
        /// </summary>
        private List<string> _currentBlockVariableList;
        /// <summary>
        /// стек всех локальных переменных блоков
        /// </summary>
        private Stack<List<string>> _currentBlockVariablesStack;

        /// <summary>
        /// стеки для бинарных операций
        /// </summary>
        private Stack<Operand> _stackOperandFirst;
        private Stack<Operand> _stackOperandSecond;

        /// <summary>
        /// временные переменные
        /// </summary>
        private IfVariableNameGenerator _ifVariableNameGenerator;
        private WhileVariableNameGenerator _whileVariableNameGenerator;
        private TempVariableNameGenerator _tempVariableNameGenerator;

        /// <summary>
        /// управляющая таблица генерации кода
        /// </summary>
        private Dictionary<NonTermType, EmitMethodDelegate> _emitTableDictionary;

        #endregion

        #region Constructors
        public SharpCodeGen(BaseSymbol root, string moduleName, ICompilerLogger compilerLogger)
        {
            _root = root;
            _moduleName = moduleName;
            _compilerLogger = compilerLogger;

            _classesTable = new Dictionary<string, TypeGen>();
            _localVariablesTable = new Dictionary<string, Operand>();
            _methodsTables = new Dictionary<string, Dictionary<string, MethodGen>>();

            // инициализация управляющей таблицы кодогенерации
            _emitTableDictionary = new Dictionary<NonTermType, EmitMethodDelegate>() {
                {NonTermType.Class, EmitClass},
                {NonTermType.MainClass, EmitMainClass},
                {NonTermType.ClassVarDecl, EmitClassVar},
                {NonTermType.Method, EmitMethod},
                {NonTermType.VarStatement, EmitVarStatement},
                {NonTermType.IdStatement, EmitIdStatement},
                {NonTermType.NewStatement, EmitNewStatement},
                {NonTermType.NewArrayStatement, EmitNewArrayStatement},
                {NonTermType.ArrayIdStatement, EmitArrayIdStatement},
                {NonTermType.ArrayIndiciesStatement, EmitArrayIndiciesStatement},

                {NonTermType.MethodCallExpression, EmitMethodCallExpression}, 
                {NonTermType.MethodThisCallExpression, EmitMethodCallExpression}, 

                //{NonTermType.BinaryExpression, EmitBinaryExpression},
                {NonTermType.LogicalOrExpression, EmitBinaryExpression},
                {NonTermType.LogicalAndExpression, EmitBinaryExpression},

                {NonTermType.EqualExpression, EmitBinaryExpression},
                {NonTermType.NotEqualExpression, EmitBinaryExpression},
                {NonTermType.LessExpression, EmitBinaryExpression},
                {NonTermType.LessEqExpression, EmitBinaryExpression},
                {NonTermType.MoreExpression, EmitBinaryExpression},
                {NonTermType.MoreEqExpression, EmitBinaryExpression},

                {NonTermType.PlusExpression, EmitBinaryExpression},
                {NonTermType.MinusExpression, EmitBinaryExpression},
                {NonTermType.MultiplyExpression, EmitBinaryExpression},
                {NonTermType.DivisionExpression, EmitBinaryExpression},

                //{NonTermType.UnaryExpression, EmitUnaryExpression},
                {NonTermType.UnaryMinusExpression, EmitUnaryExpression},
                {NonTermType.UnaryNotExpression, EmitUnaryExpression},

                {NonTermType.PrintStatement, EmitPrintStatement},

                {NonTermType.IfStatement, EmitIfStatement},
                {NonTermType.IfElseStatement, EmitIfStatement},

                {NonTermType.WhileStatement, EmitWhileStatement},
                {NonTermType.LengthFunctionExpression, EmitLengthFunctionExpression},
            };

            _currentFormalArgumentList = new List<string>();
            _currentBlockVariableList = new List<string>();
            _currentBlockVariablesStack = new Stack<List<string>>();

            _stackOperandFirst = new Stack<Operand>();
            _stackOperandSecond = new Stack<Operand>();

            _ifVariableNameGenerator = new IfVariableNameGenerator();
            _whileVariableNameGenerator = new WhileVariableNameGenerator();
            _tempVariableNameGenerator = new TempVariableNameGenerator();


        }

        #endregion

        #region Methods

        public void GenerateCode()
        {
            _asm = new AssemblyGen(_moduleName);
            //LoadClasses(_root, _asm);
            LoadClassesExtends(_root, _asm);
            LoadClassesMethods(_root, _asm);
            _compilerLogger.PrintClassesMethods(_methodsTables);

            Generate(_root);

            _asm.Save();
        }

        #region Init methods for preparing to compiling

        private void LoadClasses(BaseSymbol root, AssemblyGen ag)
        {
            root.Symbols.ForEach(s =>
                {
                    if (s.GrammarMember == GrammarMemberType.NonTerm)
                    {
                        NonTerm nonTerm = s as NonTerm;
                        switch (nonTerm.TypeNonTerm)
                        {
                            case NonTermType.Class:
                                Token termClassId = nonTerm.Symbols[0] as Token;
                                string classId = termClassId.Value;
                                TypeGen cl = ag.Class(classId);
                                _classesTable.Add(cl.Name, cl);
                                break;
                        }
                    }
                });

        }


        /// <summary>
        /// Загрузка всех классов.
        /// Допущение - классы-наследники должны располагаться после базовых классов 
        /// и иерархий наследования должно быть не более 1.
        /// </summary>
        /// <param name="root">Базовый символ</param>
        /// <param name="ag">Сборка</param>
        private void LoadClassesExtends(BaseSymbol root, AssemblyGen ag)
        {
            root.Symbols.ForEach(s =>
                {
                    if (s.GrammarMember == GrammarMemberType.NonTerm)
                    {
                        NonTerm nonTerm = s as NonTerm;
                        switch (nonTerm.TypeNonTerm)
                        {
                            case NonTermType.Class:
                                Token termClassId; //= nonTerm.Symbols[0] as Token;
                                NonTerm extends; //= nonTerm.Symbols[1] as NonTerm;
                                NonTermFactory.GetClassDecl(nonTerm, out termClassId, out extends);
                                string classId = termClassId.Value;
                                if (extends == null)
                                {
                                    TypeGen cl = ag.Class(classId);
                                    _classesTable.Add(cl.Name, cl);
                                }
                                else
                                {
                                    string baseClassId = extends.Symbols[0].Value;
                                    TypeGen cl = _classesTable[baseClassId];
                                    ///cl = _classesTable[classId].Class(classId, cl);
                                    cl = ag.Class(classId, cl);
                                    ///_classesTable[cl.Name] = cl;
                                    _classesTable.Add(cl.Name, cl);
                                }

                                break;
                        }
                    }
                });

        }


        private void LoadClassesMethods(BaseSymbol root, AssemblyGen ag)
        {
            root.Symbols.ForEach(s =>
                {
                    if (s.GrammarMember == GrammarMemberType.NonTerm)
                    {
                        NonTerm nonTerm = s as NonTerm;
                        switch (nonTerm.TypeNonTerm)
                        {
                            case NonTermType.Class:
                                Token termClassId = nonTerm.Symbols[0] as Token;
                                string classId = termClassId.Value;

                                TypeGen cl = _classesTable[classId];
                                Dictionary<string, MethodGen> methodsDictionary = new Dictionary<string, MethodGen>();

                                for (int i = 2; i < nonTerm.Symbols.Count; i++)
                                {
                                    if ((nonTerm.Symbols[i] as NonTerm).TypeNonTerm == NonTermType.Method)
                                    {
                                        KeyValuePair<string, MethodGen> method = GenerateMethodSignature(cl, nonTerm.Symbols[i], ag);
                                        methodsDictionary.Add(method.Key, method.Value);
                                    }
                                }

                                _methodsTables.Add(cl.Name, methodsDictionary);

                                break;
                        }
                    }
                });

        }

        private KeyValuePair<string, MethodGen> GenerateMethodSignature(TypeGen classDeclaration, BaseSymbol nonTerm, AssemblyGen ag)
        {
            BaseSymbol typeMethodDecl = nonTerm.Symbols[0];
            Token typeMethodDeclSimple = typeMethodDecl as Token;

            Token methodName = nonTerm.Symbols[1] as Token;
            IEnumerable<BaseSymbol> formalParametersList = nonTerm.Symbols[2].Symbols;

            Type methodReturnType = GetVariableType(typeMethodDeclSimple);
            MethodGen method = classDeclaration.Public.Method(methodReturnType, methodName.Value);

            foreach (BaseSymbol symbol in formalParametersList)
            {
                Token type = symbol.Symbols[0] as Token;
                Token id = symbol.Symbols[1] as Token;
                method.Parameter(GetVariableType(type), id.Value);
            }

            //_compilerLogger.PrintAddtFormalArgumentList(formalParametersList.Select(s => s.Symbols[1].Value).ToList());


            return new KeyValuePair<string, MethodGen>(methodName.Value, method);
        }

        #endregion

        /// <summary>
        /// Главный метод генерации IL-кода
        /// </summary>
        /// <param name="root">символ грамматики</param>
        private void Generate(BaseSymbol root)
        {
            if (root == null)
            {
                return;
            }

            if (root.GrammarMember == GrammarMemberType.NonTerm)
            {
                NonTerm nonTerm = root as NonTerm;
                _compilerLogger.PrintGenerateNonTerm(nonTerm);

                if (_emitTableDictionary.ContainsKey(nonTerm.TypeNonTerm))
                {
                    _emitTableDictionary[nonTerm.TypeNonTerm](nonTerm);
                }
                else
                {
                    root.Symbols.ForEach(Generate);
                }
            }
            
        }


        private Type GetVariableType(Token token)
        {
            Type type;
            if (token.TypeToken != TokenType.ID)
            {
                type = token.TypeOf;
            }
            else
            {
                if (_classesTable.ContainsKey(token.Value))
                {
                    type = _classesTable[token.Value];
                }
                else
                {
                    throw new CodeGenerationException(MessagesHelper.ClassNotDefined, token.ToStringInfo(), null);
                }
            }
            return type;
        }

        private Operand GetOperandValue(string token, Token tokenForExceptionInfo)
        {
            Operand operand = null;

            if (_currentMethod != null && _currentFormalArgumentList.Contains(token))
            {
                operand = _g.Arg(token);
            }
            else if (_localVariablesTable.ContainsKey(token))
            {
                operand = _localVariablesTable[token];
            }
            else if (!token.Equals(GrammarHelper.This))
            {
                try
                {
                    operand = _g.This().Field(token);
                }
                catch (Exception ex)
                {
                    throw new CodeGenerationException(MessagesHelper.NotFoundVariableEx, 
                        tokenForExceptionInfo != null ? tokenForExceptionInfo.ToStringInfo() : token, ex);
                }
            }
            else
            {
                operand = _g.This();
            }

            return operand;
        }


        #region Temp local variables methods

        private string AddTempLocalVariable()
        {
            return AddLocalVariable(_tempVariableNameGenerator, typeof(object));
        }

        private string AddTempLocalVariable(Type type)
        {
            return AddLocalVariable(_tempVariableNameGenerator, type);
        }

        private string AddArrayLocalVariable()
        {
            return AddLocalVariable(_tempVariableNameGenerator, typeof(int));
        }

        private string AddIfLocalVariable()
        {
            return AddLocalVariable(_ifVariableNameGenerator, typeof(bool));
        }

        private string AddWhileLocalVariable()
        {
            return AddLocalVariable(_whileVariableNameGenerator, typeof(bool));
        }

        private string AddLocalVariable(DummyVariable incrementor, Type type)
        {
            string localName = incrementor.GetNext();
            _currentOperand = _g.Local(type);
            _localVariablesTable.Add(localName, _currentOperand);
            return localName;
        }

        #endregion

        private Operand OperandTokenInit(BaseSymbol first, Operand operand)
        {
            Token token = first as Token;
            switch (token.TypeToken)
            {
                case TokenType.INT:
                    operand = (int)token.ValueObject;
                    break;
                case TokenType.INTEGER:
                    operand = (int)token.ValueObject;
                    break;
                case TokenType.FLOAT:
                    operand = (double)token.ValueObject;
                    break;
                case TokenType.BOOL:
                    operand = (bool)token.ValueObject;
                    break;
                case TokenType.TRUE:
                    operand = true;
                    break;
                case TokenType.FALSE:
                    operand = false;
                    break;
                case TokenType.NULL:
                    operand = null;
                    break;
                case TokenType.ID:
                    operand = GetOperandValue(token.Value, token);
                    break;
                default:
                    //operand = _g.Local(token.TypeOf, token.Value);
                    _g.Assign(operand, token.Value);
                    break;
            }
            return operand;
        }

        private Operand OperandTokenInitLocal(BaseSymbol first)
        {
            return OperandTokenInit(first, null);
        }


        #region Local variables for block

        private void AddPreInitLocalVariables(BaseSymbol root)
        {
            if (root == null)
            {
                return;
            }

            if (root.GrammarMember == GrammarMemberType.NonTerm)
            {
                NonTerm nonTerm = root as NonTerm;
                // заносим в список переменные только текущего блока
                if (GrammarHelper.BlockVariablesList.Contains(nonTerm.TypeNonTerm))
                {
                    return;
                }
                switch (nonTerm.TypeNonTerm)
                {
                    case NonTermType.Variable:
                    case NonTermType.ArrayVariable:

                        BaseSymbol typeDecl = nonTerm.Symbols[0];
                        Token typeDeclSimple = typeDecl as Token;

                        Token idVar = nonTerm.Symbols[1] as Token;

                        _currentOperand = _g.Local(GetVariableType(typeDeclSimple));

                        try
                        {
                            _localVariablesTable.Add(idVar.Value, _currentOperand);
                            _currentBlockVariableList.Add(idVar.Value);
                        }
                        catch (ArgumentException ex)
                        {
                            throw new CodeGenerationException(MessagesHelper.LocalVariableIsAlreadyDefinedEx, idVar.ToStringInfo(), ex);
                        }
                        break;
                }
            }

            if (root.GrammarMember == GrammarMemberType.NonTerm)
            {
                root.Symbols.ForEach(x => AddPreInitLocalVariables(x));
            }
        }


        private void ClearCurrentBlockLocalVariables()
        {
            if (_currentBlockVariablesStack.Count > 0)
            {
                _currentBlockVariableList = _currentBlockVariablesStack.Pop();
                _currentBlockVariableList.ForEach(v => _localVariablesTable.Remove(v));
                _compilerLogger.PrintDeleteLocalVariables(_currentBlockVariableList);
                _currentBlockVariableList.Clear();
            }
        }

        #endregion

        private void GeneratePreInitLocalVariables(BaseSymbol root)
        {
            _currentBlockVariableList.Clear();
            AddPreInitLocalVariables(root);
            _currentBlockVariablesStack.Push(new List<string>(_currentBlockVariableList));
            _compilerLogger.PrintAddLocalVariables(_currentBlockVariableList);
        }

        /// <summary>
        /// Вычисление параметров индекса массива
        /// </summary>
        /// <param name="nonTerm"></param>
        /// <param name="initIndex">начальный индекс nonTerm.Symbols, с которого будут перебираться параметры</param>
        /// <returns></returns>
        private List<Operand> GenerateArrayIndiciesExpression(NonTerm nonTerm, int initIndex)
        {
            List<Operand> arrayIndicesExpressions = new List<Operand>();

            for (int i = initIndex; i < nonTerm.Symbols.Count; i++)
            {
                BaseSymbol expressionSymbol = nonTerm.Symbols[i];
                string nameArrayIndexTempVariable = AddArrayLocalVariable();
                Operand expr = EmitExpression(expressionSymbol, typeof(int), nameArrayIndexTempVariable);
                arrayIndicesExpressions.Add(expr);
            }

            return arrayIndicesExpressions;
        }



        #region Emit methods


        private Operand EmitExpression(BaseSymbol symbol, Type type, string name)
        {
            if (!(symbol is Token))
            {
                //_compilerLogger.PrintGenerateNonTerm(symbol as NonTerm);
                _currentOperandTempResult = _g.Local(type);
                Generate(symbol);
            }
            else
            {
                _currentOperandTempResult = OperandTokenInitLocal(symbol);
            }
            return _currentOperandTempResult;
        }

        private Operand EmitBoolExpression(BaseSymbol symbol, string name)
        {
            if (!(symbol is Token))
            {
                _currentOperandTempResult = _g.Local(typeof(bool));
                Generate(symbol);
            }
            else
            {
                _currentOperandTempResult = OperandTokenInit(symbol, _localVariablesTable[name]);
            }
            return _currentOperandTempResult;
        }

        private void EmitClass(NonTerm nonTerm)
        {
            Token termClassId;
            List<BaseSymbol> declarations;
            NonTermFactory.GetClassDecl(nonTerm, out termClassId, out declarations);
            _currentClass = _classesTable[termClassId.Value];

            declarations.ForEach(symbol => Generate(symbol));
        }

        private void EmitMainClass(NonTerm nonTerm)
        {
            Token termClassId;
            List<BaseSymbol> declarations;
            NonTermFactory.GetMainClassDecl(nonTerm, out termClassId, out declarations);

            _currentClass = _asm.Class(termClassId.Value);
            _classesTable.Add(_currentClass.Name, _currentClass);

            _g = _currentClass.Public.Static.Method(typeof(void), "Main");

            GeneratePreInitLocalVariables(nonTerm);

            declarations.ForEach(symbol => Generate(symbol));

            ClearCurrentBlockLocalVariables();

        }

        private void EmitClassVar(NonTerm nonTerm)
        {
            Token typeClassVarDeclSimple;
            Token idClassVar;
            NonTermFactory.GetClassVarDecl(nonTerm, out typeClassVarDeclSimple, out idClassVar);

            Type classVarType = GetVariableType(typeClassVarDeclSimple);

            _currentClass.Private.Field(classVarType, idClassVar.Value);

        }

        private void EmitMethod(NonTerm nonTerm)
        {
            Token typeMethodDeclSimple;
            Token methodName;
            List<BaseSymbol> formalParametersList;
            BaseSymbol methodStatementList;
            BaseSymbol returnStatement;
            NonTermFactory.GetMethodDecl(nonTerm, out typeMethodDeclSimple, out methodName, 
                out formalParametersList, out methodStatementList, out returnStatement);

            _currentFormalArgumentList.Clear();
            foreach (BaseSymbol symbol in formalParametersList)
            {
                Token type;
                Token id;
                NonTermFactory.GetFormalArgumentDeclaration(symbol, out type, out id);
                _currentFormalArgumentList.Add(id.Value);
            }

            _compilerLogger.PrintRefreshFormalArgumentList(_currentFormalArgumentList);

            _currentMethod = _methodsTables[_currentClass.Name][methodName.Value];
            _g = _currentMethod;

            GeneratePreInitLocalVariables(methodStatementList);

            Generate(methodStatementList);
            Type resultType = GetVariableType(typeMethodDeclSimple);

            string nameResult = AddTempLocalVariable(resultType);
            EmitExpression(returnStatement, resultType, nameResult);

            try
            {
                _g.Return(_currentOperandTempResult);
            }
            catch (InvalidCastException ex)
            {
                throw new CodeGenerationException(MessagesHelper.TypeMismatchEx, returnStatement.ToStringInfo(), ex);
            }

            ClearCurrentBlockLocalVariables();

        }


        private void EmitVarStatement(NonTerm nonTerm)
        {
            BaseSymbol varStat;
            BaseSymbol expression;
            NonTermFactory.GetAssignVarStatement(nonTerm, out varStat, out expression);

            if (expression != null)
            {
                Token varType;
                Token varToken;
                NonTermFactory.GetVarDecl(varStat, out varType, out varToken);

                if (varType.TypeToken == TokenType.ID)
                {
                    _currentOperand = _g.Local(Exp.New(_classesTable[varType.Value]));
                    _currentOperandTempResult = _g.Local(_currentOperand);
                }
                else
                {
                    _currentOperand = _g.Local(varType.TypeOf);
                    _currentOperandTempResult = _g.Local(_currentOperand.Type);

                }

                if (!(expression is Token))
                {
                    Generate(expression);
                    _g.Assign(GetOperandValue(varToken.Value, varToken), _currentOperandTempResult);

                }
                else
                {
                    _g.Assign(GetOperandValue(varToken.Value, varToken), OperandTokenInit(expression, GetOperandValue(varToken.Value, varToken)));
                }
            }

        }


        private void EmitNewStatement(NonTerm nonTerm)
        {
            Token typeToken;
            NonTermFactory.GetNewStatement(nonTerm, out typeToken);
            _currentOperandTempResult = Exp.New(_classesTable[typeToken.Value]);

        }


        private void EmitNewArrayStatement(NonTerm nonTerm)
        {
            Token arrayTypeToken;
            List<BaseSymbol> bracketExpressionList;
            NonTermFactory.GetNewArrayStatement(nonTerm, out arrayTypeToken, out bracketExpressionList);

            Operand tempOperand = _currentOperandTempResult;

            List<Operand> arrayIndicesExpressions = GenerateArrayIndiciesExpression(nonTerm, 1);

            _currentOperandTempResult = tempOperand;
            try
            {
                _g.Assign(_currentOperandTempResult,
                    Exp.NewArray(arrayTypeToken.TypeOfWithoutArray, arrayIndicesExpressions.ToArray()));
            }
            catch (InvalidOperationException ex)
            {
                throw new CodeGenerationException(MessagesHelper.VariableUsingWithoutInitializing, arrayTypeToken.ToStringInfo(), ex);
            }

        }

        private void EmitArrayIdStatement(NonTerm nonTerm)
        {
            Token arrayIdToken;
            BaseSymbol expression;
            List<BaseSymbol> bracketExpressionList;
            NonTermFactory.GetArrayAssignIdStatement(nonTerm, out arrayIdToken, out expression, out bracketExpressionList);

            List<Operand> arrayIndicesExpressions = GenerateArrayIndiciesExpression(nonTerm, 2);

            _currentOperandTempResult = EmitExpression(expression,
                GetOperandValue(arrayIdToken.Value, arrayIdToken)[arrayIndicesExpressions.ToArray()].Type, arrayIdToken.Value);

            try
            {
                _g.Assign(GetOperandValue(arrayIdToken.Value, arrayIdToken)[arrayIndicesExpressions.ToArray()], _currentOperandTempResult);
            }
            catch (InvalidOperationException ex)
            {
                throw new CodeGenerationException(MessagesHelper.ArrayUsingWithoutInitializingEx, arrayIdToken.ToStringInfo(), ex);
            }

        }


        private void EmitIdStatement(NonTerm nonTerm)
        {
            Token idToken;
            BaseSymbol expression;
            NonTermFactory.GetAssignIdStatement(nonTerm, out idToken, out expression);

            Operand operand;
            if (_currentFormalArgumentList.Contains(idToken.Value))
            {
                operand = _g.Arg(idToken.Value);
            }
            else if (_localVariablesTable.ContainsKey(idToken.Value))
            {
                operand = _localVariablesTable[idToken.Value];
            }
            else
            {
                operand = _g.This().Field(idToken.Value);
            }
            _currentOperandTempResult = EmitExpression(expression, operand.Type, idToken.Value);
            
            try
            {
                _g.Assign(operand, _currentOperandTempResult);
            }
            catch (InvalidCastException ex)
            {
                throw new CodeGenerationException(MessagesHelper.AssignTypeMismatchEx, expression.ToStringInfo(), ex);
            }
        }


        private void EmitArrayIndiciesStatement(NonTerm nonTerm)
        {
            Token arrayNameToken;
            List<BaseSymbol> expressionSymbolList;
            NonTermFactory.GetArrayIndiciesStatement(nonTerm, out arrayNameToken, out expressionSymbolList);

            List<Operand> arrayIndicesBracketsExpressions = new List<Operand>();
            foreach (var expressionSymbol in expressionSymbolList)
            {
                string nameArrayIndexTempVariable = AddArrayLocalVariable();
                Operand expr = EmitExpression(expressionSymbol, typeof(int), nameArrayIndexTempVariable);
                arrayIndicesBracketsExpressions.Add(expr);                
            }

            try
            {
                if (_localVariablesTable.ContainsKey(arrayNameToken.Value))
                {
                    _currentOperandTempResult = _localVariablesTable[arrayNameToken.Value][arrayIndicesBracketsExpressions.ToArray()];
                }
                else
                {
                    _currentOperandTempResult = _g.This().Field(arrayNameToken.Value)[arrayIndicesBracketsExpressions.ToArray()];
                }
            }
            catch (ArgumentException ex)
            {
                throw new CodeGenerationException(MessagesHelper.IndexCountMismatchEx, arrayNameToken.ToStringInfo(), ex);
            }
        }


        private void EmitMethodCallExpression(NonTerm nonTerm)
        {
            Token idVariable = null;
            Token idMethodName = null;
            BaseSymbol methodArguments = null;
            string nameIdVariable = string.Empty;
            // BaseSymbol arrayIndicesList = nonTerm.Symbols[1];

            if (nonTerm.TypeNonTerm == NonTermType.MethodCallExpression)
            {
                if (nonTerm.Symbols[0] is Token)
                {
                    idVariable = nonTerm.Symbols[0] as Token;
                    nameIdVariable = idVariable.Value;
                }
                else
                {
                    NonTerm nonTermVariable = nonTerm.Symbols[0] as NonTerm;
                    Generate(nonTermVariable);
                    string nameNew = AddTempLocalVariable(_currentOperandTempResult.Type);
                    _localVariablesTable[nameNew] = _currentOperandTempResult;
                    nameIdVariable = nameNew;
                }
                idMethodName = nonTerm.Symbols[1] as Token;
                methodArguments = nonTerm.Symbols[2];
            }
            else
            {
                idMethodName = nonTerm.Symbols[0] as Token;
                methodArguments = nonTerm.Symbols[1];
            }

            List<Operand> methodArgumentsExpressions = new List<Operand>();

            for (int i = 0; i < methodArguments.Symbols.Count; i++)
            {
                BaseSymbol expressionSymbol = methodArguments.Symbols[i];
                string nameTempVariable = AddTempLocalVariable();
                Operand expr = EmitExpression(expressionSymbol, typeof(object), nameTempVariable);
                methodArgumentsExpressions.Add(expr);
            }

            try
            {
                if (nonTerm.TypeNonTerm == NonTermType.MethodCallExpression)
                {
                    _currentOperandTempResult = GetOperandValue(nameIdVariable, null).Invoke(idMethodName.Value, methodArgumentsExpressions.ToArray());
                }
                else
                {
                    _currentOperandTempResult = _g.This().Invoke(idMethodName.Value, methodArgumentsExpressions.ToArray());
                }
            }
            catch (MissingMethodException ex)
            {
                throw new CodeGenerationException(MessagesHelper.MissingMethodEx, idMethodName.ToStringInfo(), ex);
            }

        }


        private void EmitBinaryExpression(NonTerm nonTerm)
        {
            BaseSymbol first;
            BaseSymbol second;
            NonTermFactory.GetBinaryExpression(nonTerm, out first, out second);

            //_compilerLogger.PrintGenerateNonTerm(nonTerm);
            try
            {

                if (first.GrammarMember == GrammarMemberType.Token)
                {
                    _stackOperandFirst.Push(OperandTokenInitLocal(first));
                }
                else
                {
                    Generate(first);
                    _stackOperandFirst.Push(_g.Local(_currentOperandTempResult));
                }
            }
            catch (InvalidCastException ex)
            {
                throw new CodeGenerationException(MessagesHelper.TypeMismatchEx, first.ToStringInfo(), ex);
            }


            try
            {
                if (second.GrammarMember == GrammarMemberType.Token)
                {
                    _stackOperandSecond.Push(OperandTokenInitLocal(second));
                }
                else
                {
                    Generate(second);
                    _stackOperandSecond.Push(_g.Local(_currentOperandTempResult));
                }
            }
            catch (InvalidCastException ex)
            {
                throw new CodeGenerationException(MessagesHelper.TypeMismatchEx, second.ToStringInfo(), ex);
            }

            Operand currentOperandFirst = _stackOperandFirst.Pop();
            Operand currentOperandSecond = _stackOperandSecond.Pop();

            try
            {

                switch (nonTerm.TypeNonTerm)
                {
                    case NonTermType.EqualExpression:
                        _currentOperandTempResult = currentOperandFirst == currentOperandSecond;
                        break;
                    case NonTermType.NotEqualExpression:
                        _currentOperandTempResult = currentOperandFirst != currentOperandSecond;
                        break;

                    case NonTermType.LessExpression:
                        _currentOperandTempResult = currentOperandFirst < currentOperandSecond;
                        break;
                    case NonTermType.LessEqExpression:
                        _currentOperandTempResult = currentOperandFirst <= currentOperandSecond;
                        break;
                    case NonTermType.MoreExpression:
                        _currentOperandTempResult = currentOperandFirst > currentOperandSecond;
                        break;
                    case NonTermType.MoreEqExpression:
                        _currentOperandTempResult = currentOperandFirst >= currentOperandSecond;
                        break;

                    case NonTermType.LogicalAndExpression:
                        _currentOperandTempResult = currentOperandFirst && currentOperandSecond;
                        break;
                    case NonTermType.LogicalOrExpression:
                        _currentOperandTempResult = currentOperandFirst || currentOperandSecond;
                        break;

                    case NonTermType.PlusExpression:
                        _currentOperandTempResult = currentOperandFirst + currentOperandSecond;
                        break;
                    case NonTermType.MinusExpression:
                        _currentOperandTempResult = currentOperandFirst - currentOperandSecond;
                        break;

                    case NonTermType.MultiplyExpression:
                        _currentOperandTempResult = currentOperandFirst * currentOperandSecond;
                        break;
                    case NonTermType.DivisionExpression:
                        _currentOperandTempResult = currentOperandFirst / currentOperandSecond;
                        break;
                }

            }
            catch (InvalidOperationException ex)
            {
                throw new CodeGenerationException(MessagesHelper.InvalidOperationEx, nonTerm.ToStringInfo(), ex);
            }
            catch (InvalidCastException ex)
            {
                throw new CodeGenerationException(MessagesHelper.TypeMismatchEx, nonTerm.ToStringInfo(), ex);
            }
        }


        private void EmitUnaryExpression(NonTerm nonTerm)
        {
            BaseSymbol unaryExpression;
            NonTermFactory.GetUnaryExpression(nonTerm, out unaryExpression);

            //_compilerLogger.PrintGenerateNonTerm(nonTerm);

            try
            {
                if (unaryExpression.GrammarMember == GrammarMemberType.Token)
                {
                    Operand operandFirst = OperandTokenInitLocal(unaryExpression);
                    _stackOperandFirst.Push(_g.Local(operandFirst));
                }
                else
                {
                    Generate(unaryExpression);
                    _stackOperandFirst.Push(_g.Local(_currentOperandTempResult));
                }
            }
            catch (InvalidCastException ex)
            {
                throw new CodeGenerationException(MessagesHelper.TypeMismatchEx, unaryExpression.ToStringInfo(), ex);
            }

            Operand currentOperandFirst = _stackOperandFirst.Pop();

            try
            {
                Operand unaryExpr = null;
                switch (nonTerm.TypeNonTerm)
                {
                    case NonTermType.UnaryMinusExpression:
                        unaryExpr = -currentOperandFirst;
                        break;
                    case NonTermType.UnaryNotExpression:
                        unaryExpr = !currentOperandFirst;
                        break;
                }

                _currentOperandTempResult = unaryExpr;
           }
           catch (InvalidOperationException ex)
           {
               throw new CodeGenerationException(MessagesHelper.InvalidOperationEx, nonTerm.ToStringInfo(), ex);
           }
           catch (InvalidCastException ex)
           {
               throw new CodeGenerationException(MessagesHelper.TypeMismatchEx, nonTerm.ToStringInfo(), ex);
           }

        }

        private void EmitPrintStatement(NonTerm nonTerm)
        {
            BaseSymbol expressionPrint;
            NonTermFactory.GetPrintStatement(nonTerm, out expressionPrint);

            string namePrint = AddTempLocalVariable();

            if (!(expressionPrint is Token))
            {
                _currentOperandTempResult = _g.Local(_localVariablesTable[namePrint].Type);
                Generate(expressionPrint);
            }
            else
            {
                _currentOperandTempResult = OperandTokenInit(expressionPrint, _localVariablesTable[namePrint]);
            }

            _g.WriteLine(_currentOperandTempResult);

            _g.Invoke(typeof(Console), "ReadKey");

        }

        private void EmitIfStatement(NonTerm nonTerm)
        {
            BaseSymbol expressionIf;
            BaseSymbol statementIf;
            NonTermFactory.GetIfStatement(nonTerm, out expressionIf, out statementIf);


            string nameIf = AddIfLocalVariable();

            _currentOperandTempResult = EmitExpression(expressionIf, _localVariablesTable[nameIf].Type, nameIf);

            _g.Assign(_localVariablesTable[nameIf], _currentOperandTempResult);

            try
            {
                _g.If(_currentOperandTempResult);
            }
            catch (InvalidOperationException ex)
            {
                throw new CodeGenerationException(MessagesHelper.TypeMismatchEx, expressionIf.ToStringInfo(), ex);
            }

            GeneratePreInitLocalVariables(statementIf);
            Generate(statementIf);
            ClearCurrentBlockLocalVariables();

            if (nonTerm.TypeNonTerm == NonTermType.IfElseStatement)
            {
                BaseSymbol statementElse;
                NonTermFactory.GetElseStatement(nonTerm, out statementElse);

                _g.Else();
                GeneratePreInitLocalVariables(statementElse);
                Generate(statementElse);
                ClearCurrentBlockLocalVariables();
            }

            _g.End();
        }

        private void EmitWhileStatement(NonTerm nonTerm)
        {
            BaseSymbol expressionWhile;
            BaseSymbol statementWhile;
            NonTermFactory.GetWhileStatement(nonTerm, out expressionWhile, out statementWhile);

            string nameWhile = AddWhileLocalVariable();

            GeneratePreInitLocalVariables(statementWhile);

            _g.Assign(_localVariablesTable[nameWhile], EmitBoolExpression(expressionWhile, nameWhile));
            try
            {
                _g.While(_localVariablesTable[nameWhile]);
            }
            catch (InvalidOperationException ex)
            {
                throw new CodeGenerationException(MessagesHelper.TypeMismatchEx, expressionWhile.ToStringInfo(), ex);
            }

            Generate(statementWhile);

            _g.Assign(_localVariablesTable[nameWhile], EmitBoolExpression(expressionWhile, nameWhile));
            ClearCurrentBlockLocalVariables();
            _g.End();
 
        }

        private void EmitLengthFunctionExpression(NonTerm nonTerm)
        {
            Token lengthFunction;
            NonTermFactory.GetLengthFunctionExpression(nonTerm, out lengthFunction);

            _currentOperandTempResult = GetOperandValue(lengthFunction.Value, lengthFunction).ArrayLength();
        }

        
        #endregion

        #endregion

    }
}
