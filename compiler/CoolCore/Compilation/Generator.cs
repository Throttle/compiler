using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections;
using System.Collections.Generic;

namespace CoolCore.Compilation
{
    public class Generator
    {
        private Module m_Module = null;
        private SymbolTable m_Globals = null;
        private Stack<Label> begin_block = new Stack<Label>();
        private Stack<Label> end_block = new Stack<Label>();


        public Generator(Module module)
        {
            m_Module = module;
        }

        public void Compile(string path)
        {
            AppDomain domain = System.Threading.Thread.GetDomain();
            AssemblyName name = new AssemblyName();
            name.Name = "Sharp Code Assembly";

            AssemblyBuilder assemblyBuilder = domain.DefineDynamicAssembly(name, AssemblyBuilderAccess.Save);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(m_Module.Name, m_Module.Name + ".exe", true);

            //
            // Создаем глобальные переменные
            //

            TypeBuilder globalBuilder = moduleBuilder.DefineType("Global");
            m_Globals = new SymbolTable();
            foreach (Statement statement in m_Module.Body.Statements)
            {
                if (statement is Variable)
                {
                    Variable variable = statement as Variable;
                    System.Type type = variable.Type.ToSystemType();
                    FieldBuilder field = globalBuilder.DefineField(variable.Name, type, FieldAttributes.Public | FieldAttributes.Static);
                    m_Globals.Add(variable.Name, SymbolType.Variable, statement, field);
                }
            }

            globalBuilder.CreateType();

            //
            // Строим функции
            //

            m_Module.Body.SymbolTable = m_Globals;
            BuildFunctionStubs(m_Module.Body, moduleBuilder);

            foreach (Function function in m_Module.Body.Functions)
            {
                BuildFunction(function);
            }

            //
            // Создаем точку входа (функция main)
            //

            MethodBuilder mainBuilder = moduleBuilder.DefineGlobalMethod("Main", MethodAttributes.Public | MethodAttributes.Static, typeof(void), null);
            ILGenerator mainIL = mainBuilder.GetILGenerator();

            m_Module.Body.Statements.Add(new CallStatement(null, "main"));

            EmitBody(mainIL, m_Module.Body, true);

            moduleBuilder.CreateGlobalFunctions();
            assemblyBuilder.SetEntryPoint(mainBuilder.GetBaseDefinition());
            assemblyBuilder.Save(m_Module.Name + ".exe");

            System.IO.File.Move(m_Module.Name + ".exe", path);

        }

        private void FindFunction(Body body, ArrayList functions)
        {
            if (body != null && body.Functions != null)
            {
                foreach (Function function in body.Functions)
                    FindFunction(function.Body, functions);
            }
        }

        private void Error(string message)
        {
            throw new Exception(message);
        }

