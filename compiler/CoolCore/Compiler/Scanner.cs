using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CoolCore.Compiler
{
    /// <summary>
    /// Класс реализующий работу лексера
    /// </summary>
    public class Scanner
    {
        // путь к файлу с исходным кодом
        private string m_Path;
        // буффер куда производится чтение
        private char[] m_Buffer;
        // позиция в буффере
        private int m_Cursor = -1;
        // язык грамматики
        private Language m_Language;
        // текущий столбец и строка
        private int m_Line = 0, m_Column = 0;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="path">Полный путь к файлу с исходным кодом</param>
        /// <param name="language">Язык используемой грамматики</param>
        public Scanner(string path, Language language)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException();
            if (language == null)
                throw new ArgumentNullException("Language", "Параметр должен быть не null");
            m_Path = path;
            m_Language = language;
            StreamReader m_Reader = File.OpenText(path);
            m_Buffer = m_Reader.ReadToEnd().ToCharArray();
            m_Reader.Close();
            Reset();
        }

        /// <summary>
        /// Получить следующий токен.
        /// </summary>
        /// <returns>Токен</returns>
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

            // Производится посимвольное чтение входного файла и одновременный проход по КА
            // Если в результате прохода обнаружилась выводимая цепочка терминалов (lastAcceptingState)
            // сохраняем её. Таким образом получаем самую длинную выводимую цепочку.
            // Если приходим в состояние ошибки (не можем никуда перейти) то lastAcceptingState - следующий токен
            // иначе вернуть null

            while (true)
            {
                // не передвигаем курсор
                char nextChar = PeekNextChar();

                // Если обнаружен конец файла EOF.
                if (nextChar == (char)0 && (lastAcceptingState == null))
                {
                    result = new Token(m_Language.Symbols[0]);
                    result.Column = tokenStartColumn;
                    result.Line = tokenStartLine;
                    break;
                }
                

                // Получить следующее состояние из текущего по входному символу nextChar.
                State nextState = currentState.Move(nextChar);
                
                // Если следующее состояние существует (нет ошибки).
                if (nextState != null)
                {
                    // Если выводится цепочка терминалов - запоминаем
                    if (nextState.IsAccepting)
                    {
                        lastAcceptingState = nextState;
                        tokenEnd = m_Cursor + 2; // так как юникод
                    }

                    // Переходим в следующее состояние
                    currentState = nextState;
                    // Передвигаем курсор
                    nextChar = GetNextChar();
                }
                else
                {
                    // Находимся в состоянии ошибки. 
                    // Если определен lastAcceptingState - возвращаем его
                    // иначе ошибка (null)

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
        /// Сброс сканера в начальное положение
        /// </summary>
        public void Reset()
        {
            m_Cursor = -1;
            m_Line = m_Column = 1;
        }

        /// <summary>
        /// Получить символ на позиции index из входного буфера
        /// </summary>
        /// <param name="index">позиция символа в буфере</param>
        /// <returns>символ из входного файла, либо 0 в случае ошибки</returns>
        private char GetChar(int index)
        {
            return (index >= m_Buffer.Length) || (index < 0) ? (char)0 : m_Buffer[index];
        }


        /// <summary>
        /// Прочитать следующий значимый символ из входного потока
        /// Курсор передвигается
        /// </summary>
        /// <returns>символ</returns>
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

        /// <summary>
        /// Предпросмотр следующего символа, без прочтения (курсор на месте)
        /// </summary>
        /// <returns>Следующий символ</returns>
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
