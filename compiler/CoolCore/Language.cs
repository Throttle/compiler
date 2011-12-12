using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;


namespace CoolCore
{
    /// <summary>
    /// Класс описывающий язык грамматики
    /// </summary>
    public class Language
    {
        ///< Состояния
        private State[] m_States = null;
		///< Правила
		private Production[] m_Productions = null;
		///< Состояние парсеров
		private ParserState [] m_ParserStates = null;
		///< Символы
		private Symbol[] m_Symbols = null;
        ///< Начальное состояние
		private State m_StartState = null;
		///< Начальное состояние парсера
		private ParserState m_ParserStartState = null;

        /// <summary>
        /// Загрузка грамматики и языка из файла
        /// </summary>
        /// <param name="path">Полный путь к файлу *.egt</param>
        /// <returns>Объект Language</returns>
		public static Language FromFile(string path)
		{
			Language language = new Language();
            
            ///< загружаем грамматику
            Grammar grammar = Grammar.FromFile(path);
		
			//
			// Создаем таблицу символов
			//
            language.m_Symbols = new Symbol[grammar.SymbolTable.Length];
			for(int i = 0; i < grammar.SymbolTable.Length; i++)
			{
				Symbol newSymbol = new Symbol();
				newSymbol.Name = grammar.SymbolTable[i].Name;
				newSymbol.Type = (SymbolType)grammar.SymbolTable[i].Kind;
				language.m_Symbols[i] = newSymbol;
			}


			//
			// Инициализируем состояния КА
			//
            language.m_States = new State[grammar.DFATable.Length];
			for(int i = 0; i < grammar.DFATable.Length; i++)
			{
				language.m_States[i] = new State();
				language.m_States[i].ID = i;
			}

			//
			// Создаем переходы (ребра).
			//
			for(int i = 0; i < grammar.DFATable.Length; i++)
			{
				// Дополняем информацию о состояниях.
				language.m_States[i].IsAccepting = grammar.DFATable[i].IsAccepting;
				if(language.m_States[i].IsAccepting)
					language.m_States[i].Accepts = language.m_Symbols[grammar.DFATable[i].AcceptIndex];
			
				// Создаем ребра.
				language.m_States[i].Edges = new Edge[grammar.DFATable[i].Edges.Length];
				for(int j = 0; j < grammar.DFATable[i].Edges.Length; j++)
				{
					Edge newEdge = new Edge();
					newEdge.Characters = grammar.CharacterSetTable[grammar.DFATable[i].Edges[j].CharacterSetIndex].Characters;
					newEdge.Target = language.m_States[grammar.DFATable[i].Edges[j].TargetIndex];
					language.m_States[i].Edges[j] = newEdge;
				}
		
			}

			//
			// Создаем правила.
			//
            language.m_Productions = new Production[grammar.RuleTable.Length];
			for(int i = 0;i<grammar.RuleTable.Length;i++)
			{
				Production production = new Production();
				production.m_ID = i;
				production.Left = language.m_Symbols[grammar.RuleTable[i].Nonterminal];
				production.Right = new Symbol[grammar.RuleTable[i].Symbols.Length];
				for(int j = 0; j < grammar.RuleTable[i].Symbols.Length;j++)
				{
					production.Right[j] = language.m_Symbols[grammar.RuleTable[i].Symbols[j]];
				}
				language.m_Productions[i] = production ;
			}

			//
			// Создаем состояния парсера (на основе состояний LALR).
			//
            language.m_ParserStates = new ParserState[grammar.LALRTable.Length];

            for (int i = 0; i < grammar.LALRTable.Length; i++)
                language.m_ParserStates[i] = new ParserState();

			for(int i = 0;i<grammar.LALRTable.Length;i++)
			{
				ParserState state = language.m_ParserStates[i];
                
 				state.Actions = new Action[grammar.LALRTable[i].Actions.Length];
				for(int j = 0; j < grammar.LALRTable[i].Actions.Length; j++)
				{
					Action action = new Action();
					action.Symbol = language.m_Symbols[grammar.LALRTable[i].Actions[j].SymbolIndex];
					action.Type = (ActionType)grammar.LALRTable[i].Actions[j].ActionType;
					if((action.Type == ActionType.Shift) || (action.Type == ActionType.Goto))
						action.ParserState = language.m_ParserStates[grammar.LALRTable[i].Actions[j].Target];
					else if(action.Type == ActionType.Reduce)
						action.Production = language.m_Productions[grammar.LALRTable[i].Actions[j].Target];
					state.Actions[j] = action;
				}

				state.m_ID = i;
			}


			language.m_StartState = language.m_States[grammar.InitialDFAState];
			language.m_ParserStartState = language.m_ParserStates[grammar.InitialLALRState];
	
			return language;
		}

