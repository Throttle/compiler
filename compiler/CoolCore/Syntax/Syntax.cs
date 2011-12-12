using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Reflection.Emit;

namespace CoolCore
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
        public VariableCollection Variables = null;

        public Compilation.SymbolTable SymbolTable = null;

        /// <summary>
        /// Создание тела модуля из коллекции утверждений
        /// Функции, переменные, структуры буду разделены
        /// </summary>
        public Body(StatementCollection statements, VariableCollection variables)
        {
            if (statements == null)
                return;

            Functions = new FunctionCollection();
            Structures = new StructureCollection();
            Statements = new StatementCollection();
            Variables = new VariableCollection();

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

            if (variables == null)
                return;

            foreach (Variable v in variables)
            {
                Variables.Add(v);
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
        private enum ProductionIndex
        {
            @Program = 0,							  // <PROGRAM> ::= <CLASS>
            @Program2 = 1,							 // <PROGRAM> ::= <METHOD>
            @Program3 = 2,							 // <PROGRAM> ::= <PROGRAM> <CLASS>
            @Program4 = 3,							 // <PROGRAM> ::= <PROGRAM> <METHOD>
            @Body = 4,								 // <BODY> ::= <SUPER_INIT> <THIS_INIT> <BLOCK>
            @Body2 = 5,								// <BODY> ::= <THIS_INIT> <BLOCK>
            @Body3 = 6,								// <BODY> ::= <SUPER_INIT> <BLOCK>
            @Body4 = 7,								// <BODY> ::= <BLOCK>
            @This_init_This_Lparan_Rparan = 8,		 // <THIS_INIT> ::= this '(' <ARGLIST> ')'
            @Super_init_Super_Lparan_Rparan = 9,	   // <SUPER_INIT> ::= super '(' <ARGLIST> ')'
            @Block_Begin_End = 10,					 // <BLOCK> ::= <VARDECS> begin <STATEMENTS> end
            @Block_Begin_End2 = 11,					// <BLOCK> ::= begin <STATEMENTS> end
            @Vardeclist_Id_Semi = 12,				  // <VARDECLIST> ::= <TYPE> Id ';'
            @Vardeclist_Id_Semi2 = 13,				 // <VARDECLIST> ::= <TYPE> Id <VAR_TYPELIST> ';'
            @Var_typelist_Comma_Id = 14,			   // <VAR_TYPELIST> ::= ',' Id
            @Var_typelist_Comma_Id2 = 15,			  // <VAR_TYPELIST> ::= <VAR_TYPELIST> ',' Id
            @Vardecs_Declare = 16,					 // <VARDECS> ::= declare <VARDECLIST>
            @Vardecs_Declare2 = 17,					// <VARDECS> ::= declare <VARDECLIST> <VARDECS>
            @Name_Id = 18,							 // <NAME> ::= Id
            @Name_Dot_Id = 19,						 // <NAME> ::= <NAME> '.' Id
            @Assignment_Eq = 20,					   // <ASSIGNMENT> ::= <NAME> '=' <EXPRESSION>
            @Factor_This = 21,						 // <FACTOR> ::= this
            @Factor_Super = 22,						// <FACTOR> ::= super
            @Factor_Number = 23,					   // <FACTOR> ::= Number
            @Factor_False = 24,						// <FACTOR> ::= false
            @Factor_True = 25,						 // <FACTOR> ::= true
            @Factor_Null = 26,						 // <FACTOR> ::= null
            @Factor = 27,							  // <FACTOR> ::= <ALLOCATOR>
            @Factor2 = 28,							 // <FACTOR> ::= <CAST_EXPR>
            @Allocator_New_Lparan_Rparan = 29,		 // <ALLOCATOR> ::= new <TYPE> '(' <ARGLIST> ')'
            @Allocator_New_Lparan_Rparan2 = 30,		// <ALLOCATOR> ::= new <TYPE> '(' ')'
            @Allocator_New_Lbracket_Rbracket = 31,	 // <ALLOCATOR> ::= new <TYPE> '[' <EXPRESSION> ']'
            @Arglist = 32,							 // <ARGLIST> ::= <EXPRESSION>
            @Arglist_Comma = 33,					   // <ARGLIST> ::= <ARGLIST> ',' <EXPRESSION>
            @Cast_expr_Cast_Lparan_Comma_Rparan = 34,  // <CAST_EXPR> ::= cast '(' <TYPE> ',' <EXPRESSION> ')'
            @Expression = 35,						  // <EXPRESSION> ::= <EXPRESSION_TERM>
            @Expression_Plus = 36,					 // <EXPRESSION> ::= <EXPRESSION> '+' <EXPRESSION_TERM>
            @Expression_Minus = 37,					// <EXPRESSION> ::= <EXPRESSION> '-' <EXPRESSION_TERM>
            @Expression_term = 38,					 // <EXPRESSION_TERM> ::= <EXPRESSION_FACTOR>
            @Expression_term_Times = 39,			   // <EXPRESSION_TERM> ::= <EXPRESSION_TERM> '*' <EXPRESSION_FACTOR>
            @Expression_term_Div = 40,				 // <EXPRESSION_TERM> ::= <EXPRESSION_TERM> '/' <EXPRESSION_FACTOR>
            @Expression_factor = 41,				   // <EXPRESSION_FACTOR> ::= <EXPRESSION_BINARY>
            @Expression_factor_Percent = 42,		   // <EXPRESSION_FACTOR> ::= <EXPRESSION_FACTOR> '%' <EXPRESSION_BINARY>
            @Expression_factor_Gt = 43,				// <EXPRESSION_FACTOR> ::= <EXPRESSION_FACTOR> '>' <EXPRESSION_BINARY>
            @Expression_factor_Lt = 44,				// <EXPRESSION_FACTOR> ::= <EXPRESSION_FACTOR> '<' <EXPRESSION_BINARY>
            @Expression_factor_Gteq = 45,			  // <EXPRESSION_FACTOR> ::= <EXPRESSION_FACTOR> '>=' <EXPRESSION_BINARY>
            @Expression_factor_Lteq = 46,			  // <EXPRESSION_FACTOR> ::= <EXPRESSION_FACTOR> '<=' <EXPRESSION_BINARY>
            @Expression_factor_Eqeq = 47,			  // <EXPRESSION_FACTOR> ::= <EXPRESSION_FACTOR> '==' <EXPRESSION_BINARY>
            @Expression_factor_Num = 48,			   // <EXPRESSION_FACTOR> ::= <EXPRESSION_FACTOR> '#' <EXPRESSION_BINARY>
            @Expression_binary = 49,				   // <EXPRESSION_BINARY> ::= <EXPRESSION_UNARY>
            @Expression_binary_Ampamp = 50,			// <EXPRESSION_BINARY> ::= <EXPRESSION_BINARY> '&&' <EXPRESSION_UNARY>
            @Expression_binary_Pipepipe = 51,		  // <EXPRESSION_BINARY> ::= <EXPRESSION_BINARY> '||' <EXPRESSION_UNARY>
            @Expression_unary_Plus = 52,			   // <EXPRESSION_UNARY> ::= '+' <EXPRESSION_PRIMARY>
            @Expression_unary_Minus = 53,			  // <EXPRESSION_UNARY> ::= '-' <EXPRESSION_PRIMARY>
            @Expression_unary_Exclam = 54,			 // <EXPRESSION_UNARY> ::= '!' <EXPRESSION_PRIMARY>
            @Expression_unary = 55,					// <EXPRESSION_UNARY> ::= <EXPRESSION_PRIMARY>
            @Expression_unary_Lbracket_Rbracket = 56,  // <EXPRESSION_UNARY> ::= <EXPRESSION_PRIMARY> '[' <EXPRESSION> ']'
            @Expression_unary_Lparan_Rparan = 57,	  // <EXPRESSION_UNARY> ::= <EXPRESSION_PRIMARY> '(' <ARGLIST> ')'
            @Expression_primary = 58,				  // <EXPRESSION_PRIMARY> ::= <NAME>
            @Expression_primary2 = 59,				 // <EXPRESSION_PRIMARY> ::= <FUNCTION_CALL>
            @Expression_primary3 = 60,				 // <EXPRESSION_PRIMARY> ::= <FACTOR>
            @Statements = 61,						  // <STATEMENTS> ::= <STATEMENT>
            @Statements2 = 62,						 // <STATEMENTS> ::= <STATEMENTS> <STATEMENT>
            @Statement = 63,						   // <STATEMENT> ::= <BLOCK>
            @Statement2 = 64,						  // <STATEMENT> ::= <METHOD>
            @Statement3 = 65,						  // <STATEMENT> ::= <CLASS>
            @Statement_Semi = 66,					  // <STATEMENT> ::= <FUNCTION_CALL> ';'
            @Statement_Semi2 = 67,					 // <STATEMENT> ::= <ASSIGNMENT> ';'
            @Statement_Semi3 = 68,					 // <STATEMENT> ::= <INPUTSTMT> ';'
            @Statement_Semi4 = 69,					 // <STATEMENT> ::= <OUTPUTSTMT> ';'
            @Statement_Return_Semi = 70,			   // <STATEMENT> ::= return <EXPRESSION> ';'
            @Statement_Return_Semi2 = 71,			  // <STATEMENT> ::= return ';'
            @Statement_Continue_Semi = 72,			 // <STATEMENT> ::= continue ';'
            @Statement_Break_Semi = 73,				// <STATEMENT> ::= break ';'
            @Statement4 = 74,						  // <STATEMENT> ::= <IFSTMT>
            @Statement5 = 75,						  // <STATEMENT> ::= <TRYSTMT>
            @Statement_Loop_End_Loop = 76,			 // <STATEMENT> ::= loop <STATEMENTS> end loop
            @Statement_Exit_Semi = 77,				 // <STATEMENT> ::= exit ';'
            @Statement_Throw_Semi = 78,				// <STATEMENT> ::= throw <EXPRESSION> ';'
            @Ifstmt_If_Then_End_If = 79,			   // <IFSTMT> ::= if <EXPRESSION> then <STATEMENTS> end if
            @Ifstmt_If_Then_End_If2 = 80,			  // <IFSTMT> ::= if <EXPRESSION> then <STATEMENTS> <ELSEPART> end if
            @Ifstmt_If_Then_End_If3 = 81,			  // <IFSTMT> ::= if <EXPRESSION> then <STATEMENTS> <ELSEIF_PART> <ELSEPART> end if
            @Elsepart_Else = 82,					   // <ELSEPART> ::= else <STATEMENTS>
            @Elseif_part_Elsif_Then = 83,			  // <ELSEIF_PART> ::= elsif <EXPRESSION> then <STATEMENTS>
            @Elseif_part_Elsif_Then2 = 84,			 // <ELSEIF_PART> ::= elsif <EXPRESSION> then <STATEMENTS> <ELSEIF_PART>
            @Trystmt_Try_End_Try = 85,				 // <TRYSTMT> ::= try <STATEMENTS> <CATCH_CLAUSE> end try
            @Catch_clause_Catch_Lparan_Id_Rparan = 86,  // <CATCH_CLAUSE> ::= catch '(' <TYPE> Id ')' <STATEMENTS>
            @Catch_clause_Catch_Lparan_Id_Rparan2 = 87,  // <CATCH_CLAUSE> ::= catch '(' <TYPE> Id ')' <STATEMENTS> <CATCH_CLAUSE>
            @Outputstmt_Output_Ltlt = 88,			  // <OUTPUTSTMT> ::= output '<<' <EXPRESSION>
            @Outputstmt_Output_Ltlt_Stringliteral = 89,  // <OUTPUTSTMT> ::= output '<<' StringLiteral
            @Outputstmt_Output_Ltlt_Charliteral = 90,  // <OUTPUTSTMT> ::= output '<<' CharLiteral
            @Inputstmt_Input_Gtgt = 91,				// <INPUTSTMT> ::= input '>>' <NAME>
            @Type = 92,								// <TYPE> ::= <STRUCTURE_TYPE>
            @Type2 = 93,							   // <TYPE> ::= <PRIMITIVE_TYPE>
            @Type3 = 94,							   // <TYPE> ::= <ARRAY_TYPE>
            @Primitive_type_Integer = 95,			  // <PRIMITIVE_TYPE> ::= integer
            @Primitive_type_Boolean = 96,			  // <PRIMITIVE_TYPE> ::= boolean
            @Structure_type_Id = 97,				   // <STRUCTURE_TYPE> ::= Id
            @Array_type_Lbracketrbracket = 98,		 // <ARRAY_TYPE> ::= <STRUCTURE_TYPE> '[]'
            @Array_type_Lbracketrbracket2 = 99,		// <ARRAY_TYPE> ::= <PRIMITIVE_TYPE> '[]'
            @Access_spec_Private = 100,				// <ACCESS_SPEC> ::= private
            @Access_spec_Protected = 101,			  // <ACCESS_SPEC> ::= protected
            @Access_spec_Public = 102,				 // <ACCESS_SPEC> ::= public
            @Field_decl_Semi = 103,					// <FIELD_DECL> ::= <ACCESS_SPEC> <TYPE> <FIELD_DECLLIST> ';'
            @Field_decllist_Id = 104,				  // <FIELD_DECLLIST> ::= Id
            @Field_decllist_Comma_Id = 105,			// <FIELD_DECLLIST> ::= <FIELD_DECLLIST> ',' Id
            @Method_Method_Lparan_Rparan_Is_Id = 106,  // <METHOD> ::= method <M_TYPE> <METHOD_ID> '(' <PARAMETERS> ')' is <BODY> Id
            @Method_Method_Lparan_Rparan_Is_Id2 = 107,  // <METHOD> ::= method <M_TYPE> <METHOD_ID> '(' ')' is <BODY> Id
            @Method_decl_Method_Id_Lparan_Rparan_Semi = 108,  // <METHOD_DECL> ::= <ACCESS_SPEC> method <M_TYPE> Id '(' <PARAMETER_DECL> ')' ';'
            @Method_decl_Method_Id_Lparan_Rparan_Semi2 = 109,  // <METHOD_DECL> ::= <ACCESS_SPEC> method <M_TYPE> Id '(' ')' ';'
            @Method_id_Id_Coloncolon_Id = 110,		 // <METHOD_ID> ::= Id '::' Id
            @Method_id_Id = 111,					   // <METHOD_ID> ::= Id
            @M_type = 112,							 // <M_TYPE> ::= <TYPE>
            @M_type_Void = 113,						// <M_TYPE> ::= void
            @Parameters_Id = 114,					  // <PARAMETERS> ::= <TYPE> Id
            @Parameters_Comma_Id = 115,				// <PARAMETERS> ::= <PARAMETERS> ',' <TYPE> Id
            @Parameter_decl_Id = 116,				  // <PARAMETER_DECL> ::= <TYPE> Id
            @Parameter_decl = 117,					 // <PARAMETER_DECL> ::= <TYPE>
            @Parameter_decl_Comma_Id = 118,			// <PARAMETER_DECL> ::= <PARAMETER_DECL> ',' <TYPE> Id
            @Parameter_decl_Comma = 119,			   // <PARAMETER_DECL> ::= <PARAMETER_DECL> ',' <TYPE>
            @Function_call_Call_Lparan_Rparan = 120,   // <FUNCTION_CALL> ::= call <NAME> '(' ')'
            @Function_call_Call_Lparan_Rparan2 = 121,  // <FUNCTION_CALL> ::= call <NAME> '(' <ARGLIST> ')'
            @Class_Class_Id_Is_End_Id = 122,		   // <CLASS> ::= class Id <SUPER_CLASS> is <CLASS_MEMBERLIST> end Id
            @Class_Class_Id_Is_End_Id2 = 123,		  // <CLASS> ::= class Id is <CLASS_MEMBERLIST> end Id
            @Class_memberlist = 124,				   // <CLASS_MEMBERLIST> ::= <CLASS_MEMBER>
            @Class_memberlist2 = 125,				  // <CLASS_MEMBERLIST> ::= <CLASS_MEMBERLIST> <CLASS_MEMBER>
            @Class_member = 126,					   // <CLASS_MEMBER> ::= <FIELD_DECL>
            @Class_member2 = 127,					  // <CLASS_MEMBER> ::= <METHOD_DECL>
            @Super_class_Extends_Id = 128			  // <SUPER_CLASS> ::= extends Id
        }

        public static void Apply(Production production, SyntaxStack stack)
        {
            switch (production.m_ID)
            {
                case (short)ProductionIndex.Program:
                    // <PROGRAM> ::= <CLASS>
                    break;

                case (short)ProductionIndex.Program2:
                    // <PROGRAM> ::= <METHOD>
                    StatementCollection statements = new StatementCollection();
                    statements.Add(stack.PopStatement());
                    Body program_body = new Body(statements, null);
                    stack.Push(new Module(program_body, "Main Program"));
                    break;

                case (short)ProductionIndex.Program3:
                    // <PROGRAM> ::= <PROGRAM> <CLASS>
                    break;

                case (short)ProductionIndex.Program4:
                    // <PROGRAM> ::= <PROGRAM> <METHOD>
                    break;

                case (short)ProductionIndex.Body:
                    // <BODY> ::= <SUPER_INIT> <THIS_INIT> <BLOCK>
                    break;

                case (short)ProductionIndex.Body2:
                    // <BODY> ::= <THIS_INIT> <BLOCK>
                    break;

                case (short)ProductionIndex.Body3:
                    // <BODY> ::= <SUPER_INIT> <BLOCK>
                    break;

                case (short)ProductionIndex.Body4:
                    // <BODY> ::= <BLOCK>
                    // ничего
                    break;

                case (short)ProductionIndex.This_init_This_Lparan_Rparan:
                    // <THIS_INIT> ::= this '(' <ARGLIST> ')'
                    break;

                case (short)ProductionIndex.Super_init_Super_Lparan_Rparan:
                    // <SUPER_INIT> ::= super '(' <ARGLIST> ')'
                    break;

                case (short)ProductionIndex.Block_Begin_End:
                    // <BLOCK> ::= <VARDECS> begin <STATEMENTS> end
                    stack.Remove(0);
                    stack.Remove(1);
                    Body body = new Body((StatementCollection)stack.Pop(), (VariableCollection)stack.Pop());
                    stack.Push(body);
                    break;

                case (short)ProductionIndex.Block_Begin_End2:
                    // <BLOCK> ::= begin <STATEMENTS> end
                    break;

                case (short)ProductionIndex.Vardeclist_Id_Semi:
                    // <VARDECLIST> ::= <TYPE> Id ';'
                    break;

                case (short)ProductionIndex.Vardeclist_Id_Semi2:
                    // <VARDECLIST> ::= <TYPE> Id <VAR_TYPELIST> ';'
                    // удалим из стека ";"
                    stack.Pop();
                    break;

                case (short)ProductionIndex.Var_typelist_Comma_Id:
                    // <VAR_TYPELIST> ::= ',' Id
                    // ничего не делаем, пусть накапливаются
                    break;

                case (short)ProductionIndex.Var_typelist_Comma_Id2:
                    // <VAR_TYPELIST> ::= <VAR_TYPELIST> ',' Id
                    // последняя переменная типа
                    List<object> variables = new List<object>();
                    Type var_type = null;
                    
                    object topStack = stack.Pop();

                    if (topStack is VariableCollection)
                    {
                        stack.Push(topStack);
                        break;
                    }

                    while ((string)topStack != "declare")
                    {
                        if ((string)topStack != ",")
                        {
                            variables.Add(topStack);
                        }

                        topStack = stack.Pop();

                        if (topStack is Type)
                        {
                            var_type = (Type)topStack;
                            topStack = stack.Pop();
                        }                        
                    }

                    stack.Push("declare");
                    foreach (string var_name in variables)
                    {
                        stack.Push(new Variable(null, var_name, var_type));
                    }
                    variables.Clear();
                    break;

                case (short)ProductionIndex.Vardecs_Declare:
                    // <VARDECS> ::= declare <VARDECLIST>
                    VariableCollection var_collection = new VariableCollection();
                    while (stack.Peek() is Variable)
                    {
                        var_collection.Add((Variable)stack.Pop());
                    }
                    stack.Pop(); // pop "declare"
                    stack.Push(var_collection);
                    break;

                case (short)ProductionIndex.Vardecs_Declare2:
                    // <VARDECS> ::= declare <VARDECLIST> <VARDECS>
                    break;

                case (short)ProductionIndex.Name_Id:
                    // <NAME> ::= Id
                    // ничего не делаем, идентификатор в стеке
                    break;

                case (short)ProductionIndex.Name_Dot_Id:
                    // <NAME> ::= <NAME> '.' Id
                    break;

                case (short)ProductionIndex.Assignment_Eq:
                    // <ASSIGNMENT> ::= <NAME> '=' <EXPRESSION>
                    stack.Remove(1);
                    stack.Push(new Assignment(stack.PopExpression(), null, stack.PopString()));
                    break;

                case (short)ProductionIndex.Factor_This:
                    // <FACTOR> ::= this
                    break;

                case (short)ProductionIndex.Factor_Super:
                    // <FACTOR> ::= super
                    break;

                case (short)ProductionIndex.Factor_Number:
                    // <FACTOR> ::= Number
                    stack.Push(new Literal(stack.PopString(), LiteralType.Integer));
                    break;

                case (short)ProductionIndex.Factor_False:
                    // <FACTOR> ::= false
                    break;

                case (short)ProductionIndex.Factor_True:
                    // <FACTOR> ::= true
                    break;

                case (short)ProductionIndex.Factor_Null:
                    // <FACTOR> ::= null
                    break;

                case (short)ProductionIndex.Factor:
                    // <FACTOR> ::= <ALLOCATOR>
                    break;

                case (short)ProductionIndex.Factor2:
                    // <FACTOR> ::= <CAST_EXPR>
                    break;

                case (short)ProductionIndex.Allocator_New_Lparan_Rparan:
                    // <ALLOCATOR> ::= new <TYPE> '(' <ARGLIST> ')'
                    break;

                case (short)ProductionIndex.Allocator_New_Lparan_Rparan2:
                    // <ALLOCATOR> ::= new <TYPE> '(' ')'
                    break;

                case (short)ProductionIndex.Allocator_New_Lbracket_Rbracket:
                    // <ALLOCATOR> ::= new <TYPE> '[' <EXPRESSION> ']'
                    break;

                case (short)ProductionIndex.Arglist:
                    // <ARGLIST> ::= <EXPRESSION>
                    break;

                case (short)ProductionIndex.Arglist_Comma:
                    // <ARGLIST> ::= <ARGLIST> ',' <EXPRESSION>
                    break;

                case (short)ProductionIndex.Cast_expr_Cast_Lparan_Comma_Rparan:
                    // <CAST_EXPR> ::= cast '(' <TYPE> ',' <EXPRESSION> ')'
                    break;

                case (short)ProductionIndex.Expression:
                    // <EXPRESSION> ::= <EXPRESSION_TERM>
                    // ничего
                    break;

                case (short)ProductionIndex.Expression_Plus:
                    // <EXPRESSION> ::= <EXPRESSION> '+' <EXPRESSION_TERM>
                    break;

                case (short)ProductionIndex.Expression_Minus:
                    // <EXPRESSION> ::= <EXPRESSION> '-' <EXPRESSION_TERM>
                    break;

                case (short)ProductionIndex.Expression_term:
                    // <EXPRESSION_TERM> ::= <EXPRESSION_FACTOR>
                    // ничего
                    break;

                case (short)ProductionIndex.Expression_term_Times:
                    // <EXPRESSION_TERM> ::= <EXPRESSION_TERM> '*' <EXPRESSION_FACTOR>
                    break;

                case (short)ProductionIndex.Expression_term_Div:
                    // <EXPRESSION_TERM> ::= <EXPRESSION_TERM> '/' <EXPRESSION_FACTOR>
                    break;

                case (short)ProductionIndex.Expression_factor:
                    // <EXPRESSION_FACTOR> ::= <EXPRESSION_BINARY>
                    // ничего
                    break;

                case (short)ProductionIndex.Expression_factor_Percent:
                    // <EXPRESSION_FACTOR> ::= <EXPRESSION_FACTOR> '%' <EXPRESSION_BINARY>
                    break;

                case (short)ProductionIndex.Expression_factor_Gt:
                    // <EXPRESSION_FACTOR> ::= <EXPRESSION_FACTOR> '>' <EXPRESSION_BINARY>
                    break;

                case (short)ProductionIndex.Expression_factor_Lt:
                    // <EXPRESSION_FACTOR> ::= <EXPRESSION_FACTOR> '<' <EXPRESSION_BINARY>
                    break;

                case (short)ProductionIndex.Expression_factor_Gteq:
                    // <EXPRESSION_FACTOR> ::= <EXPRESSION_FACTOR> '>=' <EXPRESSION_BINARY>
                    break;

                case (short)ProductionIndex.Expression_factor_Lteq:
                    // <EXPRESSION_FACTOR> ::= <EXPRESSION_FACTOR> '<=' <EXPRESSION_BINARY>
                    break;

                case (short)ProductionIndex.Expression_factor_Eqeq:
                    // <EXPRESSION_FACTOR> ::= <EXPRESSION_FACTOR> '==' <EXPRESSION_BINARY>
                    break;

                case (short)ProductionIndex.Expression_factor_Num:
                    // <EXPRESSION_FACTOR> ::= <EXPRESSION_FACTOR> '#' <EXPRESSION_BINARY>
                    break;

                case (short)ProductionIndex.Expression_binary:
                    // <EXPRESSION_BINARY> ::= <EXPRESSION_UNARY>
                    // ничего
                    break;

                case (short)ProductionIndex.Expression_binary_Ampamp:
                    // <EXPRESSION_BINARY> ::= <EXPRESSION_BINARY> '&&' <EXPRESSION_UNARY>
                    break;

                case (short)ProductionIndex.Expression_binary_Pipepipe:
                    // <EXPRESSION_BINARY> ::= <EXPRESSION_BINARY> '||' <EXPRESSION_UNARY>
                    break;

                case (short)ProductionIndex.Expression_unary_Plus:
                    // <EXPRESSION_UNARY> ::= '+' <EXPRESSION_PRIMARY>
                    break;

                case (short)ProductionIndex.Expression_unary_Minus:
                    // <EXPRESSION_UNARY> ::= '-' <EXPRESSION_PRIMARY>
                    break;

                case (short)ProductionIndex.Expression_unary_Exclam:
                    // <EXPRESSION_UNARY> ::= '!' <EXPRESSION_PRIMARY>
                    break;

                case (short)ProductionIndex.Expression_unary:
                    // <EXPRESSION_UNARY> ::= <EXPRESSION_PRIMARY>
                    // ничего не делаем
                    break;

                case (short)ProductionIndex.Expression_unary_Lbracket_Rbracket:
                    // <EXPRESSION_UNARY> ::= <EXPRESSION_PRIMARY> '[' <EXPRESSION> ']'
                    break;

                case (short)ProductionIndex.Expression_unary_Lparan_Rparan:
                    // <EXPRESSION_UNARY> ::= <EXPRESSION_PRIMARY> '(' <ARGLIST> ')'
                    break;

                case (short)ProductionIndex.Expression_primary:
                    // <EXPRESSION_PRIMARY> ::= <NAME>
                    stack.Push(new Name(stack.PopString()));
                    break;

                case (short)ProductionIndex.Expression_primary2:
                    // <EXPRESSION_PRIMARY> ::= <FUNCTION_CALL>
                    break;

                case (short)ProductionIndex.Expression_primary3:
                    // <EXPRESSION_PRIMARY> ::= <FACTOR>
                    // ничего не делаем
                    break;

                case (short)ProductionIndex.Statements:
                    // <STATEMENTS> ::= <STATEMENT>
                    stack.Push(new StatementCollection(stack.PopStatement()));
                    break;

                case (short)ProductionIndex.Statements2:
                    // <STATEMENTS> ::= <STATEMENTS> <STATEMENT>
                    Statement statement = stack.PopStatement();
                    ((StatementCollection)stack.Peek()).Add(statement);
                    break;

                case (short)ProductionIndex.Statement:
                    // <STATEMENT> ::= <BLOCK>
                    break;

                case (short)ProductionIndex.Statement2:
                    // <STATEMENT> ::= <METHOD>
                    break;

                case (short)ProductionIndex.Statement3:
                    // <STATEMENT> ::= <CLASS>
                    break;

                case (short)ProductionIndex.Statement_Semi:
                    // <STATEMENT> ::= <FUNCTION_CALL> ';'
                    break;

                case (short)ProductionIndex.Statement_Semi2:
                    // <STATEMENT> ::= <ASSIGNMENT> ';'
                    // удаляем ";"
                    stack.Pop(1);
                    break;

                case (short)ProductionIndex.Statement_Semi3:
                    // <STATEMENT> ::= <INPUTSTMT> ';'
                    stack.Pop(1);
                    break;

                case (short)ProductionIndex.Statement_Semi4:
                    // <STATEMENT> ::= <OUTPUTSTMT> ';'
                    stack.Pop(1);
                    break;

                case (short)ProductionIndex.Statement_Return_Semi:
                    // <STATEMENT> ::= return <EXPRESSION> ';'
                    break;

                case (short)ProductionIndex.Statement_Return_Semi2:
                    // <STATEMENT> ::= return ';'
                    break;

                case (short)ProductionIndex.Statement_Continue_Semi:
                    // <STATEMENT> ::= continue ';'
                    break;

                case (short)ProductionIndex.Statement_Break_Semi:
                    // <STATEMENT> ::= break ';'
                    break;

                case (short)ProductionIndex.Statement4:
                    // <STATEMENT> ::= <IFSTMT>
                    break;

                case (short)ProductionIndex.Statement5:
                    // <STATEMENT> ::= <TRYSTMT>
                    break;

                case (short)ProductionIndex.Statement_Loop_End_Loop:
                    // <STATEMENT> ::= loop <STATEMENTS> end loop
                    break;

                case (short)ProductionIndex.Statement_Exit_Semi:
                    // <STATEMENT> ::= exit ';'
                    break;

                case (short)ProductionIndex.Statement_Throw_Semi:
                    // <STATEMENT> ::= throw <EXPRESSION> ';'
                    break;

                case (short)ProductionIndex.Ifstmt_If_Then_End_If:
                    // <IFSTMT> ::= if <EXPRESSION> then <STATEMENTS> end if
                    break;

                case (short)ProductionIndex.Ifstmt_If_Then_End_If2:
                    // <IFSTMT> ::= if <EXPRESSION> then <STATEMENTS> <ELSEPART> end if
                    break;

                case (short)ProductionIndex.Ifstmt_If_Then_End_If3:
                    // <IFSTMT> ::= if <EXPRESSION> then <STATEMENTS> <ELSEIF_PART> <ELSEPART> end if
                    break;

                case (short)ProductionIndex.Elsepart_Else:
                    // <ELSEPART> ::= else <STATEMENTS>
                    break;

                case (short)ProductionIndex.Elseif_part_Elsif_Then:
                    // <ELSEIF_PART> ::= elsif <EXPRESSION> then <STATEMENTS>
                    break;

                case (short)ProductionIndex.Elseif_part_Elsif_Then2:
                    // <ELSEIF_PART> ::= elsif <EXPRESSION> then <STATEMENTS> <ELSEIF_PART>
                    break;

                case (short)ProductionIndex.Trystmt_Try_End_Try:
                    // <TRYSTMT> ::= try <STATEMENTS> <CATCH_CLAUSE> end try
                    break;

                case (short)ProductionIndex.Catch_clause_Catch_Lparan_Id_Rparan:
                    // <CATCH_CLAUSE> ::= catch '(' <TYPE> Id ')' <STATEMENTS>
                    break;

                case (short)ProductionIndex.Catch_clause_Catch_Lparan_Id_Rparan2:
                    // <CATCH_CLAUSE> ::= catch '(' <TYPE> Id ')' <STATEMENTS> <CATCH_CLAUSE>
                    break;

                case (short)ProductionIndex.Outputstmt_Output_Ltlt:
                    {
                        // <OUTPUTSTMT> ::= output '<<' <EXPRESSION>
                        stack.Remove(1);
                        stack.Remove(1);
                        ArgumentCollection args = new ArgumentCollection();
                        args.Add(new Argument((Name)stack.Pop(), PassMethod.ByValue));
                        Call call = new Call(args, "Write");
                        stack.Push(new CallStatement(call.Arguments, call.Name));
                    }
                    break;

                case (short)ProductionIndex.Outputstmt_Output_Ltlt_Stringliteral:
                    // <OUTPUTSTMT> ::= output '<<' StringLiteral
                    break;

                case (short)ProductionIndex.Outputstmt_Output_Ltlt_Charliteral:
                    // <OUTPUTSTMT> ::= output '<<' CharLiteral
                    break;

                case (short)ProductionIndex.Inputstmt_Input_Gtgt:
                    {
                        // <INPUTSTMT> ::= input '>>' <NAME>
                        stack.Remove(1);
                        stack.Remove(1);

                        stack.Push(new Assignment(new Call(null, "Read"), null, stack.PopString()));
                    }
                    break;
                    

                case (short)ProductionIndex.Type:
                    // <TYPE> ::= <STRUCTURE_TYPE>
                    break;

                case (short)ProductionIndex.Type2:
                    // <TYPE> ::= <PRIMITIVE_TYPE>
                    // ничего н еделаем, тип стеке
                    break;

                case (short)ProductionIndex.Type3:
                    // <TYPE> ::= <ARRAY_TYPE>
                    break;

                case (short)ProductionIndex.Primitive_type_Integer:
                    // <PRIMITIVE_TYPE> ::= integer
                    // Создаем тип объекта integer
                    stack.Pop(1);
                    stack.Push(new Type(PrimitiveType.Integer));
                    break;

                case (short)ProductionIndex.Primitive_type_Boolean:
                    // <PRIMITIVE_TYPE> ::= boolean
                    break;

                case (short)ProductionIndex.Structure_type_Id:
                    // <STRUCTURE_TYPE> ::= Id
                    break;

                case (short)ProductionIndex.Array_type_Lbracketrbracket:
                    // <ARRAY_TYPE> ::= <STRUCTURE_TYPE> '[]'
                    break;

                case (short)ProductionIndex.Array_type_Lbracketrbracket2:
                    // <ARRAY_TYPE> ::= <PRIMITIVE_TYPE> '[]'
                    break;

                case (short)ProductionIndex.Access_spec_Private:
                    // <ACCESS_SPEC> ::= private
                    break;

                case (short)ProductionIndex.Access_spec_Protected:
                    // <ACCESS_SPEC> ::= protected
                    break;

                case (short)ProductionIndex.Access_spec_Public:
                    // <ACCESS_SPEC> ::= public
                    break;

                case (short)ProductionIndex.Field_decl_Semi:
                    // <FIELD_DECL> ::= <ACCESS_SPEC> <TYPE> <FIELD_DECLLIST> ';'
                    break;

                case (short)ProductionIndex.Field_decllist_Id:
                    // <FIELD_DECLLIST> ::= Id
                    break;

                case (short)ProductionIndex.Field_decllist_Comma_Id:
                    // <FIELD_DECLLIST> ::= <FIELD_DECLLIST> ',' Id
                    break;

                case (short)ProductionIndex.Method_Method_Lparan_Rparan_Is_Id:
                    // <METHOD> ::= method <M_TYPE> <METHOD_ID> '(' <PARAMETERS> ')' is <BODY> Id
                    break;

                case (short)ProductionIndex.Method_Method_Lparan_Rparan_Is_Id2:
                    // <METHOD> ::= method <M_TYPE> <METHOD_ID> '(' ')' is <BODY> Id
                    stack.Remove(2);
                    stack.Remove(0);
                    stack.Remove(1);
                    stack.Remove(1);

                    stack.Push(new Function(stack.PopBody(), null, stack.PopString(), stack.PopType()));
                    stack.Remove(1);
                    break;

                case (short)ProductionIndex.Method_decl_Method_Id_Lparan_Rparan_Semi:
                    // <METHOD_DECL> ::= <ACCESS_SPEC> method <M_TYPE> Id '(' <PARAMETER_DECL> ')' ';'
                    break;

                case (short)ProductionIndex.Method_decl_Method_Id_Lparan_Rparan_Semi2:
                    // <METHOD_DECL> ::= <ACCESS_SPEC> method <M_TYPE> Id '(' ')' ';'
                    break;

                case (short)ProductionIndex.Method_id_Id_Coloncolon_Id:
                    // <METHOD_ID> ::= Id '::' Id
                    break;

                case (short)ProductionIndex.Method_id_Id:
                    // <METHOD_ID> ::= Id
                    // ничего не делаем, идентификатор стеке
                    break;

                case (short)ProductionIndex.M_type:
                    // <M_TYPE> ::= <TYPE>
                    break;

                case (short)ProductionIndex.M_type_Void:
                    // <M_TYPE> ::= void
                    // создаем тип объекта void
                    stack.Pop(1);
                    stack.Push(new Type(PrimitiveType.Void));
                    break;

                case (short)ProductionIndex.Parameters_Id:
                    // <PARAMETERS> ::= <TYPE> Id
                    break;

                case (short)ProductionIndex.Parameters_Comma_Id:
                    // <PARAMETERS> ::= <PARAMETERS> ',' <TYPE> Id
                    break;

                case (short)ProductionIndex.Parameter_decl_Id:
                    // <PARAMETER_DECL> ::= <TYPE> Id
                    break;

                case (short)ProductionIndex.Parameter_decl:
                    // <PARAMETER_DECL> ::= <TYPE>
                    break;

                case (short)ProductionIndex.Parameter_decl_Comma_Id:
                    // <PARAMETER_DECL> ::= <PARAMETER_DECL> ',' <TYPE> Id
                    break;

                case (short)ProductionIndex.Parameter_decl_Comma:
                    // <PARAMETER_DECL> ::= <PARAMETER_DECL> ',' <TYPE>
                    break;

                case (short)ProductionIndex.Function_call_Call_Lparan_Rparan:
                    // <FUNCTION_CALL> ::= call <NAME> '(' ')'
                    break;

                case (short)ProductionIndex.Function_call_Call_Lparan_Rparan2:
                    // <FUNCTION_CALL> ::= call <NAME> '(' <ARGLIST> ')'
                    break;

                case (short)ProductionIndex.Class_Class_Id_Is_End_Id:
                    // <CLASS> ::= class Id <SUPER_CLASS> is <CLASS_MEMBERLIST> end Id
                    break;

                case (short)ProductionIndex.Class_Class_Id_Is_End_Id2:
                    // <CLASS> ::= class Id is <CLASS_MEMBERLIST> end Id
                    break;

                case (short)ProductionIndex.Class_memberlist:
                    // <CLASS_MEMBERLIST> ::= <CLASS_MEMBER>
                    break;

                case (short)ProductionIndex.Class_memberlist2:
                    // <CLASS_MEMBERLIST> ::= <CLASS_MEMBERLIST> <CLASS_MEMBER>
                    break;

                case (short)ProductionIndex.Class_member:
                    // <CLASS_MEMBER> ::= <FIELD_DECL>
                    break;

                case (short)ProductionIndex.Class_member2:
                    // <CLASS_MEMBER> ::= <METHOD_DECL>
                    break;

                case (short)ProductionIndex.Super_class_Extends_Id:
                    // <SUPER_CLASS> ::= extends Id
                    break;

            }  //switch

            #region Старый вариант
            #region example
            /*switch (production.m_ID)
            {
                
                case 0: // <Literal> ::= boolean_literal
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
            /*switch (production.m_ID)
            {
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
                case 8:     // <ALLOCATOR> ::= new <TYPE> '(' <ARGLIST> ')'
                    break;
                case 9:     // <ALLOCATOR> ::= new <TYPE> '[' <EXPR> ']'
                    break;
                case 10:    // <ARGLIST> ::= <EXPRLIST>
                    break;
                case 11:    // <ARGLIST> ::= 
                    break;
                case 12:    // <EXPRLIST> ::= <EXPR>
                    break;
                case 13:    // <EXPRLIST> ::= <EXPR> ',' <EXPRLIST>
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
                case 20:    // <CAST_EXPR> ::= cast '(' <TYPE> ',' <EXPR> ')'
                    break;
                case 21:    // <CATCH_CLAUSE> ::= catch '(' <TYPE> Id ')' <STMTLIST>
                    break;
                case 22:    // <CEXPR> ::= <BEXPR>
                    break;
                case 23:    // <CEXPR> ::= <BEXPR> and <CEXPR>
                    break;
                case 24:    // <CLASS> ::= class Id <SUPER_CLASS> is <CLASS_MEMBERLIST> end Id
                    break;
                case 25:    // <CLASS_MEMBERLIST> ::= <CLASS_MEMBER> <CLASS_MEMBERLIST>
                    break;
                case 26:    // <CLASS_MEMBERLIST> ::= 
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
                case 41:    // <FACTOR> ::= <VALUE_OR_REF> <MEMBER_PARTLIST>
                    break;
                case 42:    // <MEMBER_PARTLIST> ::= <MEMBER_PART> <MEMBER_PARTLIST>
                    break;
                case 43:    // <MEMBER_PARTLIST> ::= 
                    // ничего не делаем
                    break;
                case 44:    // <FIELD_DECL> ::= <ACCESS_SPEC> <TYPE> Id <FIELD_DECLLIST> ';'
                    break;
                case 45:    // <FIELD_DECLLIST> ::= ',' Id <FIELD_DECLLIST>
                    break;
                case 46:    // <FIELD_DECLLIST> ::= 
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
                    // создаем тип объекта void
                    stack.Pop(1);
                    stack.Push(new Type(PrimitiveType.Void));
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
                case 75:    // <PARAMETERS> ::= <TYPE> Id <PARAMETER_TYPELIST>
                    break;
                case 76:    // <PARAMETERS> ::= 
                    // create an empty collection of parameters
                    stack.Push(new ParameterCollection());
                    break;
                case 77:    // <PARAMETER_TYPELIST> ::= ',' <TYPE> Id <PARAMETER_TYPELIST>
                    break;
                case 78:    // <PARAMETER_TYPELIST> ::=  
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
                    // ничего делать не надо
                    break;
                case 108:   // <SUPER_CLASS> ::= extends Id
                    break;
                case 109:   // <SUPER_CLASS> ::= 
                    break;
                case 110:   // <TERM> ::= <FACTOR> <TERMLIST>
                    break;
                case 111:   // <TERMLIST> ::= <MULTOP> <FACTOR> <TERMLIST>
                    break;
                case 112:   // <TERMM> ::= 
                    break;
                case 113:   // <THIS_INIT> ::= this '(' <ARGLIST> ')'
                    break;
                case 114:   // <THIS_INIT> ::= 
                    // ничего делать не надо
                    break;
                case 115:   // <TRYSTMT> ::= try <STMTLIST> <CATCH_CLAUSE> <CATCH_CLAUSEE> end try
                    break;
                case 116:   // <CATCH_CLAUSEE> ::= <CATCH_CLAUSE> <CATCH_CLAUSEE>
                    break;
                case 117:   // <CATCH_CLAUSEE> ::=  
                    break;
                case 118:   // <TYPE> ::= <Structure_Type>
                    // ничего делать не надо
                    break;
                case 119:   // <TYPE> ::= <Primitive_Type>
                    // ничего не делаем
                    break;
                case 120:   // <TYPE> ::= <Array_Type>
                    break;
                case 121:   // <Primitive_Type> ::= integer
                    // Создаем тип объекта integer
                    stack.Pop(1);
                    stack.Push(new Type(PrimitiveType.Integer));
                    break;
                case 122:   // <Primitive_Type> ::= boolean
                    break;
                case 123:   // <Structure_Type> ::= Id
                    break;
                case 124:   // <Array_Type> ::= <Structure_Type> '[]'
                    break;
                case 125:   // <Array_Type> ::= <Primitive_Type> '[]'
                    break;
                case 126:   // <VALUE_OR_REF> ::= this
                    break;
                case 127:   // <VALUE_OR_REF> ::= super
                    break;
                case 128:   // <VALUE_OR_REF> ::= Id
                    // ничего не делаем, пусть идентификатор лежит в стеке
                    break;
                case 129:   // <VALUE_OR_REF> ::= Id '[' <EXPR> ']'
                    break;
                case 130:   // <VALUE_OR_REF> ::= Id '(' <ARGLIST> ')'
                    break;
                case 131:   // <VALUE_OR_REF> ::= '(' <EXPR> ')'
                    // ничего делать не надо
                    break;
                case 132:   // <VARDECLIST> ::= <TYPE> Id <VAR_TYPELIST> ';'
                    // удалим с вершины стека ";"
                    stack.Pop();
                    break;
                case 133:   // <VAR_TYPELIST> ::= ',' Id <VAR_TYPELIST>
                    List<object> variables = new List<object>();
                    Type var_type = null;
                    
                    object topStack = stack.Pop();

                    if (topStack is VariableCollection)
                    {
                        stack.Push(topStack);
                        break;
                    }

                    while ((string)topStack != "declare")
                    {
                        if ((string)topStack != ",")
                        {
                            variables.Add(topStack);
                        }

                        topStack = stack.Pop();

                        if (topStack is Type)
                        {
                            var_type = (Type)topStack;
                            topStack = stack.Pop();
                        }                        
                    }

                    VariableCollection var_collection = new VariableCollection();
                    foreach (string var_name in variables)
                    {
                        var_collection.Add(new Variable(null, var_name, var_type));
                    }
                    stack.Push(var_collection);
                    variables.Clear();
                    break;
                case 134:   // <VAR_TYPELIST> ::= 
                    break;
                case 135:   // <VARDECS> ::= declare <VARDECLIST> <VARDECS>
                    // ничего не делаем VariableCollection могут быть много
                    break;
                case 136:   // <VARDECS> ::= 
                    // ничего не делаем
                    break;
            }
            */
            #endregion
        }
    }

}