        private void EmitExpression(ILGenerator il, Expression expression, SymbolTable symbolTable)
        {
            if (expression is BinaryExpression)
            {
                EmitExpression(il, ((BinaryExpression)expression).Left, symbolTable);
                EmitExpression(il, ((BinaryExpression)expression).Right, symbolTable);

                switch (((BinaryExpression)expression).BinaryOperatorType)
                {
                    case BinaryOperatorType.Add:
                        il.Emit(OpCodes.Add);
                        break;
                    case BinaryOperatorType.Subtract:
                        il.Emit(OpCodes.Sub);
                        break;
                    case BinaryOperatorType.Multiply:
                        il.Emit(OpCodes.Mul);
                        break;
                    case BinaryOperatorType.Divide:
                        il.Emit(OpCodes.Div);
                        break;
                    case BinaryOperatorType.Modulo:
                        il.Emit(OpCodes.Rem);
                        break;
                    case BinaryOperatorType.Equal:
                        il.Emit(OpCodes.Ceq);
                        break;
                    case BinaryOperatorType.NotEqual:
                        il.Emit(OpCodes.Ceq);
                        il.Emit(OpCodes.Ldc_I4_0);
                        il.Emit(OpCodes.Ceq);
                        break;
                    case BinaryOperatorType.GreaterThen:
                        il.Emit(OpCodes.Cgt);
                        break;
                    case BinaryOperatorType.LessThen:
                        il.Emit(OpCodes.Clt);
                        break;
                    case BinaryOperatorType.GraterOrEqualTo:
                        il.Emit(OpCodes.Clt);
                        il.Emit(OpCodes.Ldc_I4_0);
                        il.Emit(OpCodes.Ceq);
                        break;
                    case BinaryOperatorType.LessOrEqualTo:
                        il.Emit(OpCodes.Cgt);
                        il.Emit(OpCodes.Ldc_I4_0);
                        il.Emit(OpCodes.Ceq);
                        break;
                    case BinaryOperatorType.And:
                        il.Emit(OpCodes.And);
                        break;
                    case BinaryOperatorType.Or:
                        il.Emit(OpCodes.Or);
                        break;
                }
            }
            else if (expression is UnaryExpression)
            {
                UnaryExpression unaryExpression = expression as UnaryExpression;

                switch (unaryExpression.UnaryOperatorType)
                {
                    case UnaryOperatorType.Indexer:
                        EmitExpression(il, unaryExpression.Value, symbolTable);
                        EmitExpression(il, unaryExpression.Indexer, symbolTable);
                        il.Emit(OpCodes.Ldelem_I4);
                        break;
                    case UnaryOperatorType.Negative:
                        EmitExpression(il, unaryExpression.Value, symbolTable);
                        il.Emit(OpCodes.Neg);
                        break;
                    case UnaryOperatorType.Not:
                        EmitExpression(il, unaryExpression.Value, symbolTable);
                        il.Emit(OpCodes.Not);
                        break;

                }
            }
            else if (expression is Literal)
            {
                Literal literal = expression as Literal;

                switch (literal.LiteralType)
                {
                    case LiteralType.Integer:
                        il.Emit(OpCodes.Ldc_I4, Int32.Parse(literal.Value));
                        break;
                    case LiteralType.Real:
                        il.Emit(OpCodes.Ldc_R4, float.Parse(literal.Value));
                        break;
                    case LiteralType.Character:
                        il.Emit(OpCodes.Ldc_I4, char.GetNumericValue(literal.Value, 0));
                        break;
                    case LiteralType.Boolean:
                        if (literal.Value == "true")
                            il.Emit(OpCodes.Ldc_I4, 1);
                        else if (literal.Value == "false")
                            il.Emit(OpCodes.Ldc_I4, 0);
                        break;
                    case LiteralType.String:
                        il.Emit(OpCodes.Ldstr, literal.Value);
                        break;
                }
            }
            else if (expression is Name)
            {
                Name name = expression as Name;

                Symbol variable = symbolTable.Find(name.Value, SymbolType.Variable);
                if (variable == null)
                    Error("Assignment variable " + name.Value + " unknown.");

                if (variable.CodeObject is LocalBuilder)
                    il.Emit(OpCodes.Ldloc, (LocalBuilder)variable.CodeObject);
                else if (variable.CodeObject is FieldBuilder)
                    il.Emit(OpCodes.Ldsfld, (FieldBuilder)variable.CodeObject);
                else if (variable.CodeObject is ParameterBuilder)
                {
                    Parameter p = variable.SyntaxObject as Parameter;
                    il.Emit(OpCodes.Ldarg_S, ((ParameterBuilder)variable.CodeObject).Position - 1);
                    if (p.PassMethod == PassMethod.ByReference)
                        il.Emit(OpCodes.Ldind_I4);
                }

            }
            else if (expression is Call)
            {
                EmitCall(il, expression as Call, symbolTable);
            }
        }


        // Таблица для хранения функций модуля
        private SymbolTable m_Functions = new SymbolTable();


        private void BuildFunctionStubs(Body body, ModuleBuilder builder)
        {
            if (body == null || builder == null)
                throw new ArgumentNullException();

            //SymbolTable sibillings = new SymbolTable(m_Globals);

            if (body != null && body.Functions != null)
            {
                foreach (Function function in body.Functions)
                {
                    // В фукнции должны быть видны глобальные переменные
                    function.Body.SymbolTable = new SymbolTable(m_Globals);

                    MethodBuilder method = BuildFunctionStub(function, builder);

                    // Добавляем дочернюю функцию в таблицу родителя
                    body.SymbolTable.Add(function.Name, SymbolType.Function, function, method);
                    //sibillings.Add(function.Name, SymbolType.Function, function, method);


                    BuildFunctionStubs(function.Body, builder);
                }

                
            }
        }

