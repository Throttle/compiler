using System;
using System.Collections;
using CoolCore.Syntax;

namespace CoolCore.Compilation
{
    public enum SymbolType
    {
        None,
        Function,
        Structure,
        Variable
    }

    public class Symbol
    {
        public string Name;
        public SymbolType Type = SymbolType.None;
        public object SyntaxObject;
        public object CodeObject;

        public Symbol(string name, SymbolType type, object syntaxObject, object codeObject)
        {
            Name = name; Type = type; SyntaxObject = syntaxObject; CodeObject = codeObject;
        }
    }

    public class SymbolTable
    {
        private SymbolTable m_Parent = null;
        private Hashtable m_Hashtable = new Hashtable();

        public SymbolTable()
        {

        }

        public SymbolTable(SymbolTable parent)
        {
            m_Parent = parent;
        }

        public Symbol Add(Variable variable)
        {
            return Add(variable.Name, SymbolType.Variable, variable, null);
        }

        public Symbol Add(Function function)
        {
            return Add(function.Name, SymbolType.Function, function, null);
        }

        public Symbol Add(Structure structure)
        {
            return Add(structure.Name, SymbolType.Structure, structure, null);
        }

        public Symbol Add(string name, SymbolType type, object syntaxObject, object codeObject)
        {
            string prefix = PrefixFromType(type);

            if (m_Hashtable.Contains(prefix + name))
                throw new Exception("Symbol already exists in symbol table.");

            Symbol symbol = new Symbol(name, type, syntaxObject, codeObject);
            m_Hashtable.Add(prefix + name, symbol);

            return symbol;
        }

        public bool Contains(string name, SymbolType type)
        {
            return Find(name, type) != null;
        }

        public Symbol Find(string name, SymbolType type)
        {
            string prefix = PrefixFromType(type);

            if (m_Hashtable.Contains(prefix + name))
                return (Symbol)m_Hashtable[prefix + name];
            else if (m_Parent != null)
            {
                return m_Parent.Find(name, type);
            }
            else
                return null;
        }

        private string PrefixFromType(SymbolType type)
        {
            if (type == SymbolType.Function)
                return "f_";
            else if (type == SymbolType.Structure)
                return "s_";
            else if (type == SymbolType.Variable)
                return "v_";
            return "";
        }
    }

    public delegate void VerifierEventHandler(string message);

    public class Verifier
    {
        public event VerifierEventHandler Error;

        private Module m_Module = null;

        public Verifier(Module module)
        {
            m_Module = module;

            //
            // Build symbol tables.
            //

            //m_Module.Body.SymbolTable = new SymbolTable();
            //BuildSymbolTable(m_Module.Body.SymbolTable,m_Module.Body);

        }

        private void BuildSymbolTable(SymbolTable parent, Body body)
        {
            if (body.Structures != null)
                foreach (Structure structure in body.Structures)
                    parent.Add(structure).SyntaxObject = structure;

            if (body.Functions != null)
            {
                foreach (Function function in body.Functions)
                {
                    parent.Add(function).SyntaxObject = function;
                    function.Body.SymbolTable = new SymbolTable();
                    BuildSymbolTable(function.Body.SymbolTable, function.Body);
                }
            }

            if (body.Statements != null)
            {
                foreach (Statement statement in body.Statements)
                {
                    if (statement is Variable)
                        parent.Add(statement as Variable).SyntaxObject = statement;
                }
            }
        }

        public bool VerifyExpression(SymbolTable symbolTable, Expression expression)
        {
            //
            // Verify expression.
            //

            try
            {
                GetExpressionType(symbolTable, expression);
                return true;
            }
            catch (VerifierException x)
            {
                System.Diagnostics.Debug.WriteLine(x.Message);
                return false;
            }
        }

        public CoolCore.Syntax.Type GetExpressionType(SymbolTable symbolTable, Expression expression)
        {
            if (expression is UnaryExpression)
            {
                UnaryExpression unary = (UnaryExpression)expression;
                return FindType(GetExpressionType(symbolTable, unary.Value), unary.UnaryOperatorType);
            }
            else if (expression is BinaryExpression)
            {
                BinaryExpression binary = (BinaryExpression)expression;
                return FindType(GetExpressionType(symbolTable, binary.Left), GetExpressionType(symbolTable, binary.Right), binary.BinaryOperatorType);
            }
            else if (expression is Literal)
            {
                Literal literal = expression as Literal;

                if (literal.LiteralType == LiteralType.Boolean)
                    return new CoolCore.Syntax.Type(PrimitiveType.Boolean);
                if (literal.LiteralType == LiteralType.Character)
                    return new CoolCore.Syntax.Type(PrimitiveType.Character);
                if (literal.LiteralType == LiteralType.Integer)
                    return new CoolCore.Syntax.Type(PrimitiveType.Integer);
                if (literal.LiteralType == LiteralType.Real)
                    return new CoolCore.Syntax.Type(PrimitiveType.Real);
                if (literal.LiteralType == LiteralType.String)
                    return new CoolCore.Syntax.Type(PrimitiveType.Void);
            }
            else if (expression is Name)
            {
                return ((Variable)symbolTable.Find(((Name)expression).Value, SymbolType.Variable).SyntaxObject).Type;
            }
            else if (expression is Call)
            {
                return ((Call)expression).Type;
            }
            return null;
        }

