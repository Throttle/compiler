using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Diagnostics;
using CoolCore;

namespace CoolCore.Compiler
{
    public delegate void ParserEventHandler(Token token, string message);

    /// <summary>
    /// Парсер
    /// </summary>
    public class Parser
    {
        // лексер
        private Scanner m_Scanner = null;
        // язык грамматики
        private Language m_Language = null;

        public event ParserEventHandler Error;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="scanner">лексер</param>
        /// <param name="language">язык</param>
        public Parser(Scanner scanner, Language language)
        {
            if (scanner == null)
                throw new ArgumentNullException("Scanner");
            if (language == null)
                throw new ArgumentNullException("Language");
            m_Scanner = scanner;
            m_Language = language;
        }

        /// <summary>
        /// Создать дерево вывода
        /// </summary>
        /// <returns>Дерево</returns>
        public ParseTreeNode CreateParseTree()
        {
            // флаг отслеживающий вхождение в блок коментариев
            bool inCommentBlock = false;

            // стек парсера
            Stack stack = new Stack();

            // стек дерева
            Stack treeStack = new Stack();

            // установка начальных значений
            m_Scanner.Reset();
            ParserState currentState = m_Language.ParserStartState;

            // запихиваем в стек начальное состояние
            stack.Push(currentState);

            while (true)
            {
                // получить следующий токен
                Symbol nextSymbol = m_Scanner.PeekNextToken().Symbol;

                // не учитываем символы разделители
                if (nextSymbol.Type == SymbolType.Whitespace)
                {
                    m_Scanner.GetNextToken();
                    continue;
                }

                // игнорируем блоки комментариев
                // вошли в блок
                if (nextSymbol.Type == SymbolType.CommentStart)
                {
                    m_Scanner.GetNextToken();
                    inCommentBlock = true;
                    continue;
                }

                // игнорируем блоки комментариев
                // вышли из блока
                if (nextSymbol.Type == SymbolType.CommentEnd)
                {
                    m_Scanner.GetNextToken();
                    inCommentBlock = false;
                    continue;
                }

                // игнорируем контент комментариев
                if (inCommentBlock)
                {
                    m_Scanner.GetNextToken();
                    continue;
                }

                // вывести содержимое стека на экран
                Print(stack);

                // получить действие в зависимости от символа
                Action action = currentState.Find(nextSymbol);

                // Если находимся в ошибочном состоянии - действеи null
                if (action == null)
                {
                    Debug.WriteLine("Error");
                    break;
                }

                // СДВИГ
                else if (action.Type == ActionType.Shift)
                {
                    stack.Push(m_Scanner.GetNextToken().Symbol);
                    stack.Push(action.ParserState);

                    // запихиваем терминал в стек дерева
                    treeStack.Push(new ParseTreeNode(nextSymbol.Name, nextSymbol, null));
                }
                // ПЕРЕХОД
                else if (action.Type == ActionType.Goto)
                {
                    Debug.WriteLine("Goto");
                }
                // СВЕРТКА
                else if (action.Type == ActionType.Reduce)
                {
                    // Pop off the stack however many state-symbol pairs as the Production
                    // has Right Terminals,Nonterminals.

                    int rightItems = action.Production.Right.Length;
                    for (int i = 0; i < rightItems; i++)
                    {
                        stack.Pop();
                        stack.Pop();
                    }

                    // получаем вершину стека
                    ParserState topState = (ParserState)stack.Peek();
                    // запихиваем левую часть правила вывода.
                    stack.Push(action.Production.Left);
                    // запихиваем состояние в которое переходим из 
                    // состояния top по символу из левой части правила вывода
                    stack.Push(topState.Find(action.Production.Left).ParserState);

                    // создаем потомков
                    ParseTreeNode[] children = new ParseTreeNode[rightItems];
                    ParseTreeNode node = new ParseTreeNode(action.Production.Left.Name, action.Production.Left, children);
                    for (int i = rightItems - 1; i >= 0; i--)
                    {
                        children[i] = (ParseTreeNode)treeStack.Pop();
                        children[i].Parent = node;
                    }
                    treeStack.Push(node);
                }
                // выдать результирующее дерево
                else if (action.Type == ActionType.Accept)
                {
                    Debug.WriteLine("Accept");
                    return (ParseTreeNode)treeStack.Pop();
                }
                currentState = (ParserState)stack.Peek();
            }

            return null;
        }