        private MethodBuilder BuildFunctionStub(Function function, ModuleBuilder builder)
        {
            if (function == null || builder == null)
                throw new ArgumentNullException();
            //
            // Создаем функцию
            //

            // Имя (уникальное)
            string functionName = function.Name;
            while (m_Functions.Find(functionName, SymbolType.Function) != null)
                functionName += "#";

            // Возвращаемый тип
            System.Type returnType = function.Type.ToSystemType();

            // Параметры
            System.Type[] parameters = null;
            if (function.Parameters != null)
            {
                parameters = new System.Type[function.Parameters.Count];

                for (int x = 0; x < function.Parameters.Count; x++)
                {
                    parameters[x] = function.Parameters[x].Type.ToSystemType();
                }
            }

            // Создаем метод (глобально)
            MethodBuilder method = builder.DefineGlobalMethod(functionName, MethodAttributes.Public | MethodAttributes.Static, returnType, parameters);

            if (function.Parameters != null)
            {
                for (int x = 0; x < function.Parameters.Count; x++)
                {
                    ParameterBuilder p = null;
                    p = method.DefineParameter(x + 1, ParameterAttributes.None, function.Parameters[x].Name);
                    function.Body.SymbolTable.Add(p.Name, SymbolType.Variable, function.Parameters[x], p);
                }
            }

            function.Builder = method;
            m_Functions.Add(functionName, SymbolType.Function, function, method);
            return method;
        }

        private void BuildFunction(Function function)
        {
            if (function == null)
                throw new ArgumentNullException();

            //
            // Собираем дочерние функции
            //

            if (function.Body.Functions != null)
            {
                foreach (Function child in function.Body.Functions)
                    BuildFunction(child);
            }

            //
            // Собираем тело функции
            //

            ILGenerator il = function.Builder.GetILGenerator();

            EmitBody(il, function.Body, false);

            return;

        }