        public CoolCore.Syntax.Type FindType(CoolCore.Syntax.Type leftType, CoolCore.Syntax.Type rightType, BinaryOperatorType operatorType)
        {
            //
            // Binary operations can only be performed on primitive types.
            //

            if ((leftType.VariableType != VariableType.Primitive) || (rightType.VariableType != VariableType.Primitive))
                throw new VerifierException("Binary operations can only be performed on primitive types.");

            //
            // Boolean operations.
            //

            if (leftType.PrimitiveType == PrimitiveType.Boolean && rightType.PrimitiveType == PrimitiveType.Boolean)
            {
                switch (operatorType)
                {
                    case BinaryOperatorType.And:
                    case BinaryOperatorType.Or:
                    case BinaryOperatorType.NotEqual:
                        return new CoolCore.Syntax.Type(PrimitiveType.Boolean);
                        break;
                    default:
                        throw new VerifierException("Specified operator cannot be applied to boolean types.");
                        break;
                }
            }

            //
            // Integer operations.
            //

            else if (leftType.PrimitiveType == PrimitiveType.Integer && rightType.PrimitiveType == PrimitiveType.Integer)
            {
                switch (operatorType)
                {
                    case BinaryOperatorType.And:
                    case BinaryOperatorType.Or:
                        throw new VerifierException("Specified operator cannot be applied to integer types.");
                        break;
                    case BinaryOperatorType.GraterOrEqualTo:
                    case BinaryOperatorType.GreaterThen:
                    case BinaryOperatorType.LessOrEqualTo:
                    case BinaryOperatorType.LessThen:
                    case BinaryOperatorType.Equal:
                    case BinaryOperatorType.NotEqual:
                        return new CoolCore.Syntax.Type(PrimitiveType.Boolean);
                        break;
                    default:
                        return new CoolCore.Syntax.Type(PrimitiveType.Integer);
                }
            }

            //
            // Real operations.
            //

            else if (leftType.PrimitiveType == PrimitiveType.Real && rightType.PrimitiveType == PrimitiveType.Real)
            {
                switch (operatorType)
                {
                    case BinaryOperatorType.And:
                    case BinaryOperatorType.Or:
                        throw new VerifierException("Specified operator cannot be applied to real types.");
                        break;
                    default:
                        return new CoolCore.Syntax.Type(PrimitiveType.Real);
                }
            }

            //
            // Character operations.
            //

            else if (leftType.PrimitiveType == PrimitiveType.Character && rightType.PrimitiveType == PrimitiveType.Character)
            {
                switch (operatorType)
                {
                    case BinaryOperatorType.And:
                    case BinaryOperatorType.Or:
                        throw new VerifierException("Specified operator cannot be applied to character types.");
                        break;
                    default:
                        return new CoolCore.Syntax.Type(PrimitiveType.Character);
                }
            }

            throw new VerifierException("Incompatible types for specified operation.");
        }

        public CoolCore.Syntax.Type FindType(CoolCore.Syntax.Type type, UnaryOperatorType operatorType)
        {
            switch (operatorType)
            {
                case UnaryOperatorType.Indexer:
                    if (type.VariableType == VariableType.PrimitiveArray)
                        return new CoolCore.Syntax.Type(type.PrimitiveType);
                    else if (type.VariableType == VariableType.StructureArray)
                        return new CoolCore.Syntax.Type(type.Name);
                    else
                        throw new VerifierException("The indexer operator cannot be applied to the specified type.");
                    break;
                case UnaryOperatorType.Not:
                    if (type.PrimitiveType == PrimitiveType.Boolean)
                        return new CoolCore.Syntax.Type(type.PrimitiveType);
                    else
                        throw new VerifierException("The NOT operator cannot be applied to the specified type.");
                    break;
                default:
                    if (type.VariableType == VariableType.Primitive)
                        return new CoolCore.Syntax.Type(type.PrimitiveType);
                    else
                        throw new VerifierException("The +/- operators cannot be applied to the specified type.");
                    break;
            }
        }


        public bool VerifyBody(Body body)
        {
            //
            // Check variable declarations.
            //

            foreach (Statement statement in body.Statements)
            {
                if (statement is Variable)
                {
                    Variable variable = (Variable)statement;
                    if (variable.Value is Expression)
                    {
                        if (VerifyExpression(body.SymbolTable, (Expression)variable.Value))
                            System.Diagnostics.Debug.WriteLine("Fuck yah");
                        else
                            System.Diagnostics.Debug.WriteLine("Fuck nahh");
                    }
                }
            }

            return false;
        }




    }
}