        /// <summary>
        /// Массив состояний КА
        /// </summary>
		public State[] States
		{
			get{return m_States;}
		}

        /// <summary>
        /// Массив состояний парсера
        /// </summary>
		public ParserState[] ParserStates
		{
			get{return m_ParserStates;}
		}

        /// <summary>
        /// Массив правил
        /// </summary>
		public Production[] Productions
		{
			get{return m_Productions;}
		}

        /// <summary>
        /// Массив символов
        /// </summary>
        public Symbol[] Symbols
		{
			get{return m_Symbols;}
		}

        /// <summary>
        /// Начальное состояние КА
        /// </summary>
		public State StartState
		{
			get{return m_StartState;}
		}

        /// <summary>
        /// Начальное состояние парсера
        /// </summary>
		public ParserState ParserStartState
		{
			get{return m_ParserStartState;}
		}

	    /// <summary>
	    /// 
	    /// </summary>
	    /// <param name="production"></param>
	    /// <param name="syntaxStack"></param>
		public void ApplySemantic(Production production, Stack syntaxStack)
		{
			switch(production.m_ID)
			{
				case 0:
					break;
			}

		}

	}

    /// <summary>
    /// Класс описывающий грамматику
    /// </summary>
	public class Grammar
	{
        /// <summary>
        /// Класс описывающий наборы символов
        /// </summary>
		internal class CharacterSet
		{
            // Индекс в таблице
			public short Index;
            // строка символов
			public string Characters;
		}

        /// <summary>
        /// Символ грамматики
        /// </summary>
		internal class Symbol
		{
            // индекс в таблице
			public short Index;
            // словесное описание символа
			public string Name;
            // тип сивола
			public short Kind;
		}

        /// <summary>
        /// Правило вывода
        /// </summary>
		internal class Rule
		{
            // индекс в таблице
			public short Index;
            // исходный нетерминал (индекс в таблице символов)
			public short Nonterminal;
            // выходная цепочка
			public short [] Symbols = null;
		}

        /// <summary>
        /// Ребро
        /// </summary>
		internal class Edge
		{
			public short CharacterSetIndex;
			public short TargetIndex;
		}

        /// <summary>
        /// Состояние конечного автомата
        /// </summary>
		internal class DFA
		{
            // индекс в таблице
			public short Index;
            // если выводится терминальный символ
			public bool IsAccepting;
            // индекс терминального символа
			public short AcceptIndex;
            // ребра ИЗ данного состояния
			public Edge [] Edges;
		}

        /// <summary>
        /// Действие
        /// </summary>
		internal class Action
		{
            // индекс начального символа
			public short SymbolIndex;
            // тип перехода
			public short ActionType;
            // индекс конечного символа
			public short Target;
		}

        /// <summary>
        /// Состояние LALR анализатора 
        /// </summary>
		internal class LALR
		{
            // индекс в таблице
			public short Index;
            // набор возможных действий
			public Action [] Actions;
		}

        // объекты для чтения грамматики
		private static BinaryReader m_Reader = null;
		private static Encoding m_Encoding = null;