        private void EmitBody(ILGenerator il, Body body, bool root, object additional_param = null)
        {
            //
            // Определяем локальные переменные
            //

            foreach (Variable variable in body.Variables)
            {
                LocalBuilder local = null;
                FieldBuilder global = null;

                if (root)
                {
                    global = (FieldBuilder)body.SymbolTable.Find(variable.Name, SymbolType.Variable).CodeObject;
                }
                else
                {
                    local = il.DeclareLocal(variable.Type.ToSystemType());
                    if (variable.Type.VariableType != VariableType.PrimitiveArray)
                        body.SymbolTable.Add(variable.Name, SymbolType.Variable, variable, local);
                }

                //
                // Инициализируем переменные
                //

                if (variable.Type.VariableType == VariableType.Primitive)
                {
                    if (variable.Value != null && variable.Value is Expression)
                    {
                        EmitExpression(il, (Expression)variable.Value, body.SymbolTable);

                        if (root)
                            il.Emit(OpCodes.Stsfld, global);
                        else
                            il.Emit(OpCodes.Stloc, local);
                    }
                }
                else if (variable.Type.VariableType == VariableType.PrimitiveArray)
                {
                    // Определяем пустой массив
                    if (variable.Value != null && variable.Value is Expression)
                    {
                        EmitExpression(il, (Expression)variable.Value, body.SymbolTable);
                        il.Emit(OpCodes.Newarr, variable.Type.ToSystemType());
                        if (root)
                            il.Emit(OpCodes.Stsfld, global);
                        else
                            il.Emit(OpCodes.Stloc, local);
                    }
                    else if (variable.Value != null && variable.Value is ElementCollection)
                    {
                        ElementCollection elements = variable.Value as ElementCollection;

                        il.Emit(OpCodes.Ldc_I4, elements.Count);
                        il.Emit(OpCodes.Newarr, variable.Type.ToSystemType());
                        if (root)
                            il.Emit(OpCodes.Stsfld, global);
                        else
                            il.Emit(OpCodes.Stloc, local);

                        for (int x = 0; x < elements.Count; x++)
                        {
                            // загружаем массив
                            if (root)
                                il.Emit(OpCodes.Ldsfld, global);
                            else
                                il.Emit(OpCodes.Ldloc, local);
                            // загружаем индекс
                            il.Emit(OpCodes.Ldc_I4, x);
                            // значение
                            EmitExpression(il, elements[x].Expression, body.SymbolTable);
                            // сохраняем
                            il.Emit(OpCodes.Stelem_I4);
                        }

                    }

                }
            }

            foreach (Statement statement in body.Statements)
            {
                if (statement is Variable)
                {
                    Variable variable = statement as Variable;
                    // Определяем пустой массив
                    if (variable.Value != null && variable.Value is Expression)
                    {
                        EmitExpression(il, (Expression)variable.Value, body.SymbolTable);
                        il.Emit(OpCodes.Newarr, variable.Type.ToSystemType());
                        LocalBuilder local = null;
                        FieldBuilder global = null;
                        if (root)
                        {
                            global = (FieldBuilder)body.SymbolTable.Find(variable.Name, SymbolType.Variable).CodeObject;
                            il.Emit(OpCodes.Stsfld, global);
                        }
                        else
                        {
                            local = il.DeclareLocal(variable.Type.ToSystemType());
                            body.SymbolTable.Add(variable.Name, SymbolType.Variable, variable, local);
                            il.Emit(OpCodes.Stloc, local);
                        }
                    }
                }
                else if (statement is Assignment)
                {
                    EmitAssignment(il, statement as Assignment, body.SymbolTable);
                }
                else if (statement is Return)
                {
                    if (((Return)statement).Value != null)
                        EmitExpression(il, ((Return)statement).Value, body.SymbolTable);
                    il.Emit(OpCodes.Ret);
                }
                else if (statement is CallStatement)
                {
                    CallStatement call = statement as CallStatement;
                    Symbol symbol = body.SymbolTable.Find(call.Name, SymbolType.Function);
                    EmitCallStatement(il, statement as CallStatement, body.SymbolTable);

                    if (symbol != null)
                    {
                        if (((MethodBuilder)symbol.CodeObject).ReturnType != typeof(void))
                            il.Emit(OpCodes.Pop);
                    }
                    else
                    {
                        if (call.Name == "Read")
                            il.Emit(OpCodes.Pop);
                    }

                }
                else if (statement is If)
                {
                    //
                    // Генерируем if инструкцию
                    //

                    If ifStatement = statement as If;

                    // Вычисляем условие
                    EmitExpression(il, ifStatement.Condition, body.SymbolTable);

                    if (ifStatement.IfBody != null && ifStatement.ElseBody == null)
                    {
                        ifStatement.IfBody.SymbolTable = new SymbolTable(body.SymbolTable);
                        Label exit = il.DefineLabel();
                        il.Emit(OpCodes.Brfalse, exit);
                        EmitBody(il, ifStatement.IfBody, false);
                        il.MarkLabel(exit);
                    }
                    else if (ifStatement.IfBody != null && ifStatement.ElseBody != null)
                    {
                        ifStatement.IfBody.SymbolTable = new SymbolTable(body.SymbolTable);
                        ifStatement.ElseBody.SymbolTable = new SymbolTable(body.SymbolTable);
                        Label exit = il.DefineLabel();
                        Label elseLabel = il.DefineLabel();
                        il.Emit(OpCodes.Brfalse, elseLabel);
                        EmitBody(il, ifStatement.IfBody, false);
                        il.Emit(OpCodes.Br, exit);
                        il.MarkLabel(elseLabel);
                        EmitBody(il, ifStatement.ElseBody, false);
                        il.MarkLabel(exit);
                    }
                }

                else if (statement is While)
                {
                    //
                    // Генерируем while инструкцию
                    //

                    While whileStatement = statement as While;
                    whileStatement.Body.SymbolTable = new SymbolTable(body.SymbolTable);
                    Label begin = il.DefineLabel();
                    begin_block.Push(begin);
                    Label exit = il.DefineLabel();
                    end_block.Push(exit);
                    il.MarkLabel(begin);
                    // Eval condition
                    EmitExpression(il, whileStatement.Condition, body.SymbolTable);
                    il.Emit(OpCodes.Brfalse, exit);
                    EmitBody(il, whileStatement.Body, false, new Label[]{begin, exit});
                    il.Emit(OpCodes.Br, begin);
                    il.MarkLabel(exit);
                    begin_block.Pop();
                    end_block.Pop();

                }
                else if (statement is Break)
                {
                    il.Emit(OpCodes.Br, end_block.Peek());
                }
                else if (statement is Continue)
                {
                    il.Emit(OpCodes.Br, begin_block.Peek());
                }

            }
        }

