using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Reflection.Emit;

namespace CoolCore.Syntax
{
    /// <summary>
    /// Стек для хранения семантической информации
    /// </summary>
    public class SyntaxStack
    {
        // сам стек
        private Stack m_Stack = new Stack(100);
        public Stack Stack { get { return m_Stack; } }
        // выпихнуть из стека
        public object Pop() { return m_Stack.Pop(); }
        // посмотреть вершину стека
        public object Peek() { return m_Stack.Peek(); }
        // впихнуть в стек
        public void Push(object value) { m_Stack.Push(value); }
        // выпихнуть определенное количество элементов
        public void Pop(int count)
        {
            while (count-- > 0)
                m_Stack.Pop();
        }

        /// <summary>
        /// Удалить элемент на указанной глубине стека (0 - вершина)
        /// </summary>
        public void Remove(int depth)
        {
            Stack temp = new Stack();
            for (; depth > 0; depth--)
                temp.Push(m_Stack.Pop());
            m_Stack.Pop();
            while (temp.Count > 0)
                m_Stack.Push(temp.Pop());
        }

        // извлечь строку
        public string PopString() { return (string)m_Stack.Pop(); }
        // извлечь элемент определенного типа
        public Type PopType() { return (Type)m_Stack.Pop(); }
        // извлечь "тело"
        public Body PopBody() { return (Body)m_Stack.Pop(); }
        // извлечь выражение
        public Expression PopExpression() { return (Expression)m_Stack.Pop(); }
        // 
        public CollectionBase PeekCollection() { return (CollectionBase)m_Stack.Peek(); }
        // извлечь 
        public Statement PopStatement() { return (Statement)m_Stack.Pop(); }
        // извлечь инициализацию
        public Assignment PopAssignment() { return (Assignment)m_Stack.Pop(); }
    }

    /// <summary>
    /// 
    /// </summary>
    public enum PassMethod
    {
        ByValue,
        ByReference
    }

    /// <summary>
    /// Тип переменной
    /// </summary>
    public enum VariableType
    {
        Primitive,
        PrimitiveArray,
        Structure,
        StructureArray
    }

    /// <summary>
    /// Тип примитива
    /// </summary>
    public enum PrimitiveType
    {
        Boolean,
        Integer,
        Real,
        Character,
        Void
    }

    /// <summary>
    /// Тип бинарного оператора
    /// </summary>
    public enum BinaryOperatorType
    {
        None,
        Add,
        Subtract,
        Multiply,
        Divide,
        Modulo,
        GreaterThen,
        LessThen,
        GraterOrEqualTo,
        LessOrEqualTo,
        Equal,
        NotEqual,
        And,
        Or
    }

    /// <summary>
    /// Тип унарного оператора
    /// </summary>
    public enum UnaryOperatorType
    {
        None,
        Positive,
        Negative,
        Not,
        Indexer
    }


    /// <summary>
    /// Описывает тип
    /// </summary>
    public class Type
    {
        /// <summary>
        /// Имя типа, обычно имя структуры если VariableType - структура
        /// </summary>
        public string Name;
        /// <summary>
        /// Тип переменной
        /// </summary>
        public VariableType VariableType = VariableType.Primitive;
        /// <summary>
        /// Тип примитива если VariableType - примитив.
        /// </summary>
        public PrimitiveType PrimitiveType = PrimitiveType.Void;

        // является ли тип ссылочным
        public bool IsRef = false;

        /// <summary>
        /// Создать тип примитива
        /// </summary>
        /// <param name="primitiveType"></param>
        public Type(PrimitiveType primitiveType)
        {
            VariableType = VariableType.Primitive;
            PrimitiveType = primitiveType;
        }

        /// <summary>
        /// Создать тип структуры
        /// </summary>
        /// <param name="name"></param>
        public Type(string name)
        {
            VariableType = VariableType.Structure;
            Name = name;
        }

        /// <summary>
        /// Создать .NET тип из нашего типа
        /// </summary>
        /// <returns></returns>
        public System.Type ToSystemType()
        {
            if (!IsRef)
            {
                if (VariableType == VariableType.Primitive)
                {
                    if (PrimitiveType == PrimitiveType.Integer)
                        return typeof(int);
                    else if (PrimitiveType == PrimitiveType.Boolean)
                        return typeof(bool);
                    else if (PrimitiveType == PrimitiveType.Character)
                        return typeof(char);
                    else if (PrimitiveType == PrimitiveType.Real)
                        return typeof(float);
                    else if (PrimitiveType == PrimitiveType.Void)
                        return typeof(void);
                }
                else if (VariableType == VariableType.PrimitiveArray)
                {
                    if (PrimitiveType == PrimitiveType.Integer)
                        return typeof(int[]);
                    else if (PrimitiveType == PrimitiveType.Boolean)
                        return typeof(bool[]);
                    else if (PrimitiveType == PrimitiveType.Character)
                        return typeof(char[]);
                    else if (PrimitiveType == PrimitiveType.Real)
                        return typeof(float[]);
                }
            }
            else
            {
                if (VariableType == VariableType.Primitive)
                {
                    if (PrimitiveType == PrimitiveType.Integer)
                        return System.Type.GetType("System.Int32&");
                    else
                        return typeof(void);
                }
                else if (VariableType == VariableType.PrimitiveArray)
                {
                    if (PrimitiveType == PrimitiveType.Integer)
                        return System.Type.GetType("System.Int32[]&");
                    else
                        return typeof(void);
                }
            }
            return null;
        }

        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        private Type() { }	

        /// <summary>
        /// Create a primitive array type from a non array type.
        /// </summary>
        public static Type CreateArrayFromType(Type type)
        {
            if (type.VariableType == VariableType.Primitive)
                type.VariableType = VariableType.PrimitiveArray;
            else if (type.VariableType == VariableType.Structure)
                type.VariableType = VariableType.StructureArray;
            return type;
        }

        /// <summary>
        /// Создать тип - массив примитивов
        /// </summary>
        public static Type CreateArrayType(PrimitiveType primitiveType)
        {
            Type newType = new Type();
            newType.VariableType = VariableType.PrimitiveArray;
            newType.PrimitiveType = primitiveType;
            return newType;
        }

        /// <summary>
        /// Создать тип - массив структур
        /// </summary>
        public static Type CreateArrayType(string name)
        {
            Type newType = new Type();
            newType.VariableType = VariableType.StructureArray;
            newType.Name = name;
            return newType;
        }
    }

    /// <summary>
    /// Тип литерала
    /// </summary>
    public enum LiteralType
    {
        Boolean,
        Integer,
        Real,
        Character,
        String
    }

    /// <summary>
    /// Описывает литерал найденный в тексте
    /// </summary>
    public class Literal : Expression
    {
        /// <summary>
        /// текстовове предсталение литерального символа
        /// </summary>
        public string Value;

        /// <summary>
        /// тип литерала
        /// </summary>
        public LiteralType LiteralType;

        /// <summary>
        /// Конструтор
        /// </summary>
        /// <param name="value">тектовое представление литерала</param>
        /// <param name="literalType">тип литерала</param>
        public Literal(string value, LiteralType literalType)
        {
            Value = value; 
            LiteralType = literalType;
        }
    }

    /// <summary>
    /// Описывает компилируемый модуль
    /// </summary>
    public class Module
    {
        /// <summary>
        /// имя модуля
        /// </summary>
        public string Name;

        /// <summary>
        /// тело модуля
        /// </summary>
        public Body Body;

        /// <summary>
        /// Конструктор
        /// </summary>
        public Module(Body body, string name)
        {
            Body = body; 
            Name = name;
        }

    }

