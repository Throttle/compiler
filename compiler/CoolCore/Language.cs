using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;

namespace CoolCore
{
    public class Language
    {
        private State[] m_States = null;
		private Production[] m_Productions = null;
		private ParserState [] m_ParserStates = null;
		private Symbol[] m_Symbols = null;

		private State m_StartState = null;
		private ParserState m_ParserStartState = null;

		public static Language FromFile(string path)
		{
			Language language = new Language();

			Grammar grammar = Grammar.FromFile(path);
		
			//
			// Create Symbols
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
			// Create DFA states.
			//

			language.m_States = new State[grammar.DFATable.Length];
			for(int i = 0; i < grammar.DFATable.Length; i++)
			{
				language.m_States[i] = new State();
				language.m_States[i].ID = i;
			}

			//
			// Create Edges and update states.
			//

			for(int i = 0; i < grammar.DFATable.Length; i++)
			{
				// Update states.
				language.m_States[i].IsAccepting = grammar.DFATable[i].IsAccepting;
				if(language.m_States[i].IsAccepting)
					language.m_States[i].Accepts = language.m_Symbols[grammar.DFATable[i].AcceptIndex];
			
				// Create edges.
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
			// Create productions.
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
			// Create parser states.
			//

			language.m_ParserStates = new ParserState[grammar.LALRTable.Length];

			for(int i = 0;i<grammar.LALRTable.Length;i++)
				language.m_ParserStates[i] = new ParserState();;

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

		public State[] States
		{
			get{return m_States;}
		}

		public ParserState[] ParserStates
		{
			get{return m_ParserStates;}
		}

		public Production[] Productions
		{
			get{return m_Productions;}
		}

		public Symbol[] Symbols
		{
			get{return m_Symbols;}
		}

		public State StartState
		{
			get{return m_StartState;}
		}

		public ParserState ParserStartState
		{
			get{return m_ParserStartState;}
		}

	
		public void ApplySemantic(Production production, Stack syntaxStack)
		{
			switch(production.m_ID)
			{
				case 0:
					break;
			}

		}

	}

	public class Grammar
	{
		internal class CharacterSet
		{
			public short Index;
			public string Characters;
		}

		internal class Symbol
		{
			public short Index;
			public string Name;
			public short Kind;
		}

		internal class Rule
		{
			public short Index;
			public short Nonterminal;
			public short [] Symbols = null;
		}

		internal class Edge
		{
			public short CharacterSetIndex;
			public short TargetIndex;
		}

		internal class DFA
		{
			public short Index;
			public bool IsAccepting;
			public short AcceptIndex;
			public Edge [] Edges;
		}

		internal class Action
		{
			public short SymbolIndex;
			public short ActionType;
			public short Target;
		}

		internal class LALR
		{
			public short Index;
			public Action [] Actions;
		}

		private static BinaryReader m_Reader = null;
		private static Encoding m_Encoding = null;

		public static Grammar FromFile(string path)
		{
			m_Encoding = new UnicodeEncoding(false, true);
			m_Reader = new BinaryReader(new FileStream(path, FileMode.Open));

			if(ReadString() != "GOLD Parser Tables/v5.0")
			{
				m_Reader.Close();
				throw new Exception("Invalid file format.");
			}

			Grammar grammar = new Grammar();

			int count;

			//
			// Read Records
			//
			
			try
			{
				while(m_Reader.ReadByte() == (byte)'M')
				{
				
					Int16 entries = m_Reader.ReadInt16();

					//
					// Read entry
					//

					byte read_byte = m_Reader.ReadByte();
                    read_byte = m_Reader.ReadByte();
                    char rbch = (char)read_byte;
					if (read_byte == (byte)'p')
                    {
						m_Reader.ReadInt32();
						grammar.Name = ReadString();
                        m_Reader.ReadInt32();
						grammar.Version = ReadString();
                        m_Reader.ReadInt32();
						grammar.Author = ReadString();
                        m_Reader.ReadInt32();
						grammar.About = ReadString();
                        m_Reader.ReadInt32();
						grammar.IsCaseSensitive = !(m_Reader.ReadByte() == 0);
                        m_Reader.ReadInt32();
						grammar.StartSymbol = m_Reader.ReadInt16();
                    }
                    else if (read_byte == (byte)'t')
                    {
							m_Reader.ReadByte();
							grammar.SymbolTable = new Symbol[m_Reader.ReadInt16()];

							m_Reader.ReadByte();
							grammar.CharacterSetTable = new CharacterSet[m_Reader.ReadInt16()];

							m_Reader.ReadByte();
							grammar.RuleTable = new Rule[m_Reader.ReadInt16()];

							m_Reader.ReadByte();
							grammar.DFATable = new DFA[m_Reader.ReadInt16()];

							m_Reader.ReadByte();
							grammar.LALRTable = new LALR[m_Reader.ReadInt16()];

                    }
                    else if (read_byte == (byte)'c')
					{
						CharacterSet item = new CharacterSet();

						m_Reader.ReadByte();
						item.Index = m_Reader.ReadInt16();

						m_Reader.ReadByte();
						item.Characters = ReadString();

						grammar.CharacterSetTable[item.Index] = item;

					}
                    else if (read_byte == (byte)'r')
					{
							Rule item = new Rule();

							m_Reader.ReadByte();
							item.Index = m_Reader.ReadInt16();

							m_Reader.ReadByte();
							item.Nonterminal = m_Reader.ReadInt16();

							m_Reader.ReadByte();

							//
							// Read Symbol Indices
							//

							item.Symbols = new short[entries - 4];
							for(int x = 0; x < item.Symbols.Length; x++)
							{
								m_Reader.ReadByte();
								item.Symbols[x] = m_Reader.ReadInt16();
							}

							grammar.RuleTable[item.Index] = item;
					}
                    else if (read_byte == (byte)'s')
					{
							Symbol item = new Symbol();

							m_Reader.ReadByte();
							item.Index = m_Reader.ReadInt16();

							m_Reader.ReadByte();
							item.Name = ReadString();

							m_Reader.ReadByte();
							item.Kind = m_Reader.ReadInt16();

							grammar.SymbolTable[item.Index] = item;
					}
                    else if (read_byte == (byte)'d')
					{
    						DFA item = new DFA();

							m_Reader.ReadByte();
							item.Index = m_Reader.ReadInt16();

							m_Reader.ReadByte();
							item.IsAccepting = !(m_Reader.ReadByte() == 0);

							m_Reader.ReadByte();
							item.AcceptIndex = m_Reader.ReadInt16();

							m_Reader.ReadByte();

							//
							// Read Edges
							//

							item.Edges = new Edge[(entries - 5) / 3];

							for(int x = 0; x < item.Edges.Length; x++)
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
						}
                        else if (read_byte == (byte)'l')
						{
							LALR item = new LALR();

							m_Reader.ReadByte();
							item.Index = m_Reader.ReadInt16();

							m_Reader.ReadByte();

							//
							// Read Actions
							//

							item.Actions = new Action[(entries - 3)/4];

							for(int x = 0; x < item.Actions.Length; x++)
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
						}
                        else if (read_byte == (byte)'i')
                        {
	                        m_Reader.ReadByte();
	                        grammar.InitialDFAState = m_Reader.ReadInt16();
	                        m_Reader.ReadByte();
	                        grammar.InitialLALRState = m_Reader.ReadInt16();
                        }
                }
			}
			catch(Exception x)
			{

			}

			return grammar;

		}

		private static string ReadString()
		{
			int index = 0;
			byte[] buffer = new byte[1024];

			while(true)
			{
				m_Reader.Read(buffer,index,2);
				if(buffer[index] == 0)
					break;
				index += 2;
			}

			return m_Encoding.GetString(buffer,0,index);
		}

		public string Name;
		public string Version;
		public string Author;
		public string About;
		public bool IsCaseSensitive;
		public Int16 StartSymbol;

		internal CharacterSet [] CharacterSetTable = null;
		internal Symbol [] SymbolTable = null;
		internal Rule [] RuleTable = null;
		internal DFA [] DFATable = null;
		internal LALR [] LALRTable = null;


		public short InitialDFAState;
		public short InitialLALRState;

		private Grammar()
		{
			
		}
    }
}