        private void EmitAssignment(ILGenerator il, Assignment assignment, SymbolTable symbolTable)
        {
            Symbol variable = symbolTable.Find(assignment.Name, SymbolType.Variable);
            if (variable == null)
                Error("Assignment variable " + assignment.Name + " unknown.");

            // Не массив
            if (assignment.Index == null)
            {
                if (variable.CodeObject is ParameterBuilder)
                {
                    Parameter p = variable.SyntaxObject as Parameter;
                    if (p.PassMethod == PassMethod.ByReference)
                        il.Emit(OpCodes.Ldarg_S, ((ParameterBuilder)variable.CodeObject).Position - 1);
                }

                // Получаем значение
                EmitExpression(il, assignment.Value, symbolTable);

                // Сохраняем
                if (variable.CodeObject is LocalBuilder)
                    il.Emit(OpCodes.Stloc, (LocalBuilder)variable.CodeObject);
                else if (variable.CodeObject is FieldBuilder)
                    il.Emit(OpCodes.Stsfld, (FieldBuilder)variable.CodeObject);
                else if (variable.CodeObject is ParameterBuilder)
                {
                    Parameter p = variable.SyntaxObject as Parameter;
                    if (p.PassMethod == PassMethod.ByReference)
                        il.Emit(OpCodes.Stind_I4);
                    else
                        il.Emit(OpCodes.Starg, ((ParameterBuilder)variable.CodeObject).Position - 1);
                }
            }
            else
            {
                // Загружаем массив
                if (variable.CodeObject is LocalBuilder)
                    il.Emit(OpCodes.Ldloc, (LocalBuilder)variable.CodeObject);
                else if (variable.CodeObject is FieldBuilder)
                    il.Emit(OpCodes.Ldsfld, (FieldBuilder)variable.CodeObject);
                // Индекс
                EmitExpression(il, assignment.Index, symbolTable);
                // Значение
                EmitExpression(il, assignment.Value, symbolTable);
                // Устанавливаем значение
                il.Emit(OpCodes.Stelem_I4);
            }
        }