        /// <summary>
        /// Получить грамматику из файла
        /// </summary>
        /// <param name="path">Полный путь к файлу</param>
        /// <returns>Объект Грамматики</returns>
		public static Grammar FromFile(string path)
		{
            // определяем кодировку
			m_Encoding = new UnicodeEncoding(false, true);
			m_Reader = new BinaryReader(new FileStream(path, FileMode.Open));

            // Header == Заголовок 
			if(ReadString() != "GOLD Parser Tables/v5.0")
			{
				m_Reader.Close();
				throw new Exception("Invalid file format.");
			}

            // инициализируем объект грамматики
			Grammar grammar = new Grammar();

            // начало чтения записей файла грамматики
            #region РАЗБОР ФАЙЛА ГРАММАТИКИ EGT-формат
            try
			{
                byte read_byte = m_Reader.ReadByte();
                char rbch = (char)read_byte;
				while(read_byte == (byte)'M')
				{
				
					Int16 entries = m_Reader.ReadInt16();

					//
					// Read entry
					//             

                    read_byte = m_Reader.ReadByte();
                    rbch = (char)read_byte;

                    read_byte = m_Reader.ReadByte(); 
                    rbch = (char)read_byte;

                    if (read_byte == (byte)'p')
                    {
                        // name
                        m_Reader.ReadBytes(3);          // index
                        m_Reader.ReadByte();            // string prefix (83)
                        ReadString();                   // property name
                        m_Reader.ReadByte();            // string prefix (83)
                        grammar.Name = ReadString();    // property value
                            
                        // version
                        m_Reader.ReadByte();            // new record ('M')
                        m_Reader.ReadBytes(2);          // number of entities
                        m_Reader.ReadByte();            // byte separator
                        m_Reader.ReadByte();            // property tag ('p')
                        m_Reader.ReadBytes(3);          // index
                        m_Reader.ReadByte();            // string prefix (83)
                        ReadString();                   // property name
                        m_Reader.ReadByte();            // string prefix (83)
                        grammar.Version = ReadString(); // property value

                        // author
                        m_Reader.ReadByte();            // new record ('M')
                        m_Reader.ReadBytes(2);          // number of entities
                        m_Reader.ReadByte();            // byte separator
                        m_Reader.ReadByte();            // property tag ('p')
                        m_Reader.ReadBytes(3);          // index
                        m_Reader.ReadByte();            // string prefix (83)
                        ReadString();                   // property name
                        m_Reader.ReadByte();            // string prefix (83)
                        grammar.Author = ReadString(); // property value

                        // about
                        m_Reader.ReadByte();            // new record ('M')
                        m_Reader.ReadBytes(2);          // number of entities
                        m_Reader.ReadByte();            // byte separator
                        m_Reader.ReadByte();            // property tag ('p')
                        m_Reader.ReadBytes(3);          // index
                        m_Reader.ReadByte();            // string prefix (83)
                        ReadString();                   // property name
                        m_Reader.ReadByte();            // string prefix (83)
                        grammar.About = ReadString(); // property value

                        // character set
                        m_Reader.ReadByte();            // new record ('M')
                        m_Reader.ReadBytes(2);          // number of entities
                        m_Reader.ReadByte();            // byte separator
                        m_Reader.ReadByte();            // property tag ('p')
                        m_Reader.ReadBytes(3);          // index
                        m_Reader.ReadByte();            // string prefix (83)
                        ReadString();                   // property name
                        m_Reader.ReadByte();            // string prefix (83)
                        //grammar. = ReadString(); // property value
                        string character_set = ReadString();

                        // character mapping
                        m_Reader.ReadByte();            // new record ('M')
                        m_Reader.ReadBytes(2);          // number of entities
                        m_Reader.ReadByte();            // byte separator
                        m_Reader.ReadByte();            // property tag ('p')
                        m_Reader.ReadBytes(3);          // index
                        m_Reader.ReadByte();            // string prefix (83)
                        ReadString();                   // property name
                        m_Reader.ReadByte();            // string prefix (83)
                        //grammar. = ReadString(); // property value
                        string character_mapping = ReadString();


                        // generated by
                        m_Reader.ReadByte();            // new record ('M')
                        m_Reader.ReadBytes(2);          // number of entities
                        m_Reader.ReadByte();            // byte separator
                        m_Reader.ReadByte();            // property tag ('p')
                        m_Reader.ReadBytes(3);          // index
                        m_Reader.ReadByte();            // string prefix (83)
                        ReadString();                   // property name
                        m_Reader.ReadByte();            // string prefix (83)
                        //grammar. = ReadString(); // property value
                        string generated_by = ReadString();


                        // generation date
                        m_Reader.ReadByte();            // new record ('M')
                        m_Reader.ReadBytes(2);          // number of entities
                        m_Reader.ReadByte();            // byte separator
                        m_Reader.ReadByte();            // property tag ('p')
                        m_Reader.ReadBytes(3);          // index
                        m_Reader.ReadByte();            // string prefix (83)
                        ReadString();                   // property name
                        m_Reader.ReadByte();            // string prefix (83)
                        string generated_date = ReadString();
                        grammar.IsCaseSensitive = true;
                        read_byte = m_Reader.ReadByte();
                        rbch = (char)read_byte; 
                    }
                    else if (read_byte == (byte)'t')
                    {
                        ///
                        /// Инициализация рабочих таблиц
                        ///

                        // таблица символов
                        m_Reader.ReadByte();
                        grammar.SymbolTable = new Symbol[m_Reader.ReadInt16()];

                        // таблица групп сиволов
                        m_Reader.ReadByte();
                        grammar.CharacterSetTable = new CharacterSet[m_Reader.ReadInt16()];

                        // таблица правил
                        m_Reader.ReadByte();
                        grammar.RuleTable = new Rule[m_Reader.ReadInt16()];

                        // таблица состояний КА
                        m_Reader.ReadByte();
                        grammar.DFATable = new DFA[m_Reader.ReadInt16()];

                        // таблица состояний LALR анализатора
                        m_Reader.ReadByte();
                        grammar.LALRTable = new LALR[m_Reader.ReadInt16()];

                        // таблица ролей символов (групп по ролям)
                        m_Reader.ReadByte();
                        int[]  group_table = new int[m_Reader.ReadInt16()];

                        read_byte = m_Reader.ReadByte();
                        rbch = (char)read_byte; 
                    }
                    else if (read_byte == (byte)'c')
                    {
                        ///
                        /// Определение таблицы наборов символов
                        ///
                        CharacterSet item = new CharacterSet();

                        int temp = -1;
                        rbch = (char)m_Reader.ReadByte();
                        item.Index = m_Reader.ReadInt16();
                            
                        rbch = (char)m_Reader.ReadByte();
                        temp = m_Reader.ReadInt16();        // Codepage
                        m_Reader.ReadByte();                // Range
                        int total = m_Reader.ReadInt16();
                        rbch = (char)m_Reader.ReadByte();   // Reserved

                        string char_str = string.Empty;
                        while (true)
                        {
                                
                            read_byte = m_Reader.ReadByte();
                            if (read_byte == 73)
                            {
                                int start_index = m_Reader.ReadInt16();
                                    
                                m_Reader.ReadByte();
                                int end_index = m_Reader.ReadInt16();
                                                                        
                                for (int ci = Math.Min(start_index, end_index); ci <= Math.Max(start_index, end_index); ci++)
                                {
                                    char_str += Convert.ToChar(ci);
                                }
                            }
                            else
                            {
                                rbch = (char)read_byte; 
                                break;
                            }
                        }
                        item.Characters = char_str;
                        grammar.CharacterSetTable[item.Index] = item;
                            

                    }
                    else if (read_byte == (byte)'g')
                    {
                        /// 
                        /// Определение таблицы групп символов
                        ///
                        m_Reader.ReadByte();
                        int index = m_Reader.ReadInt16();
                        m_Reader.ReadByte();
                        string name = ReadString();
                        m_Reader.ReadByte();
                        int container_index = m_Reader.ReadInt16();
                        m_Reader.ReadByte();
                        int start_index = m_Reader.ReadInt16(); 
                        m_Reader.ReadByte();
                        int end_index = m_Reader.ReadInt16();
                        m_Reader.ReadByte();
                        int advanced_mode = m_Reader.ReadInt16();
                        m_Reader.ReadByte();
                        int ending_mode = m_Reader.ReadInt16();
                        m_Reader.ReadByte();
                        while (true)
                        {
                            read_byte = m_Reader.ReadByte();
                            if (read_byte == 73)
                            {
                                int group_index = m_Reader.ReadInt16();
                            }
                            else
                            {
                                rbch = (char)read_byte;
                                break;
                            }
                        }

                    }
                    else if (read_byte == (byte)'R')
                    {
                        ///
                        /// Определение таблицы правил
                        ///
                        Rule item = new Rule();

                        m_Reader.ReadByte();
                        item.Index = m_Reader.ReadInt16();

                        m_Reader.ReadByte();
                        item.Nonterminal = m_Reader.ReadInt16();

                        m_Reader.ReadByte();

                        // читаем индексы символов
                        item.Symbols = new short[entries - 4];
                        for (int x = 0; x < item.Symbols.Length; x++)
                        {
                            m_Reader.ReadByte();
                            item.Symbols[x] = m_Reader.ReadInt16();
                        }

                        grammar.RuleTable[item.Index] = item;

                        read_byte = m_Reader.ReadByte();
                        rbch = (char)read_byte; 
                    }
                    else if (read_byte == (byte)'S')
                    {
                        ///
                        /// Определение таблицы символов
                        ///
                        Symbol item = new Symbol();

                        m_Reader.ReadByte();
                        item.Index = m_Reader.ReadInt16();

                        m_Reader.ReadByte();
                        item.Name = ReadString();

                        m_Reader.ReadByte();
                        item.Kind = m_Reader.ReadInt16();

                        grammar.SymbolTable[item.Index] = item;
                        read_byte = m_Reader.ReadByte();
                        rbch = (char)read_byte; 
                    }
                    else if (read_byte == (byte)'D')
                    {
                        ///
                        /// Определение таблицы состояний КА
                        ///

                        DFA item = new DFA();

                        m_Reader.ReadByte();
                        item.Index = m_Reader.ReadInt16();

                        m_Reader.ReadByte();
                        item.IsAccepting = !(m_Reader.ReadByte() == 0);

                        m_Reader.ReadByte();
                        item.AcceptIndex = m_Reader.ReadInt16();

                        m_Reader.ReadByte();

                        // читаем набор ребер
                        item.Edges = new Edge[(entries - 5) / 3];
                        for (int x = 0; x < item.Edges.Length; x++)
                        {
                            Edge edge = new Edge();

                            m_Reader.ReadByte();
                            edge.CharacterSetIndex = m_Reader.ReadInt16();

                            m_Reader.ReadByte();
                            edge.TargetIndex = m_Reader.ReadInt16();

                            m_Reader.ReadByte();

                            item.Edges[x] = edge;
                        }

                        grammar.DFATable[item.Index] = item;
                        read_byte = m_Reader.ReadByte();
                        rbch = (char)read_byte; 
                    }
                    else if (read_byte == (byte)'L')
                    {
                        ///
                        /// Определение состояний LALR анализатора
                        ///
                        LALR item = new LALR();

                        m_Reader.ReadByte();
                        item.Index = m_Reader.ReadInt16();

                        m_Reader.ReadByte();

                        // Читаем действия
                        item.Actions = new Action[(entries - 3) / 4];

                        for (int x = 0; x < item.Actions.Length; x++)
                        {
                            Action action = new Action();

                            m_Reader.ReadByte();
                            action.SymbolIndex = m_Reader.ReadInt16();

                            m_Reader.ReadByte();
                            action.ActionType = m_Reader.ReadInt16();

                            m_Reader.ReadByte();
                            action.Target = m_Reader.ReadInt16();

                            m_Reader.ReadByte();

                            item.Actions[x] = action;
                        }

                        grammar.LALRTable[item.Index] = item;
                        read_byte = NextByte();
                    }
                    else if (read_byte == (byte)'I')
                    {
                        ///
                        /// Определение начальных состояний
                        ///
                        m_Reader.ReadByte();
                        grammar.InitialDFAState = m_Reader.ReadInt16();
                        m_Reader.ReadByte();
                        grammar.InitialLALRState = m_Reader.ReadInt16();
                        read_byte = NextByte();
                    }                                           
                }
			}
			catch(Exception x)
			{
                return null;
            }
            #endregion

            return grammar;
		}

