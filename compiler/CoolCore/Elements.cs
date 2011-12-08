using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoolCore
{
    public class State
    {
        private bool m_IsAccepting;
        private Symbol m_Accepts;
        private Edge[] m_Edges;
        private int m_ID;

        /// <summary>
        /// State ID
        /// </summary>
        public int ID
        {
            get { return m_ID; }
            set { m_ID = value; }
        }

        /// <summary>
        /// State is an accepting state.
        /// </summary>
        public bool IsAccepting
        {
            get { return m_IsAccepting; }
            set { m_IsAccepting = value; }
        }

        /// <summary>
        /// The symbol the state accepts if its an accepting state.
        /// </summary>
        public Symbol Accepts
        {
            get { return m_Accepts; }
            set { m_Accepts = value; }
        }

        /// <summary>
        /// Outbound edges.
        /// </summary>
        public Edge[] Edges
        {
            get { return m_Edges; }
            set { m_Edges = value; }
        }

        /// <summary>
        /// Try to move to next state.
        /// </summary>
        public State Move(char c)
        {
            State nextState = new State();
            foreach (Edge edge in m_Edges)
            {
                nextState = edge.Move(c);
                if (nextState != null)
                    return nextState;
            }
            return null;
        }
    }

    public class Edge
    {
        private string m_Characters;
        private State m_Target;

        /// <summary>
        /// Set of characters that cause an edge transition.
        /// </summary>
        public string Characters
        {
            get { return m_Characters; }
            set { m_Characters = value; }
        }

        /// <summary>
        /// State the edge is pointing to.
        /// </summary>
        public State Target
        {
            get { return m_Target; }
            set { m_Target = value; }
        }

        /// <summary>
        /// Try to move to target state.
        /// </summary>
        /// <param name="c">Character to move on.</param>
        /// <returns>Target state or null if character doesn't exist in the character set.</returns>
        public State Move(char c)
        {
            if (m_Characters.IndexOf(c) != -1)
                return Target;
            else
                return null;
        }
    }

    public enum SymbolType
    {
        Nonterminal = 0,
        Terminal,
        Whitespace,
        /// <summary>
        /// End of source input.
        /// </summary>
        End,
        CommentStart,
        CommentEnd,
        CommentLine,
        Error
    }

    public class Symbol
    {
        protected string m_Name;
        protected SymbolType m_Type;
        protected int m_ID;

        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        public virtual SymbolType Type
        {
            get { return m_Type; }
            set { m_Type = value; }
        }

        public override string ToString()
        {
            if (m_Type == SymbolType.Terminal)
                return m_Name;
            else
                return "<" + m_Name + ">";
        }
    }

    public enum TokenType
    {
        Eof = 0,  // (EOF)
        Error = 1,  // (Error)
        Whitespace = 2,  // (Whitespace)
        Commentline = 3,  // (Comment Line)
        Identifier = 4,  // Identifier
    }

    public class Token
    {
        public Symbol Symbol;
        public string Text;
        public int Line;
        public int Column;

        public Token(Symbol symbol)
        {
            Symbol = symbol;
        }

        public override string ToString()
        {
            if (Symbol != null)
                return "Token {" + Symbol.Name + "," + Symbol.Type.ToString() + ",'" + Text + "',[" + Line + "," + Column + "]}";
            else
                return "Token {Error" + "'" + Text + "',[" + Line + "," + Column + "]}";
        }
    }

    public enum ActionType
    {
        Shift = 1,
        Reduce = 2,
        Goto = 3,
        Accept = 4
    }

    public class Action
    {
        private ActionType m_Type;
        private Symbol m_Symbol;

        private ParserState m_ParserState;
        private Production m_Production;

        public ParserState ParserState
        {
            get { return m_ParserState; }
            set { m_ParserState = value; }
        }

        public Production Production
        {
            get { return m_Production; }
            set { m_Production = value; }
        }

        public Symbol Symbol
        {
            get { return m_Symbol; }
            set { m_Symbol = value; }
        }

        public ActionType Type
        {
            get { return m_Type; }
            set { m_Type = value; }
        }

    }

    public class ParserState
    {
        public int m_ID;

        private Action[] m_Actions;

        public Action Find(Symbol symbol)
        {
            foreach (Action action in m_Actions)
            {
                if (action.Symbol == symbol)
                    return action;
            }

            return null;
        }

        public Action[] Actions
        {
            get { return m_Actions; }
            set { m_Actions = value; }
        }
    }

    public class Production
    {
        public int m_ID;
        private Symbol m_Left;
        private Symbol[] m_Right;

        /// <summary>
        /// Left hand side of the production.
        /// </summary>
        public Symbol Left
        {
            get { return m_Left; }
            set { m_Left = value; }
        }

        /// <summary>
        /// Right hand side of the production, terminals and nonterminals.
        /// </summary>
        public Symbol[] Right
        {
            get { return m_Right; }
            set { m_Right = value; }
        }

        public override string ToString()
        {
            string str = "";
            str = m_Left.ToString() + " =>";
            foreach (Symbol symbol in m_Right)
            {
                str += " " + symbol.ToString();
            }
            return str;
        }
    }
}