        private void EmitCall(ILGenerator il, Call call, SymbolTable symbolTable)
        {
            Symbol symbol = symbolTable.Find(call.Name, SymbolType.Function);

            if (symbol != null)
            {
                Function function = symbol.SyntaxObject as Function;

                //
                // Проверка аргументов
                //

                if (call.Arguments == null && function.Parameters == null)
                {
                    goto Hack;
                }
                else if (call.Arguments.Count != function.Parameters.Count)
                {
                    Error("Argument mismatch [" + call.Name + "]");
                }
                else if (call.Arguments.Count != function.Parameters.Count)
                {
                    Error("Argument mismatch [" + call.Name + "]");
                }
                else
                {
                    // проверка способа передачи параметра
                    for (int x = 0; x < call.Arguments.Count; x++)
                    {
                        if (call.Arguments[x].PassMethod != function.Parameters[x].PassMethod)
                        {
                            Error("Argument error [" + call.Name + "], argument [" + x + "] is wrong.");
                        }
                    }
                }

                if (call.Arguments != null)
                {
                    foreach (Argument argument in call.Arguments)
                    {
                        if (argument.PassMethod == PassMethod.ByReference)
                        {
                            // Если передаем по ссылке
                            if (argument.Value is Name)
                            {
                                Symbol variable = symbolTable.Find(((Name)argument.Value).Value, SymbolType.Variable);
                                if (variable.CodeObject is LocalBuilder)
                                {
                                    il.Emit(OpCodes.Ldloca, variable.CodeObject as LocalBuilder);
                                }
                                else if (variable.CodeObject is FieldBuilder)
                                {
                                    il.Emit(OpCodes.Ldsflda, variable.CodeObject as FieldBuilder);
                                }
                                else if (variable.CodeObject is ParameterBuilder)
                                {
                                    il.Emit(OpCodes.Ldarga_S, ((ParameterBuilder)variable.CodeObject).Position - 1);
                                }
                            }
                            else if (argument.Value is UnaryExpression && ((UnaryExpression)argument.Value).UnaryOperatorType == UnaryOperatorType.Indexer)
                            {
                                Symbol variable = symbolTable.Find(((Name)argument.Value).Value, SymbolType.Variable);
                                if (variable.CodeObject is LocalBuilder)
                                {
                                    if (((Variable)variable.SyntaxObject).Type.VariableType == VariableType.PrimitiveArray)
                                        Error("ref cannot be applied to arrays");
                                    il.Emit(OpCodes.Ldloca, variable.CodeObject as LocalBuilder);
                                }
                                else if (variable.CodeObject is FieldBuilder)
                                {
                                    if (((Variable)variable.SyntaxObject).Type.VariableType == VariableType.PrimitiveArray)
                                        Error("ref cannot be applied to arrays");
                                    il.Emit(OpCodes.Ldsflda, variable.CodeObject as FieldBuilder);
                                }
                                else if (variable.CodeObject is ParameterBuilder)
                                {
                                    if (((Parameter)variable.SyntaxObject).Type.VariableType == VariableType.PrimitiveArray)
                                        Error("ref cannot be applied to arrays");
                                    il.Emit(OpCodes.Ldarga, ((ParameterBuilder)variable.CodeObject).Position - 1);
                                }

                                EmitExpression(il, ((UnaryExpression)argument.Value).Indexer, symbolTable);
                                il.Emit(OpCodes.Ldelema);
                            }
                            else
                            {
                                Error("ref may only be applied to variables");
                            }
                        }
                        else
                        {
                            EmitExpression(il, argument.Value, symbolTable);
                        }
                    }
                }

            Hack:
                il.Emit(OpCodes.Call, ((MethodBuilder)symbol.CodeObject));
            }
            else
            {
                if (call.Name == "Read")
                {
                    il.Emit(OpCodes.Ldstr, "Input > ");
                    MethodInfo write = System.Type.GetType("System.Console").GetMethod("Write", new System.Type[] { typeof(string) });
                    il.EmitCall(OpCodes.Call, write, null);

                    MethodInfo read = System.Type.GetType("System.Console").GetMethod("ReadLine");
                    MethodInfo parse = System.Type.GetType("System.Int32").GetMethod("Parse", new System.Type[] { typeof(string) });
                    il.EmitCall(OpCodes.Call, read, null);
                    il.EmitCall(OpCodes.Call, parse, null);
                }
                else if (call.Name == "Write")
                {
                    EmitExpression(il, call.Arguments[0].Value, symbolTable);
                    MethodInfo write = System.Type.GetType("System.Console").GetMethod("WriteLine");//, new System.Type[] { typeof(int) });
                    il.EmitCall(OpCodes.Call, write, null);
                }
                else
                {
                    Error("Unknown function name. [" + call.Name + "]");
                }
            }
        }