        public Module CreateSyntaxTree(string module_name="MainProgram")
        {
            // Are we currently in a comment block.
            bool inCommentBlock = false;

            // Parser stack.
            Stack stack = new Stack();

            // Syntax stack.
            SyntaxStack syntaxStack = new SyntaxStack();

            m_Scanner.Reset();
            ParserState currentState = m_Language.ParserStartState;

            // Push start state.
            stack.Push(currentState);

            while (true)
            {
                // Get next token.
                Token nextToken = m_Scanner.PeekNextToken();
                Symbol nextSymbol = nextToken.Symbol;

                // Ignore whitespace.
                if (nextSymbol.Type == SymbolType.Whitespace)
                {
                    m_Scanner.GetNextToken();
                    continue;
                }

                // Ignore comment blocks
                if (nextSymbol.Type == SymbolType.CommentStart)
                {
                    m_Scanner.GetNextToken();
                    inCommentBlock = true;
                    continue;
                }

                // Ignore comment blocks
                if (nextSymbol.Type == SymbolType.CommentEnd)
                {
                    m_Scanner.GetNextToken();
                    inCommentBlock = false;
                    continue;
                }

                // Ignore stuff inside comments
                if (inCommentBlock)
                {
                    m_Scanner.GetNextToken();
                    continue;
                }

                Print(stack);
                PrintSyntax(syntaxStack.Stack);

                // Lookup action out of current state.
                Action action = currentState.Find(nextSymbol);

                // Do we have a parser error ? (Entered an invalid state.)
                if (action == null)
                {
                    StringBuilder message = new StringBuilder("Token Unexpected, expecting [ ");

                    for (int x = 0; x < currentState.Actions.Length; x++)
                    {
                        if (currentState.Actions[x].Symbol.Type == SymbolType.Terminal)
                            message.Append(currentState.Actions[x].Symbol.Name + " ");
                    }
                    message.Append("]");

                    if (Error != null)
                        Error(nextToken, message.ToString());

                    return null;
                }

                // Should we shift token and the next state on the stack.
                else if (action.Type == ActionType.Shift)
                {
                    Token token = m_Scanner.GetNextToken();
                    stack.Push(token.Symbol);
                    syntaxStack.Push(token.Text);
                    stack.Push(action.ParserState);
                }
                // Should we reduce ?
                else if (action.Type == ActionType.Reduce)
                {
                    // Pop off the stack however many state-symbol pairs as the Production
                    // has Right Terminals,Nonterminals.

                    int rightItems = action.Production.Right.Length;
                    for (int i = 0; i < rightItems; i++)
                    {
                        stack.Pop();
                        stack.Pop();
                    }

                    // Find the top of the stack.
                    ParserState topState = (ParserState)stack.Peek();
                    // Push left hand side of the production.
                    stack.Push(action.Production.Left);
                    // Find next state by looking up the action for the top of the stack
                    // on the Left hand side symbol of the production.
                    stack.Push(topState.Find(action.Production.Left).ParserState);

                    // Apply semantic rule.
                    Semantics.Apply(action.Production, syntaxStack);

                }
                else if (action.Type == ActionType.Accept)
                {
                    Debug.WriteLine("Accept");
                    return new Module((Body)syntaxStack.Pop(), module_name);
                }
                currentState = (ParserState)stack.Peek();
            }

            return null;
        }


        private void PrintSyntax(Stack stack)
        {
            string str = "Syntax : ";
            object[] items = stack.ToArray();

            for (int i = items.Length - 1; i >= 0; i--)
            {
                str += items[i].ToString() + " ";
            }

            Debug.WriteLine(str);
        }

        private void Print(Stack stack)
        {
            string str = "Stack : ";
            object[] items = stack.ToArray();

            for (int i = items.Length - 1; i >= 0; i--)
            {
                object item = items[i];
                if (item.GetType() == typeof(ParserState))
                {
                    str += ((ParserState)item).m_ID.ToString() + " ";
                }
                else if (item.GetType() == typeof(Symbol))
                {
                    str += ((Symbol)item).Name + " ";
                }
            }

            Debug.WriteLine(str);
        }
    }

    /// <summary>
    /// Describes a node in a parse tree.
    /// </summary>
    public class ParseTreeNode
    {
        public string Name;

        public ParseTreeNode Parent = null;
        public readonly ParseTreeNode[] Children = null;
        public readonly Symbol Symbol = null;

        public ParseTreeNode(string name, Symbol symbol, ParseTreeNode[] children)
        {
            Name = name;
            Symbol = symbol;
            Children = children;
        }
    }
}