        /// <summary>
        /// Полчить из входного потока очередной символ
        /// Если возвращается символ с кодом 0 значит следует прекратить разбор
        /// </summary>
        /// <returns>символ</returns>
        private static byte NextByte()
        {
            if (m_Reader.BaseStream.Length == m_Reader.BaseStream.Position)
            {
                return (byte)0; 
            }
            byte read_byte = m_Reader.ReadByte();
            return read_byte; 
        }

        /// <summary>
        /// Читаем строчку из входного потока до тех пока не встретим
        /// нулевой юникодный символ (2 байта == 0)
        /// </summary>
        /// <returns></returns>
		private static string ReadString()
		{
			int index = 0;
			byte[] buffer = new byte[1024];

			while(true)
			{
				m_Reader.Read(buffer,index,2);
                if (buffer[index] == 0 && buffer[index+1] == 0)
					break;
				index += 2;
			}
            
			return m_Encoding.GetString(buffer,0,index);
		}

        ///< имя грамматики
		public string Name;
        ///< версия
        public string Version;
        ///< авторы 
        public string Author;
        ///< информацция о грамматике
        public string About;
        ///< чувствительность к регистру
        public bool IsCaseSensitive;
        ///< начальный символ
        public Int16 StartSymbol;

        /// таблицы
        
        // таблица наборов символов
		internal CharacterSet [] CharacterSetTable = null;
		// таблица символов
        internal Symbol [] SymbolTable = null;
		// таблица правил
        internal Rule [] RuleTable = null;
		// таблица состояний КА
        internal DFA [] DFATable = null;
		// таблица состояний LALR анализатора
        internal LALR [] LALRTable = null;

        // начальное состояние КА
		public short InitialDFAState;
        // начальное сотсояние LALR анализатора
		public short InitialLALRState;

        /// <summary>
        /// Конструктор
        /// </summary>
		private Grammar()
		{
			
		}
    }
}