    /// <summary>
    /// Описывает тло модуля
    /// </summary>
    public class Body
    {
        public FunctionCollection Functions = null;
        public StructureCollection Structures = null;
        public StatementCollection Statements = null;

        public Compilation.SymbolTable SymbolTable = null;

        /// <summary>
        /// Создание тела модуля из коллекции утверждений
        /// Функции, переменные, структуры буду разделены
        /// </summary>
        public Body(StatementCollection statements)
        {
            if (statements == null)
                return;

            Functions = new FunctionCollection();
            Structures = new StructureCollection();
            Statements = new StatementCollection();

            foreach (Statement statement in statements)
            {
                if (statement.GetType() == typeof(Function))
                    Functions.Add((Function)statement);
                //else if(statement.GetType() == typeof(Variable))
                //	Variables.Add((Variable)statement);
                else if (statement.GetType() == typeof(Structure))
                    Structures.Add((Structure)statement);
                else
                    Statements.Add(statement);
            }
        }
    }

    /// <summary>
    /// Описывает представление функции
    /// </summary>
    public class Function : Statement
    {
        /// <summary>
        /// имя функции
        /// </summary>
        public string Name;
        /// <summary>
        /// тело функции
        /// </summary>
        public Body Body;
        /// <summary>
        /// тип функции
        /// </summary>
        public Type Type;
        /// <summary>
        /// параметры функции
        /// </summary>
        public ParameterCollection Parameters;

        /// <summary>
        /// IL method builder.
        /// </summary>
        public MethodBuilder Builder;

        /// <summary>
        /// Конструктор
        /// </summary>
        public Function(Body body, ParameterCollection parameters, string name, Type type)
        {
            Body = body; Parameters = parameters; Name = name; Type = type;
        }
    }

    /// <summary>
    /// Описывает выражение вызова функции
    /// </summary>
    public class CallStatement : Statement
    {
        /// <summary>
        /// имя функции для вызова
        /// </summary>
        public string Name;

        /// <summary>
        /// параметры для передачи функции
        /// </summary>
        public ArgumentCollection Arguments;

        /// <summary>
        /// Конструктор
        /// </summary>
        public CallStatement(ArgumentCollection arguments, string name)
        {
            Arguments = arguments; 
            Name = name;
        }
    }

    /// <summary>
    /// Описывает вызов функции
    /// </summary>
    public class Call : Expression
    {
        /// <summary>
        /// имя функции для вызова
        /// </summary>
        public string Name;

        /// <summary>
        /// список перещдаваемых параметров
        /// </summary>
        public ArgumentCollection Arguments;

        /// <summary>
        /// Конструктор
        /// </summary>
        public Call(ArgumentCollection arguments, string name)
        {
            Arguments = arguments; Name = name;
        }
    }

    /// <summary>
    /// Описывает структуру
    /// </summary>
    public class Structure : Statement
    {
        /// <summary>
        /// имя сруктуры
        /// </summary>
        public string Name;

        /// <summary>
        /// поля структуры
        /// </summary>
        public VariableCollection Variables = null;

        /// <summary>
        /// конструктор
        /// </summary>
        public Structure(VariableCollection variables, string name)
        {
            Variables = variables; Name = name;
        }

    }

    /// <summary>
    /// Описывает переменную
    /// </summary>
    public class Variable : Statement
    {
        /// <summary>
        /// имя переменной
        /// </summary>
        public string Name;
        /// <summary>
        /// тип переменной
        /// </summary>
        public Type Type;

        /// <summary>
        /// значение по умолчанию
        /// </summary>
        public object Value;

        /// <summary>
        /// конструктор
        /// </summary>
        public Variable(object value, string name, Type type)
        {
            Name = name; Type = type; Value = value;
        }
    }

    /// <summary>
    /// Описывает параметр
    /// </summary>
    public class Parameter
    {
        /// <summary>
        /// имя параметра
        /// </summary>
        public string Name;
        /// <summary>
        /// тип параметра
        /// </summary>
        public Type Type;
        /// <summary>
        /// имя метода кууда данный параметр передается
        /// </summary>
        public PassMethod PassMethod = PassMethod.ByValue;

        /// <summary>
        /// конструктор
        /// </summary>
        public Parameter(string name, Type type, PassMethod passMethod)
        {
            Name = name; Type = type; PassMethod = passMethod;
        }
    }

    /// <summary>
    /// Описывает аргумент
    /// </summary>
    public class Argument
    {
        /// <summary>
        /// 
        /// </summary>
        public Expression Value;

        /// <summary>
        /// метод к которому данынй аргумент относится
        /// </summary>
        public PassMethod PassMethod = PassMethod.ByValue;

        /// <summary>
        /// конструктор
        /// </summary>
        public Argument(Expression value, PassMethod passMethod)
        {
            Value = value; PassMethod = passMethod;
        }
    }

    /// <summary>
    /// Базовый класс
    /// </summary>
    public class Statement
    {

    }


    /// <summary>
    /// Тип элемента
    /// </summary>
    public enum ElementType
    {
        Collection,
        Expression
    }

    /// <summary>
    /// Describes an array or structure initialization element.
    /// </summary>
    public class Element
    {
        public ElementType ElementType = ElementType.Expression;
        public ElementCollection Elements = null;
        public Expression Expression = null;

        /// <summary>
        /// Creates an element collection element object.
        /// </summary>
        public Element(ElementCollection elements)
        {
            ElementType = ElementType.Collection;
            Elements = elements;
        }

        /// <summary>
        /// Creates a expression element object.
        /// </summary>
        public Element(Expression expression)
        {
            ElementType = ElementType.Expression;
            Expression = expression;
        }
    }

    public abstract class Expression
    {
        public Type Type;
    }

    public class BinaryExpression : Expression
    {
        public Expression Right = null;
        public Expression Left = null;
        public BinaryOperatorType BinaryOperatorType = BinaryOperatorType.None;

        public BinaryExpression(Expression right, Expression left, BinaryOperatorType binaryOperatorType)
        {
            Right = right; Left = left; BinaryOperatorType = binaryOperatorType;
        }
    }

    public class UnaryExpression : Expression
    {
        public Expression Value = null;
        public Expression Indexer = null;
        public UnaryOperatorType UnaryOperatorType = UnaryOperatorType.None;

        public UnaryExpression(Expression indexer, Expression value, UnaryOperatorType unaryOperatorType)
        {
            Value = value; Indexer = indexer; UnaryOperatorType = unaryOperatorType;
        }
    }

    public class Name : Expression
    {
        public string Value;
        public Name(string value)
        {
            Value = value;
        }
    }

    /// <summary>
    /// Описывает return
    /// </summary>
    public class Return : Statement
    {
        /// <summary>
        /// возвращаемое значение
        /// </summary>
        public Expression Value;
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="value"></param>
        public Return(Expression value)
        {
            Value = value;
        }
    }

    /// <summary>
    /// Описывает if конструкцию
    /// </summary>
    public class If : Statement
    {
        public Expression Condition = null;
        public Body IfBody = null;
        public Body ElseBody = null;

        public If(Body elseBody, Body ifBody, Expression condition)
        {
            ElseBody = elseBody; IfBody = ifBody; Condition = condition;
        }
    }

    /// <summary>
    /// Описывает while конструкцию
    /// </summary>
    public class While : Statement
    {
        public Expression Condition = null;
        public Body Body = null;

        public While(Body body, Expression condition)
        {
            Body = body;
            Condition = condition;
        }
    }

    public class Do : Statement
    {
        public Expression Condition = null;
        public Body Body = null;

        public Do(Expression condition, Body body)
        {
            Body = body;
            Condition = condition;
        }
    }

    public class For : Statement
    {
        public Assignment Initializer = null;
        public Expression Condition = null;
        public Assignment Counter = null;
        public Body Body = null;

