using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CoolCore.Compiler
{
    public class Scanner
    {
        private string m_Path;
        private char[] m_Buffer;
        private int m_Cursor = -1;
        private Language m_Language;
        private int m_Line = 0, m_Column = 0;

        public Scanner(string path, Language language)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException();
            if (language == null)
                throw new ArgumentNullException("Language");
            m_Path = path;
            m_Language = language;
            StreamReader m_Reader = File.OpenText(path);
            m_Buffer = m_Reader.ReadToEnd().ToCharArray();
            m_Reader.Close();

            Reset();
        }

        /// <summary>
        /// Peek at the next token.
        /// </summary>
        /// <returns>The next token found.</returns>
        public Token PeekNextToken()
        {
            int save = m_Cursor;
            int saveColumn = m_Column;
            int saveLine = m_Line;
            Token token = GetNextToken();
            m_Cursor = save;
            m_Column = saveColumn;
            m_Line = saveLine;
            return token;
        }

        /// <summary>
        /// Получить следующий токен
        /// </summary>
        /// <returns>Следующий в потоке токен</returns>
        public Token GetNextToken()
        {
            State currentState = m_Language.StartState;
            State lastAcceptingState = null;
            int tokenStart = m_Cursor + 1;
            int tokenEnd = tokenStart;

            int tokenStartColumn = m_Column;
            int tokenStartLine = m_Line;

            Token result = null;

            //
            // Retrieve one character at a time from the source input and walk through the DFA.
            // when we enter an accepting state save it as the lastAcceptingState and keep walking.
            // If we enter an error state (nextState == null) then return the lastAcceptingState, or
            // a null token if the lastAcceptingState is never set.
            //

            while (true)
            {
                // Don't advance the cursor.
                char nextChar = PeekNextChar();

                // Return an EOF token.
                if (nextChar == (char)0 && (lastAcceptingState == null))
                {
                    result = new Token(m_Language.Symbols[0]);
                    result.Column = tokenStartColumn;
                    result.Line = tokenStartLine;
                    break;
                }
                

                // Get next state from current state on the next character.
                State nextState = currentState.Move(nextChar);
                // If the next state is not an error state move to the next state.
                if (nextState != null)
                {
                    // Save accepting state if its accepting.
                    if (nextState.IsAccepting)
                    {
                        lastAcceptingState = nextState;
                        tokenEnd = m_Cursor + 2;
                    }
                    // Move to the next state.
                    currentState = nextState;
                    // Advance cursor.
                    nextChar = GetNextChar();
                }
                else
                {
                    // We have entered an error state. Thus either return the lastAcceptingState or
                    // a null token.
                    if (lastAcceptingState == null)
                    {
                        result = new Token(null);
                        result.Column = tokenStartColumn;
                        result.Line = tokenStartLine;
                        result.Text = new string(m_Buffer, tokenStart, tokenEnd - tokenStart);
                    }
                    else
                    {
                        result = new Token(lastAcceptingState.Accepts);
                        result.Column = tokenStartColumn;
                        result.Line = tokenStartLine;
                        result.Text = new string(m_Buffer, tokenStart, tokenEnd - tokenStart);
                    }
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// Resets the scanner.
        /// </summary>
        public void Reset()
        {
            m_Cursor = -1;
            m_Line = m_Column = 1;
        }

        private char GetChar(int index)
        {
            return (index >= m_Buffer.Length) || (index < 0) ? (char)0 : m_Buffer[index];
        }

        private char GetNextChar()
        {
            char nextChar = GetChar(++m_Cursor);

            if ((nextChar == (char)13) && (PeekNextChar() == (char)10))
            {
                m_Line++;
                m_Column = 0;
            }
            else
                m_Column++;

            return nextChar;
        }

        private char PeekNextChar()
        {
            return GetChar(m_Cursor + 1);
        }

        private char GetCurrentChar()
        {
            return GetChar(m_Cursor);
        }

    }
}