        private void EmitCallStatement(ILGenerator il, CallStatement call, SymbolTable symbolTable)
        {
            Symbol symbol = symbolTable.Find(call.Name, SymbolType.Function);

            if (symbol != null)
            {
                Function function = symbol.SyntaxObject as Function;

                //
                // Проверка аргументов
                //
                if (call.Arguments == null && function.Parameters == null)
                {
                    goto Hack;
                }
                else if (call.Arguments.Count != function.Parameters.Count)
                {
                    Error("Argument mismatch [" + call.Name + "]");
                }
                else if (call.Arguments.Count != function.Parameters.Count)
                {
                    Error("Argument mismatch [" + call.Name + "]");
                }
                else
                {
                    for (int x = 0; x < call.Arguments.Count; x++)
                    {
                        if (call.Arguments[x].PassMethod != function.Parameters[x].PassMethod)
                        {
                            Error("Argument error [" + call.Name + "], argument [" + x + "] is wrong.");
                        }
                    }
                }

                if (call.Arguments != null)
                {
                    foreach (Argument argument in call.Arguments)
                    {
                        if (argument.PassMethod == PassMethod.ByReference)
                        {
                            // Если параметр передан по ссылке
                            if (argument.Value is Name)
                            {
                                Symbol variable = symbolTable.Find(((Name)argument.Value).Value, SymbolType.Variable);
                                if (variable.CodeObject is LocalBuilder)
                                {
                                    if (((Variable)variable.SyntaxObject).Type.VariableType == VariableType.PrimitiveArray)
                                        Error("ref cannot be applied to arrays");
                                    il.Emit(OpCodes.Ldloca, variable.CodeObject as LocalBuilder);
                                }
                                else if (variable.CodeObject is FieldBuilder)
                                {
                                    if (((Variable)variable.SyntaxObject).Type.VariableType == VariableType.PrimitiveArray)
                                        Error("ref cannot be applied to arrays");
                                    il.Emit(OpCodes.Ldsflda, variable.CodeObject as FieldBuilder);
                                }
                                else if (variable.CodeObject is ParameterBuilder)
                                {
                                    if (((Parameter)variable.SyntaxObject).Type.VariableType == VariableType.PrimitiveArray)
                                        Error("ref cannot be applied to arrays");
                                    il.Emit(OpCodes.Ldarga, ((ParameterBuilder)variable.CodeObject).Position - 1);
                                }
                            }
                            else if (argument.Value is UnaryExpression && ((UnaryExpression)argument.Value).UnaryOperatorType == UnaryOperatorType.Indexer)
                            {
                                Symbol variable = symbolTable.Find(((Name)argument.Value).Value, SymbolType.Variable);
                                if (variable.CodeObject is LocalBuilder)
                                {
                                    il.Emit(OpCodes.Ldloc, variable.CodeObject as LocalBuilder);
                                }
                                else if (variable.CodeObject is FieldBuilder)
                                {
                                    il.Emit(OpCodes.Ldsfld, variable.CodeObject as FieldBuilder);
                                }
                                else if (variable.CodeObject is ParameterBuilder)
                                {
                                    il.Emit(OpCodes.Ldarga, ((ParameterBuilder)variable.CodeObject).Position - 1);
                                }

                                EmitExpression(il, ((UnaryExpression)argument.Value).Indexer, symbolTable);
                                il.Emit(OpCodes.Ldelema);
                            }
                            else
                            {
                                Error("ref may only be applied to variables");
                            }
                        }
                        else
                        {
                            EmitExpression(il, argument.Value, symbolTable);
                        }
                    }
                }

            Hack:
                il.Emit(OpCodes.Call, ((MethodBuilder)symbol.CodeObject));
            }
            else
            {
                if (call.Name == "Read")
                {
                    il.Emit(OpCodes.Ldstr, "Input > ");
                    MethodInfo write = System.Type.GetType("System.Console").GetMethod("Write", new System.Type[] { typeof(string) });
                    il.EmitCall(OpCodes.Call, write, null);

                    MethodInfo read = System.Type.GetType("System.Console").GetMethod("ReadLine");
                    MethodInfo parse = System.Type.GetType("System.Int32").GetMethod("Parse", new System.Type[] { typeof(string) });
                    il.EmitCall(OpCodes.Call, read, null);
                    il.EmitCall(OpCodes.Call, parse, null);

                }
                else if (call.Name == "Write")
                {
                    EmitExpression(il, call.Arguments[0].Value, symbolTable);
                    MethodInfo write = null;

                    if (call.Arguments[0].Value is Literal)
                    {
                        Literal temp = call.Arguments[0].Value as Literal;
                        System.Type type = null;
                        switch (temp.LiteralType)
                        {
                            case LiteralType.Boolean:
                                type = typeof(bool);
                                break;
                            case LiteralType.Character:
                                type = typeof(char);
                                break;
                            case LiteralType.Integer:
                                type = typeof(int);
                                break;
                            case LiteralType.Real:
                                type = typeof(double);
                                break;
                            case LiteralType.String:
                                type = typeof(string);
                                break;
                        }
                        write = System.Type.GetType("System.Console").GetMethod("WriteLine", new System.Type[] { type });
                    }
                    else
                    {
                        write = System.Type.GetType("System.Console").GetMethod("WriteLine", new System.Type[] { typeof(int) });
                    }
                    il.EmitCall(OpCodes.Call, write, null);
                }
                else
                {
                    Error("Unknown function name. [" + call.Name + "]");
                }
            }
        }
    }
}