        public For(Body body, Assignment counter, Expression condition, Assignment initializer)
        {
            Body = body; Counter = counter; Condition = condition; Initializer = initializer;
        }
    }

    /// <summary>
    /// Описывает присваивание
    /// </summary>
    public class Assignment : Statement
    {
        public string Name;
        public Expression Value;
        public Expression Index;
        public Assignment(Expression value, Expression index, string name)
        {
            Value = value; Name = name; Index = index;
        }
    }

    public class Semantics
    {
        public static void Apply(Production production, SyntaxStack stack)
        {
            switch (production.m_ID)
            {
                #region example
                /*case 0: // <Literal> ::= boolean_literal
                    // Create boolean literal object.
                    stack.Push(new Literal(stack.PopString(), LiteralType.Boolean));
                    break;
                case 1: // <Literal> ::= integer_literal
                    // Create integer literal object.
                    stack.Push(new Literal(stack.PopString(), LiteralType.Integer));
                    break;
                case 2: // <Literal> ::= real_literal
                    // Create real literal object.
                    stack.Push(new Literal(stack.PopString(), LiteralType.Real));
                    break;
                case 3: // <Literal> ::= character_literal
                    // Create character literal object.
                    stack.Push(new Literal(stack.PopString(), LiteralType.Character));
                    break;
                case 4: // <Literal> ::= string_literal
                    // Create string literal object.
                    stack.Push(new Literal(stack.PopString(), LiteralType.String));
                    break;
                case 5: // <Type> ::= <Structure_Type>
                    // !!! DO NOTHING !!!
                    break;
                case 6: // <Type> ::= <Primitive_Type>
                    // !!! DO NOTHING !!!
                    break;
                case 7: // <Type> ::= <Array_Type>
                    // !!! DO NOTHING !!!
                    break;
                case 8: // <Structure_Type> ::= identifier
                    // Create structure type object.
                    stack.Push(new Type(stack.PopString()));
                    break;
                case 9: // <Primitive_Type> ::= bool
                    // Create a boolean type object.
                    stack.Pop(1);
                    stack.Push(new Type(PrimitiveType.Boolean));
                    break;
                case 10: // <Primitive_Type> ::= int
                    // Create an integer type object.
                    stack.Pop(1);
                    stack.Push(new Type(PrimitiveType.Integer));
                    break;
                case 11: // <Primitive_Type> ::= real
                    // Create an real type object.
                    stack.Pop(1);
                    stack.Push(new Type(PrimitiveType.Real));
                    break;
                case 12: // <Primitive_Type> ::= char
                    // Create an character type object.
                    stack.Pop(1);
                    stack.Push(new Type(PrimitiveType.Character));
                    break;
                case 13: // <Primitive_Type> ::= void
                    // Create an void type object.
                    stack.Pop(1);
                    stack.Push(new Type(PrimitiveType.Void));
                    break;
                case 14: // <Array_Type> ::= <Structure_Type> '[' ']'
                    // Create a structure array type.
                    stack.Pop(2);
                    stack.Push(Type.CreateArrayFromType(stack.PopType()));
                    break;
                case 15: // <Array_Type> ::= <Primitive_Type> '[' ']'
                    // Create a primitive array type.
                    stack.Pop(2);
                    stack.Push(Type.CreateArrayFromType(stack.PopType()));
                    break;
                case 16: // <Module> ::= module identifier <Body>
                    // Create a module object.
                    stack.Remove(2);
                    stack.Push(new Module(stack.PopBody(), stack.PopString()));
                    break;
                case 17: // <Body> ::= '{' <Statements> '}'
                    // Create a body object.
                    stack.Pop(1);
                    Body body = new Body((StatementCollection)stack.Pop());
                    stack.Pop(1);
                    stack.Push(body);
                    break;
                case 18: // <Body> ::= '{' '}'
                    // Create a null body object.
                    stack.Pop(2);
                    stack.Push(new Body(null));
                    break;
                case 19: // <Name> ::= identifier
                    // !!! DO NOTHING !!!
                    break;
                case 20: // <Name> ::= <Name> . identifier
                    // Append name section to top of stack.
                    string identifer = stack.PopString();
                    stack.Pop(1);
                    string name = stack.PopString();
                    stack.Push(name + "." + identifer);
                    break;
                case 21: // <Variable_Declarations> ::= <Variable_Declaration>
                    // Create a variable collection containing the variable on top of the stack.
                    stack.Push(new VariableCollection((Variable)stack.Pop()));
                    break;
                case 22: // <Variable_Declarations> ::= <Variable_Declarations> <Variable_Declaration>
                    // Add the variable on top of the stack to the variable collection.
                    Variable variable = (Variable)stack.Pop();
                    ((VariableCollection)stack.Peek()).Add(variable);
                    break;
                case 23: // <Variable_Declaration> ::= <Type> identifier ;
                    // Create a variable object with a null value.
                    stack.Pop(1);
                    stack.Push(new Variable(null, stack.PopString(), stack.PopType()));
                    break;
                case 24: // <Variable_Declaration> ::= <Type> identifier = <Expression> ;
                    // Create a variable object with an expression value.
                    stack.Pop(1);
                    stack.Remove(1);
                    stack.Push(new Variable(stack.PopExpression(), stack.PopString(), stack.PopType()));
                    break;
                case 25: // <Variable_Declaration> ::= <Type> identifier = new '[' <Expression> ']' ;
                    // Create a variable object with an expression value.
                    stack.Pop(2); stack.Remove(1); stack.Remove(1); stack.Remove(1);
                    stack.Push(new Variable(stack.PopExpression(), stack.PopString(), stack.PopType()));
                    break;
                case 26: // <Variable_Declaration> ::= <Type> identifier = '{' <Elements> '}' ;
                    // Create a variable object with an element collection value.
                    stack.Pop(2); stack.Remove(1); stack.Remove(1);
                    stack.Push(new Variable(stack.Pop(), stack.PopString(), stack.PopType()));
                    break;
                case 27: // <Elements> ::= <Element>
                    // Create an element collection containing the element on top of the stack.
                    stack.Push(new ElementCollection((Element)stack.Pop()));
                    break;
                case 28: // <Elements> ::= <Elements> , <Element>
                    // Add the element on top of the stack to the current element collection.
                    Element element = (Element)stack.Pop();
                    stack.Pop(1);
                    ((ElementCollection)stack.Peek()).Add(element);
                    break;
                case 29: // <Element> ::= <Expression>
                    // Create a new element object.
                    stack.Push(new Element(stack.PopExpression()));
                    break;
                case 30: // <Element> ::= '{' <Elements> '}'
                    // Create a new element object with elements inside of it.
                    stack.Pop(1); stack.Remove(1);
                    stack.Push(new Element((ElementCollection)stack.Pop()));
                    break;
                case 31: // <Function_Declaration> ::= <Type> identifier '(' ')' <Body>
                    // Create a function object.
                    stack.Remove(1); stack.Remove(1);
                    stack.Push(new Function(stack.PopBody(), null, stack.PopString(), stack.PopType()));
                    break;
                case 32: // <Function_Declaration> ::= <Type> identifier '(' <Parameters> ')' <Body>
                    // Create a function object.
                    stack.Remove(1); stack.Remove(2);
                    stack.Push(new Function(stack.PopBody(), (ParameterCollection)stack.Pop(), stack.PopString(), stack.PopType()));
                    break;
                case 33: // <Parameters> ::= <Parameter>
                    // Create a parameter collection containing the parameter on top of the stack.
                    stack.Push(new ParameterCollection((Parameter)stack.Pop()));
                    break;
                case 34: // <Parameters> ::= <Parameters> , <Parameter>
                    // Add parameter to parameter collection.
                    Parameter parameter = (Parameter)stack.Pop();
                    stack.Pop(1);
                    ((ParameterCollection)stack.Peek()).Add(parameter);
                    break;
                case 35: // <Parameter> ::= <Type> identifier
                    // Create a parameter object.
                    stack.Push(new Parameter(stack.PopString(), stack.PopType(), PassMethod.ByValue));
                    break;
                case 36: // <Parameter> ::= ref <Type> identifier
                    // Create a parameter object.
                    stack.Remove(2);
                    stack.Push(new Parameter(stack.PopString(), stack.PopType(), PassMethod.ByReference));
                    ((Parameter)stack.Peek()).Type.IsRef = true;
                    break;
                case 37: // <Function_Call> ::= <Name> '(' ')'
                    // Create a function call object.
                    stack.Pop(2);
                    stack.Push(new Call(null, stack.PopString()));
                    break;
                case 38: // <Function_Call> ::= <Name> '(' <Arguments> ')'
                    // Create a function call object.
                    stack.Pop(1); stack.Remove(1);
                    stack.Push(new Call((ArgumentCollection)stack.Pop(), stack.PopString()));
                    break;
                case 39: // <Arguments> ::= <Argument>
                    // Create an argument collection containing the argument on top of stack.
                    stack.Push(new ArgumentCollection((Argument)stack.Pop()));
                    break;
                case 40: // <Arguments> ::= <Arguments> , <Argument>
                    // Add argument to argument collection.
                    Argument argument = (Argument)stack.Pop();
                    stack.Pop(1);
                    ((ArgumentCollection)stack.Peek()).Add(argument);
                    break;
                case 41: // <Argument> ::= <Expression>
                    // Create argument object.
                    stack.Push(new Argument(stack.PopExpression(), PassMethod.ByValue));
                    break;
                case 42: // <Argument> ::= ref <Expression>
                    // Create argument object.
                    stack.Remove(1);
                    stack.Push(new Argument(stack.PopExpression(), PassMethod.ByReference));
                    break;
                case 43: // <Structure_Declaration> ::= struct identifier '{' <Variable_Declarations> '}'
                    // Create structure object.
                    stack.Pop(1); stack.Remove(1); stack.Remove(2);
                    stack.Push(new Structure((VariableCollection)stack.Pop(), stack.PopString()));
                    break;
                case 44: // <Structure_Declaration> ::= struct identifier '{' '}'
                    // Create structure object.
                    stack.Pop(2); stack.Remove(1);
                    stack.Push(new Structure(null, stack.PopString()));
                    break;
                case 45: // <Statements> ::= <Statement>
                    // Create statement collection containing statement on stack.
                    stack.Push(new StatementCollection(stack.PopStatement()));
                    break;
                case 46: // <Statements> ::= <Statements> <Statement>
                    // Add current statement to collection.
                    Statement statement = stack.PopStatement();
                    ((StatementCollection)stack.Peek()).Add(statement);
                    break;
                case 47: // <Statement> ::= <Variable_Declaration>
                    // !!! DO NOTHING !!!
                    break;
                case 48: // <Statement> ::= <Function_Declaration>
                    // !!! DO NOTHING !!!
                    break;
                case 49: // <Statement> ::= <Structure_Declaration>
                    // !!! DO NOTHING !!!
                    break;
                case 50: // <Statement> ::= <Function_Call> ;
                    // Remove ';'
                    stack.Pop(1);
                    Call call = (Call)stack.Pop();
                    stack.Push(new CallStatement(call.Arguments, call.Name));
                    break;
                case 51: // <Statement> ::= <Assignment> ;
                    // Remove ';'
                    stack.Pop(1);
                    break;
                case 52: // <Statement> ::= return <Expression> ;
                    // Create return object.
                    stack.Pop(1); stack.Remove(1);
                    stack.Push(new Return(stack.PopExpression()));
                    break;
                case 53: // <Statement> ::= return ;
                    // Create return object.
                    stack.Pop(2);
                    stack.Push(new Return(null));
                    break;
                case 54: // <Statement> ::= if '(' <Expression> ')' <Body>
                    // Create if statement.
                    stack.Remove(1); stack.Remove(2); stack.Remove(2);
                    stack.Push(new If(null, stack.PopBody(), stack.PopExpression()));
                    break;
                case 55: // <Statement> ::= if '(' <Expression> ')' <Body> else <Body>
                    // Create if statement.
                    stack.Remove(1); stack.Remove(2); stack.Remove(3); stack.Remove(3);
                    stack.Push(new If(stack.PopBody(), stack.PopBody(), stack.PopExpression()));
                    break;
                case 56: // <Statement> ::= while '(' <Expression> ')' <Body>
                    // Create while statement.
                    stack.Remove(1); stack.Remove(2); stack.Remove(2);
                    stack.Push(new While(stack.PopBody(), stack.PopExpression()));
                    break;
                case 57: // <Statement> ::= do <Body> while '(' <Expression> ')'
                    /// Create do statement.
                    stack.Pop(1); stack.Remove(1); stack.Remove(1); stack.Remove(2);
                    stack.Push(new Do(stack.PopExpression(), stack.PopBody()));
                    break;
                case 58: // <Statement> ::= for '(' <Assignment> ; <Expression> ; <Assignment> ')' <Body>
                    // Create for statement.
                    stack.Remove(1); stack.Remove(2); stack.Remove(3); stack.Remove(4); stack.Remove(4);
                    stack.Push(new For(stack.PopBody(), stack.PopAssignment(), stack.PopExpression(), stack.PopAssignment()));
                    break;
                case 59: // <Assignment> ::= set <Name> = <Expression>
                    // Create assignment statement.
                    stack.Remove(1);
                    stack.Remove(2);
                    stack.Push(new Assignment(stack.PopExpression(), null, stack.PopString()));
                    break;
                case 60: // <Assignment> ::= set <Name> [ <Expression> ] = <Expression>
                    // Create assignment statement.
                    stack.Remove(1);
                    stack.Remove(1);
                    stack.Remove(2);
                    stack.Remove(3);
                    stack.Push(new Assignment(stack.PopExpression(), stack.PopExpression(), stack.PopString()));
                    break;
                case 61: // <Assignment> ::= set <Name> '++'
                    // TO DO:
                    break;
                case 62: // <Assignment> ::= set <Name> '--'
                    // TO DO:
                    break;
                case 63: // <Expression> ::= <Expression_Term>
                    // !!! DO NOTHING !!!
                    break;
                case 64: // <Expression> ::= <Expression> '+' <Expression_Term>
                    // Create binary expression.
                    stack.Remove(1);
                    stack.Push(new BinaryExpression(stack.PopExpression(), stack.PopExpression(), BinaryOperatorType.Add));
                    break;
                case 65: // <Expression> ::= <Expression> '-' <Expression_Term>
                    // Create binary expression.
                    stack.Remove(1);
                    stack.Push(new BinaryExpression(stack.PopExpression(), stack.PopExpression(), BinaryOperatorType.Subtract));
                    break;
                case 66: // <Expression_Term> ::= <Expression_Factor>
                    // !!! DO NOTHING !!!
                    break;
                case 67: // <Expression_Term> ::= <Expression_Term> '*' <Expression_Factor>
                    // Create binary expression.
                    stack.Remove(1);
                    stack.Push(new BinaryExpression(stack.PopExpression(), stack.PopExpression(), BinaryOperatorType.Multiply));
                    break;
                case 68: // <Expression_Term> ::= <Expression_Term> / <Expression_Factor>
                    // Create binary expression.
                    stack.Remove(1);
                    stack.Push(new BinaryExpression(stack.PopExpression(), stack.PopExpression(), BinaryOperatorType.Divide));
                    break;
                case 69: // <Expression_Factor> ::= <Expression_Binary>
                    // !!! DO NOTHING !!!
                    break;
                case 70: // <Expression_Factor> ::= <Expression_Factor> % <Expression_Binary>
                    // Create binary expression.
                    stack.Remove(1);
                    stack.Push(new BinaryExpression(stack.PopExpression(), stack.PopExpression(), BinaryOperatorType.Modulo));
                    break;
                case 71: // <Expression_Factor> ::= <Expression_Factor> '>' <Expression_Binary>
                    // Create binary expression.
                    stack.Remove(1);
                    stack.Push(new BinaryExpression(stack.PopExpression(), stack.PopExpression(), BinaryOperatorType.GreaterThen));
                    break;
                case 72: // <Expression_Factor> ::= <Expression_Factor> '<' <Expression_Binary>
                    // Create binary expression.
                    stack.Remove(1);
                    stack.Push(new BinaryExpression(stack.PopExpression(), stack.PopExpression(), BinaryOperatorType.LessThen));
                    break;
                case 73: // <Expression_Factor> ::= <Expression_Factor> '>=' <Expression_Binary>
                    // Create binary expression.
                    stack.Remove(1);
                    stack.Push(new BinaryExpression(stack.PopExpression(), stack.PopExpression(), BinaryOperatorType.GraterOrEqualTo));
                    break;
                case 74: // <Expression_Factor> ::= <Expression_Factor> '<=' <Expression_Binary>
                    // Create binary expression.
                    stack.Remove(1);
                    stack.Push(new BinaryExpression(stack.PopExpression(), stack.PopExpression(), BinaryOperatorType.LessOrEqualTo));
                    break;
                case 75: // <Expression_Factor> ::= <Expression_Factor> == <Expression_Binary>
                    // Create binary expression.
                    stack.Remove(1);
                    stack.Push(new BinaryExpression(stack.PopExpression(), stack.PopExpression(), BinaryOperatorType.Equal));
                    break;
                case 76: // <Expression_Factor> ::= <Expression_Factor> '!=' <Expression_Binary>
                    // Create binary expression.
                    stack.Remove(1);
                    stack.Push(new BinaryExpression(stack.PopExpression(), stack.PopExpression(), BinaryOperatorType.NotEqual));
                    break;
                case 77: // <Expression_Binary> ::= <Expression_Unary>
                    // !!! DO NOTHING !!!
                    break;
                case 78: // <Expression_Binary> ::= <Expression_Binary> && <Expression_Unary>
                    // Create binary expression.
                    stack.Remove(1);
                    stack.Push(new BinaryExpression(stack.PopExpression(), stack.PopExpression(), BinaryOperatorType.And));
                    break;
                case 79: // <Expression_Binary> ::= <Expression_Binary> '||' <Expression_Unary>
                    // Create binary expression.
                    stack.Remove(1);
                    stack.Push(new BinaryExpression(stack.PopExpression(), stack.PopExpression(), BinaryOperatorType.Or));
                    break;
                case 80: // <Expression_Unary> ::= '+' <Expression_Primary>
                    // Create unary expression.
                    stack.Remove(1);
                    stack.Push(new UnaryExpression(null, stack.PopExpression(), UnaryOperatorType.Positive));
                    break;
                case 81: // <Expression_Unary> ::= '-' <Expression_Primary>
                    // Create unary expression.
                    stack.Remove(1);
                    stack.Push(new UnaryExpression(null, stack.PopExpression(), UnaryOperatorType.Negative));
                    break;
                case 82: // <Expression_Unary> ::= '!' <Expression_Primary>
                    // Create unary expression.
                    stack.Remove(1);
                    stack.Push(new UnaryExpression(null, stack.PopExpression(), UnaryOperatorType.Not));
                    break;
                case 83: // <Expression_Unary> ::= <Expression_Primary>
                    // !!! DO NOTHING !!!
                    break;
                case 84: // <Expression_Unary> ::= <Expression_Primary> '[' <Expression> ']'
                    // Create unary expression.
                    stack.Pop(1); stack.Remove(1);
                    stack.Push(new UnaryExpression(stack.PopExpression(), stack.PopExpression(), UnaryOperatorType.Indexer));
                    break;
                case 85: // <Expression_Primary> ::= <Name>
                    // Create name expression.
                    stack.Push(new Name(stack.PopString()));
                    break;
                case 86: // <Expression_Primary> ::= <Function_Call>
                    // !!! DO NOTHING !!!
                    break;
                case 87: // <Expression_Primary> ::= <Literal>
                    // !!! DO NOTHING !!!
                    break;
                case 88:  // <Expression_Primary> ::= '(' <Expression> ')'
                    // Remove pharanthesis
                    stack.Pop(1); stack.Remove(1);
                    break;
                case 109: // <SUPER_CLASS> ::= 
                    //
                    stack.Pop(1);
                    break;*/
                #endregion

                case 0:     // <PROGRAM> ::= <CLASS> <PROGRAM>
                    break;
                case 1:     // <PROGRAM> ::= <METHOD> <PROGRAM>
                    break;
                case 2:     // <PROGRAM> ::= 
                    break;
                case 3:     // <ACCESS_SPEC> ::= private
                    break;
                case 4:     // <ACCESS_SPEC> ::= protected
                    break;
                case 5:     // <ACCESS_SPEC> ::= public
                    break;
                case 6:     // <ADDOP> ::= '+'
                    break;
                case 7:     // <ADDOP> ::= '-'
                    break;
                case 8:     // <ALLOCATOR> ::= new <TYPE_ID> '(' <ARGLIST> ')'
                    break;
                case 9:     // <ALLOCATOR> ::= new <TYPE_ID> '[' <EXPR> ']'
                    break;
                case 10:    // <ARGLIST> ::= <EXPRR>
                    break;
                case 11:    // <ARGLIST> ::= 
                    break;
                case 12:    // <EXPRR> ::= <EXPR>
                    break;
                case 13:    // <EXPRR> ::= <EXPR> ',' <EXPRR>
                    break; 
                case 14:    // <ASSIGNSTMT> ::= <FACTOR> '=' <EXPR>
                    break;
                case 15:    // <BEXPR> ::= <SIMPLEEXPR>
                    break;
                case 16:    // <BEXPR> ::= <SIMPLEEXPR> <RELOP> <SIMPLEEXPR>
                    break;
                case 17:    // <BLOCK> ::= <VARDECS> begin <STMTLIST> end
                    break;
                case 18:    // <BODY> ::= <SUPER_INIT> <THIS_INIT> <BLOCK>
                    break;
                case 19:    // <CALLSTMT> ::= call <FACTOR>
                    break;
                case 20:    // <CAST_EXPR> ::= cast '(' <TYPE_ID> ',' <EXPR> ')'
                    break;
                case 21:    // <CATCH_CLAUSE> ::= catch '(' <TYPE_ID> Id ')' <STMTLIST>
                    break;
                case 22:    // <CEXPR> ::= <BEXPR>
                    break;
                case 23:    // <CEXPR> ::= <BEXPR> and <CEXPR>
                    break;
                case 24:    // <CLASS> ::= class Id <SUPER_CLASS> is <CLASS_MEMBERR> end Id
                    break;
                case 25:    // <CLASS_MEMBERR> ::= <CLASS_MEMBER> <CLASS_MEMBERR>
                    break;
                case 26:    // <CLASS_MEMBERR> ::= 
                    break;  
                case 27:    // <CLASS_MEMBER> ::= <FIELD_DECL>
                    break;
                case 28:    // <CLASS_MEMBER> ::= <METHOD_DECL>
                    break;
                case 29:    // <ELSEPART> ::= else <STMTLIST>
                    break;
                case 30:    // <ELSEPART> ::= 
                    break;
                case 31:    // <EXPR> ::= <CEXPR>
                    break;
                case 32:    // <EXPR> ::= <CEXPR> or <EXPR>
                    break;
                case 33:    // <FACTOR> ::= '-' <FACTOR>
                    break;
                case 34:    // <FACTOR> ::= not <FACTOR>
                    break;
                case 35:    // <FACTOR> ::= Number
                    break;
                case 36:    // <FACTOR> ::= false
                    break;
                case 37:    // <FACTOR> ::= true
                    break;
                case 38:    // <FACTOR> ::= null
                    break;
                case 39:    // <FACTOR> ::= <ALLOCATOR>
                    break;
                case 40:    // <FACTOR> ::= <CAST_EXPR>
                    break;
                case 41:    // <FACTOR> ::= <VALUE_OR_REF> <MEMBER_PARTT>
                    break;
                case 42:    // <MEMBER_PARTT> ::= <MEMBER_PART> <MEMBER_PARTT>
                    break;
                case 43:    // <MEMBER_PARTT> ::= 
                    break;
                case 44:    // <FIELD_DECL> ::= <ACCESS_SPEC> <TYPE> Id <FIELD_DECLL> ';'
                    break;
                case 45:    // <FIELD_DECLL> ::= ',' Id <FIELD_DECLL>
                    break;
                case 46:    // <FIELD_DECLL> ::= 
                    break;
                case 47:    // <IFSTMT> ::= if <EXPR> then <STMTLIST> <ELSEIF_PART> <ELSEPART> end if
                    break;
                case 48:    // <ELSEIF_PART> ::= elsif <EXPR> then <STMTLIST> <ELSEIF_PART>
                    break;
                case 49:    // <ELSEIF_PART> ::= 
                    break;
                case 50:    // <INPUTSTMT> ::= input '>>' <FACTOR>
                    break;
                case 51:    // <LOOPSTMT> ::= loop <STMTLIST> end loop
                    break;
                case 52:    // <MEMBER_PART> ::= '.' Id
                    break;
                case 53:    // <MEMBER_PART> ::= '.' Id '(' <ARGLIST> ')'
                    break;
                case 54:    // <MEMBER_PART> ::= '.' Id '[' <EXPR> ']'
                    break;
                case 55:    // <METHOD> ::= method <M_TYPE> <METHOD_ID> '(' <PARAMETERS> ')' is <BODY> Id
                    break;
                case 56:    // <METHOD_DECL> ::= <ACCESS_SPEC> method <M_TYPE> Id '(' <PARAMETER_DECL> ')' ';'
                    break;
                case 57:    // <METHOD_ID> ::= Id '::' Id
                    break;
                case 58:    // <METHOD_ID> ::= Id
                    break;
                case 59:    // <M_TYPE> ::= <TYPE>
                    break;
                case 60:    // <M_TYPE> ::= void
                    break;
                case 61:    // <MULTOP> ::= '*'
                    break;
                case 62:    // <MULTOP> ::= '/'
                    break;
                case 63:    // <MULTOP> ::= mod
                    break;
                case 64:    // <OPTIONAL_ID> ::= Id
                    break;
                case 65:    // <OPTIONAL_ID> ::= 
                    break;
                case 66:    // <OUTPUTSTMT> ::= output '<<' <EXPR>
                    break;
                case 67:    // <OUTPUTSTMT> ::= output '<<' <STRING_OR_CHAR>
                    break;
                case 68:    // <STRING_OR_CHAR> ::= StringLiteral
                    break;
                case 69:    // <STRING_OR_CHAR> ::= CharLiteral
                    break;
                case 70:    // <STRING_OR_CHAR> ::= 
                    break;
                case 71:    // <PARAMETER_DECL> ::= <TYPE> <OPTIONAL_ID> <PARAMETER_DECLL>
                    break;
                case 72:    // <PARAMETER_DECL> ::= 
                    break;
                case 73:    // <PARAMETER_DECLL> ::= ',' <TYPE> <OPTIONAL_ID> <PARAMETER_DECLL>
                    break;
                case 74:    // <PARAMETER_DECLL> ::= 
                    break;
                case 75:    // <PARAMETERS> ::= <TYPE> Id <TYPEE>
                    break;
                case 76:    // <PARAMETERS> ::= 
                    break;
                case 77:    // <TYPEE> ::= ',' <TYPE> Id <TYPEE>
                    break;
                case 78:    // <TYPEE> ::= 
                    break;
                case 79:    // <RELOP> ::= '=='
                    break;
                case 80:    // <RELOP> ::= '<'
                    break;
                case 81:    // <RELOP> ::= '<='
                    break;
                case 82:    // <RELOP> ::= '>'
                    break;
                case 83:    // <RELOP> ::= '>='
                    break;
                case 84:    // <RELOP> ::= '#'
                    break;
                case 85:    // <SIMPLEEXPR> ::= <TERM> <SIMPLEEXPRR>
                    break;
                case 86:    // <SIMPLEEXPRR> ::= <ADDOP> <TERM> <SIMPLEEXPRR>
                    break;
                case 87:    // <SIMPLEEXPRR> ::= 
                    break;
                case 88:    // <STMT> ::= <BLOCK>
                    break;
                case 89:    // <STMT> ::= <TRYSTMT>
                    break;
                case 90:    // <STMT> ::= <IFSTMT>
                    break;
                case 91:    // <STMT> ::= <LOOPSTMT>
                    break;
                case 92:    // <STMT> ::= <ASSIGNSTMT>
                    break;
                case 93:    // <STMT> ::= <CALLSTMT>
                    break;
                case 94:    // <STMT> ::= <OUTPUTSTMT>
                    break;
                case 95:    // <STMT> ::= <INPUTSTMT>
                    break;
                case 96:    // <STMT> ::= continue
                    break;
                case 97:    // <STMT> ::= break
                    break;
                case 98:    // <STMT> ::= return
                    break;  
                case 99:    // <STMT> ::= return <EXPR>
                    break;
                case 100:   // <STMT> ::= exit
                    break;
                case 101:   // <STMT> ::= throw <EXPR>
                    break;
                case 102:   // <STMTLIST> ::= <STMT> ';' <STMTLISTT>
                    break;
                case 103:   // <STMTLIST> ::=
                    break;
                case 104:   // <STMTLISTT> ::= <STMT> ';' <STMTLISTT>
                    break;
                case 105:   // <STMTLISTT> ::= 
                    break;
                case 106:   // <SUPER_INIT> ::= super '(' <ARGLIST> ')'
                    break;
                case 107:   // <SUPER_INIT> ::= 
                    break;
                case 108:   // <SUPER_CLASS> ::= extends Id
                    break;
                case 109:   // <SUPER_CLASS> ::= 
                    break;
                case 110:   // <TERM> ::= <FACTOR> <TERMM>
                    break;
                case 111:   // <TERMM> ::= <MULTOP> <FACTOR> <TERMM>
                    break;
                case 112:   // <TERMM> ::= 
                    break;
                case 113:   // <THIS_INIT> ::= this '(' <ARGLIST> ')'
                    break;
                case 114:   // <THIS_INIT> ::= 
                    break;
                case 115:   // <TRYSTMT> ::= try <STMTLIST> <CATCH_CLAUSE> <CATCH_CLAUSEE> end try
                    break;
                case 116:   // <CATCH_CLAUSEE> ::= <CATCH_CLAUSE> <CATCH_CLAUSEE>
                    break;
                case 117:   // <CATCH_CLAUSEE> ::= 
                    break;  
                case 118:   // <TYPE> ::= <TYPE_ID>
                    break;
                case 119:   // <TYPE> ::= <TYPE_ID> '[]'
                    break;
                case 120:   // <TYPE_ID> ::= integer
                    break;
                case 121:   // <TYPE_ID> ::= boolean
                    break;
                case 122:   // <TYPE_ID> ::= Id
                    break;
                case 123:   // <VALUE_OR_REF> ::= this
                    break;
                case 124:   // <VALUE_OR_REF> ::= super
                    break;
                case 125:   // <VALUE_OR_REF> ::= Id
                    break;  
                case 126:   // <VALUE_OR_REF> ::= Id '[' <EXPR> ']'
                    break;
                case 127:   // <VALUE_OR_REF> ::= Id '(' <ARGLIST> ')'
                    break;
                case 128:   // <VALUE_OR_REF> ::= '(' <EXPR> ')'
                    break;
                case 129:   // <VARDECLIST> ::= <TYPE> Id <TYPEEE> ';'
                    break;
                case 130:   // <TYPEEE> ::= ',' Id <TYPEE>
                    break;
                case 131:   // <TYPEEE> ::= 
                    break;
                case 132:   // <VARDECS> ::= declare <VARDECLIST> <VARDECLISTT>
                    break;
                case 133:   // <VARDECS> ::= 
                    break;  
                case 134:   // <VARDECLISTT> ::= <VARDECLIST> <VARDECLISTT>
                    break;
                case 135:   // <VARDECLISTT> ::= 
                    break;
            }

            //@Program3 = 2,                             
            //@Access_spec_Private = 3,                  
            //@Access_spec_Protected = 4,                // <ACCESS_SPEC> ::= protected
            //@Access_spec_Public = 5,                   // <ACCESS_SPEC> ::= public
            //@Addop_Plus = 6,                           // <ADDOP> ::= '+'
            //@Addop_Minus = 7,                          // <ADDOP> ::= '-'
            //@Allocator_New_Lparan_Rparan = 8,          // <ALLOCATOR> ::= new <TYPE_ID> '(' <ARGLIST> ')'
            //@Allocator_New_Lbracket_Rbracket = 9,      // <ALLOCATOR> ::= new <TYPE_ID> '[' <EXPR> ']'
            //@Arglist = 10,                             // <ARGLIST> ::= <EXPRR>
            //@Arglist2 = 11,                            // <ARGLIST> ::= 
            //@Exprr = 12,                               // <EXPRR> ::= <EXPR>
            //@Exprr_Comma = 13,                         // <EXPRR> ::= <EXPR> ',' <EXPRR>
            //@Assignstmt_Eq = 14,                       // <ASSIGNSTMT> ::= <FACTOR> '=' <EXPR>
            //@Bexpr = 15,                               // <BEXPR> ::= <SIMPLEEXPR>
            //@Bexpr2 = 16,                              // <BEXPR> ::= <SIMPLEEXPR> <RELOP> <SIMPLEEXPR>
            //@Block_Begin_End = 17,                     // <BLOCK> ::= <VARDECS> begin <STMTLIST> end
            //@Body = 18,                                // <BODY> ::= <SUPER_INIT> <THIS_INIT> <BLOCK>
            //@Callstmt_Call = 19,                       // <CALLSTMT> ::= call <FACTOR>
            //@Cast_expr_Cast_Lparan_Comma_Rparan = 20,  // <CAST_EXPR> ::= cast '(' <TYPE_ID> ',' <EXPR> ')'
            //@Catch_clause_Catch_Lparan_Id_Rparan = 21,  // <CATCH_CLAUSE> ::= catch '(' <TYPE_ID> Id ')' <STMTLIST>
            //@Cexpr = 22,                               // <CEXPR> ::= <BEXPR>
            //@Cexpr_And = 23,                           // <CEXPR> ::= <BEXPR> and <CEXPR>
            //@Class_Class_Id_Is_End_Id = 24,            // <CLASS> ::= class Id <SUPER_CLASS> is <CLASS_MEMBERR> end Id
            //@Class_memberr = 25,                       // <CLASS_MEMBERR> ::= <CLASS_MEMBER> <CLASS_MEMBERR>
            //@Class_memberr2 = 26,                      // <CLASS_MEMBERR> ::= 
            //@Class_member = 27,                        // <CLASS_MEMBER> ::= <FIELD_DECL>
            //@Class_member2 = 28,                       // <CLASS_MEMBER> ::= <METHOD_DECL>
            //@Elsepart_Else = 29,                       // <ELSEPART> ::= else <STMTLIST>
            //@Elsepart = 30,                            // <ELSEPART> ::= 
            //@Expr = 31,                                // <EXPR> ::= <CEXPR>
            //@Expr_Or = 32,                             // <EXPR> ::= <CEXPR> or <EXPR>
            //@Factor_Minus = 33,                        // <FACTOR> ::= '-' <FACTOR>
            //@Factor_Not = 34,                          // <FACTOR> ::= not <FACTOR>
            //@Factor_Number = 35,                       // <FACTOR> ::= Number
            //@Factor_False = 36,                        // <FACTOR> ::= false
            //@Factor_True = 37,                         // <FACTOR> ::= true
            //@Factor_Null = 38,                         // <FACTOR> ::= null
            //@Factor = 39,                              // <FACTOR> ::= <ALLOCATOR>
            //@Factor2 = 40,                             // <FACTOR> ::= <CAST_EXPR>
            //@Factor3 = 41,                             // <FACTOR> ::= <VALUE_OR_REF> <MEMBER_PARTT>
            //@Member_partt = 42,                        // <MEMBER_PARTT> ::= <MEMBER_PART> <MEMBER_PARTT>
            //@Member_partt2 = 43,                       // <MEMBER_PARTT> ::= 
            //@Field_decl_Id_Semi = 44,                  // <FIELD_DECL> ::= <ACCESS_SPEC> <TYPE> Id <FIELD_DECLL> ';'
            //@Field_decll_Comma_Id = 45,                // <FIELD_DECLL> ::= ',' Id <FIELD_DECLL>
            //@Field_decll = 46,                         // <FIELD_DECLL> ::= 
            //@Ifstmt_If_Then_End_If = 47,               // <IFSTMT> ::= if <EXPR> then <STMTLIST> <ELSEIF_PART> <ELSEPART> end if
            //@Elseif_part_Elsif_Then = 48,              // <ELSEIF_PART> ::= elsif <EXPR> then <STMTLIST> <ELSEIF_PART>
            //@Elseif_part = 49,                         // <ELSEIF_PART> ::= 
            //@Inputstmt_Input_Gtgt = 50,                // <INPUTSTMT> ::= input '>>' <FACTOR>
            //@Loopstmt_Loop_End_Loop = 51,              // <LOOPSTMT> ::= loop <STMTLIST> end loop
            //@Member_part_Dot_Id = 52,                  // <MEMBER_PART> ::= '.' Id
            //@Member_part_Dot_Id_Lparan_Rparan = 53,    // <MEMBER_PART> ::= '.' Id '(' <ARGLIST> ')'
            //@Member_part_Dot_Id_Lbracket_Rbracket = 54,  // <MEMBER_PART> ::= '.' Id '[' <EXPR> ']'
            //@Method_Method_Lparan_Rparan_Is_Id = 55,   // <METHOD> ::= method <M_TYPE> <METHOD_ID> '(' <PARAMETERS> ')' is <BODY> Id
            //@Method_decl_Method_Id_Lparan_Rparan_Semi = 56,  // <METHOD_DECL> ::= <ACCESS_SPEC> method <M_TYPE> Id '(' <PARAMETER_DECL> ')' ';'
            //@Method_id_Id_Coloncolon_Id = 57,          // <METHOD_ID> ::= Id '::' Id
            //@Method_id_Id = 58,                        // <METHOD_ID> ::= Id
            //@M_type = 59,                              // <M_TYPE> ::= <TYPE>
            //@M_type_Void = 60,                         // <M_TYPE> ::= void
            //@Multop_Times = 61,                        // <MULTOP> ::= '*'
            //@Multop_Div = 62,                          // <MULTOP> ::= '/'
            //@Multop_Mod = 63,                          // <MULTOP> ::= mod
            //@Optional_id_Id = 64,                      // <OPTIONAL_ID> ::= Id
            //@Optional_id = 65,                         // <OPTIONAL_ID> ::= 
            //@Outputstmt_Output_Ltlt = 66,              // <OUTPUTSTMT> ::= output '<<' <EXPR>
            //@Outputstmt_Output_Ltlt2 = 67,             // <OUTPUTSTMT> ::= output '<<' <STRING_OR_CHAR>
            //@String_or_char_Stringliteral = 68,        // <STRING_OR_CHAR> ::= StringLiteral
            //@String_or_char_Charliteral = 69,          // <STRING_OR_CHAR> ::= CharLiteral
            //@String_or_char = 70,                      // <STRING_OR_CHAR> ::= 
            //@Parameter_decl = 71,                      // <PARAMETER_DECL> ::= <TYPE> <OPTIONAL_ID> <PARAMETER_DECLL>
            //@Parameter_decl2 = 72,                     // <PARAMETER_DECL> ::= 
            //@Parameter_decll_Comma = 73,               // <PARAMETER_DECLL> ::= ',' <TYPE> <OPTIONAL_ID> <PARAMETER_DECLL>
            //@Parameter_decll = 74,                     // <PARAMETER_DECLL> ::= 
            //@Parameters_Id = 75,                       // <PARAMETERS> ::= <TYPE> Id <TYPEE>
            //@Parameters = 76,                          // <PARAMETERS> ::= 
            //@Typee_Comma_Id = 77,                      // <TYPEE> ::= ',' <TYPE> Id <TYPEE>
            //@Typee = 78,                               // <TYPEE> ::= 
            //@Relop_Eqeq = 79,                          // <RELOP> ::= '=='
            //@Relop_Lt = 80,                            // <RELOP> ::= '<'
            //@Relop_Lteq = 81,                          // <RELOP> ::= '<='
            //@Relop_Gt = 82,                            // <RELOP> ::= '>'
            //@Relop_Gteq = 83,                          // <RELOP> ::= '>='
            //@Relop_Num = 84,                           // <RELOP> ::= '#'
            //@Simpleexpr = 85,                          // <SIMPLEEXPR> ::= <TERM> <SIMPLEEXPRR>
            //@Simpleexprr = 86,                         // <SIMPLEEXPRR> ::= <ADDOP> <TERM> <SIMPLEEXPRR>
            //@Simpleexprr2 = 87,                        // <SIMPLEEXPRR> ::= 
            //@Stmt = 88,                                // <STMT> ::= <BLOCK>
            //@Stmt2 = 89,                               // <STMT> ::= <TRYSTMT>
            //@Stmt3 = 90,                               // <STMT> ::= <IFSTMT>
            //@Stmt4 = 91,                               // <STMT> ::= <LOOPSTMT>
            //@Stmt5 = 92,                               // <STMT> ::= <ASSIGNSTMT>
            //@Stmt6 = 93,                               // <STMT> ::= <CALLSTMT>
            //@Stmt7 = 94,                               // <STMT> ::= <OUTPUTSTMT>
            //@Stmt8 = 95,                               // <STMT> ::= <INPUTSTMT>
            //@Stmt_Continue = 96,                       // <STMT> ::= continue
            //@Stmt_Break = 97,                          // <STMT> ::= break
            //@Stmt_Return = 98,                         // <STMT> ::= return
            //@Stmt_Return2 = 99,                        // <STMT> ::= return <EXPR>
            //@Stmt_Exit = 100,                          // <STMT> ::= exit
            //@Stmt_Throw = 101,                         // <STMT> ::= throw <EXPR>
            //@Stmtlist_Semi = 102,                      // <STMTLIST> ::= <STMT> ';' <STMTLISTT>
            //@Stmtlist = 103,                           // <STMTLIST> ::= 
            //@Stmtlistt_Semi = 104,                     // <STMTLISTT> ::= <STMT> ';' <STMTLISTT>
            //@Stmtlistt = 105,                          // <STMTLISTT> ::= 
            //@Super_init_Super_Lparan_Rparan = 106,     // <SUPER_INIT> ::= super '(' <ARGLIST> ')'
            //@Super_init = 107,                         // <SUPER_INIT> ::= 
            //@Super_class_Extends_Id = 108,             // <SUPER_CLASS> ::= extends Id
            //@Super_class = 109,                        // <SUPER_CLASS> ::= 
            //@Term = 110,                               // <TERM> ::= <FACTOR> <TERMM>
            //@Termm = 111,                              // <TERMM> ::= <MULTOP> <FACTOR> <TERMM>
            //@Termm2 = 112,                             // <TERMM> ::= 
            //@This_init_This_Lparan_Rparan = 113,       // <THIS_INIT> ::= this '(' <ARGLIST> ')'
            //@This_init = 114,                          // <THIS_INIT> ::= 
            //@Trystmt_Try_End_Try = 115,                // <TRYSTMT> ::= try <STMTLIST> <CATCH_CLAUSE> <CATCH_CLAUSEE> end try
            //@Catch_clausee = 116,                      // <CATCH_CLAUSEE> ::= <CATCH_CLAUSE> <CATCH_CLAUSEE>
            //@Catch_clausee2 = 117,                     // <CATCH_CLAUSEE> ::= 
            //@Type = 118,                               // <TYPE> ::= <TYPE_ID>
            //@Type_Lbracketrbracket = 119,              // <TYPE> ::= <TYPE_ID> '[]'
            //@Type_id_Integer = 120,                    // <TYPE_ID> ::= integer
            //@Type_id_Boolean = 121,                    // <TYPE_ID> ::= boolean
            //@Type_id_Id = 122,                         // <TYPE_ID> ::= Id
            //@Value_or_ref_This = 123,                  // <VALUE_OR_REF> ::= this
            //@Value_or_ref_Super = 124,                 // <VALUE_OR_REF> ::= super
            //@Value_or_ref_Id = 125,                    // <VALUE_OR_REF> ::= Id
            //@Value_or_ref_Id_Lbracket_Rbracket = 126,  // <VALUE_OR_REF> ::= Id '[' <EXPR> ']'
            //@Value_or_ref_Id_Lparan_Rparan = 127,      // <VALUE_OR_REF> ::= Id '(' <ARGLIST> ')'
            //@Value_or_ref_Lparan_Rparan = 128,         // <VALUE_OR_REF> ::= '(' <EXPR> ')'
            //@Vardeclist_Id_Semi = 129,                 // <VARDECLIST> ::= <TYPE> Id <TYPEEE> ';'
            //@Typeee_Comma_Id = 130,                    // <TYPEEE> ::= ',' Id <TYPEE>
            //@Typeee = 131,                             // <TYPEEE> ::= 
            //@Vardecs_Declare = 132,                    // <VARDECS> ::= declare <VARDECLIST> <VARDECLISTT>
            //@Vardecs = 133,                            // <VARDECS> ::= 
            //@Vardeclistt = 134,                        // <VARDECLISTT> ::= <VARDECLIST> <VARDECLISTT>
            //@Vardeclistt2 = 135                        // <VARDECLISTT> ::= 
            //}
        }
    }

}