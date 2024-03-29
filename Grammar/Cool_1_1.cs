﻿//Generated by the GOLD Parser Builder

using System.IO;

public class CoolParser
{
	private GOLD.Parser parser = new GOLD.Parser();

	public GOLD.Reduction Root;	 //Store the top of the tree
	public string FailMessage;

	private enum SymbolIndex
	{
		@Eof = 0,								  // (EOF)
		@Error = 1,								// (Error)
		@Comment = 2,							  // Comment
		@Newline = 3,							  // NewLine
		@Whitespace = 4,						   // Whitespace
		@Timesdiv = 5,							 // '*/'
		@Divtimes = 6,							 // '/*'
		@Divdiv = 7,							   // '//'
		@Minus = 8,								// '-'
		@Exclam = 9,							   // '!'
		@Num = 10,								 // '#'
		@Percent = 11,							 // '%'
		@Ampamp = 12,							  // '&&'
		@Lparan = 13,							  // '('
		@Rparan = 14,							  // ')'
		@Times = 15,							   // '*'
		@Comma = 16,							   // ','
		@Dot = 17,								 // '.'
		@Div = 18,								 // '/'
		@Coloncolon = 19,						  // '::'
		@Semi = 20,								// ';'
		@Lbracket = 21,							// '['
		@Lbracketrbracket = 22,					// '[]'
		@Rbracket = 23,							// ']'
		@Pipepipe = 24,							// '||'
		@Plus = 25,								// '+'
		@Lt = 26,								  // '<'
		@Ltlt = 27,								// '<<'
		@Lteq = 28,								// '<='
		@Eq = 29,								  // '='
		@Eqeq = 30,								// '=='
		@Gt = 31,								  // '>'
		@Gteq = 32,								// '>='
		@Gtgt = 33,								// '>>'
		@Begin = 34,							   // begin
		@Boolean = 35,							 // boolean
		@Break = 36,							   // break
		@Call = 37,								// call
		@Cast = 38,								// cast
		@Catch = 39,							   // catch
		@Charliteral = 40,						 // CharLiteral
		@Class = 41,							   // class
		@Continue = 42,							// continue
		@Declare = 43,							 // declare
		@Else = 44,								// else
		@Elsif = 45,							   // elsif
		@End = 46,								 // end
		@Exit = 47,								// exit
		@Extends = 48,							 // extends
		@False = 49,							   // false
		@Id = 50,								  // Id
		@If = 51,								  // if
		@Input = 52,							   // input
		@Integer = 53,							 // integer
		@Is = 54,								  // is
		@Loop = 55,								// loop
		@Method = 56,							  // method
		@New = 57,								 // new
		@Null = 58,								// null
		@Number = 59,							  // Number
		@Output = 60,							  // output
		@Private = 61,							 // private
		@Protected = 62,						   // protected
		@Public = 63,							  // public
		@Return = 64,							  // return
		@Stringliteral = 65,					   // StringLiteral
		@Super = 66,							   // super
		@Then = 67,								// then
		@This = 68,								// this
		@Throw = 69,							   // throw
		@True = 70,								// true
		@Try = 71,								 // try
		@Void = 72,								// void
		@Access_spec = 73,						 // <ACCESS_SPEC>
		@Allocator = 74,						   // <ALLOCATOR>
		@Arglist = 75,							 // <ARGLIST>
		@Array_type = 76,						  // <ARRAY_TYPE>
		@Assignment = 77,						  // <ASSIGNMENT>
		@Block = 78,							   // <BLOCK>
		@Body = 79,								// <BODY>
		@Cast_expr = 80,						   // <CAST_EXPR>
		@Catch_clause = 81,						// <CATCH_CLAUSE>
		@Class2 = 82,							  // <CLASS>
		@Class_member = 83,						// <CLASS_MEMBER>
		@Class_memberlist = 84,					// <CLASS_MEMBERLIST>
		@Elseif_part = 85,						 // <ELSEIF_PART>
		@Elsepart = 86,							// <ELSEPART>
		@Expression = 87,						  // <EXPRESSION>
		@Expression_binary = 88,				   // <EXPRESSION_BINARY>
		@Expression_factor = 89,				   // <EXPRESSION_FACTOR>
		@Expression_primary = 90,				  // <EXPRESSION_PRIMARY>
		@Expression_term = 91,					 // <EXPRESSION_TERM>
		@Expression_unary = 92,					// <EXPRESSION_UNARY>
		@Factor = 93,							  // <FACTOR>
		@Field_decl = 94,						  // <FIELD_DECL>
		@Field_decllist = 95,					  // <FIELD_DECLLIST>
		@Function_call = 96,					   // <FUNCTION_CALL>
		@Ifstmt = 97,							  // <IFSTMT>
		@Inputstmt = 98,						   // <INPUTSTMT>
		@M_type = 99,							  // <M_TYPE>
		@Method2 = 100,							// <METHOD>
		@Method_decl = 101,						// <METHOD_DECL>
		@Method_id = 102,						  // <METHOD_ID>
		@Name = 103,							   // <NAME>
		@Outputstmt = 104,						 // <OUTPUTSTMT>
		@Parameter_decl = 105,					 // <PARAMETER_DECL>
		@Parameters = 106,						 // <PARAMETERS>
		@Primitive_type = 107,					 // <PRIMITIVE_TYPE>
		@Program = 108,							// <PROGRAM>
		@Statement = 109,						  // <STATEMENT>
		@Statements = 110,						 // <STATEMENTS>
		@Structure_type = 111,					 // <STRUCTURE_TYPE>
		@Super_class = 112,						// <SUPER_CLASS>
		@Super_init = 113,						 // <SUPER_INIT>
		@This_init = 114,						  // <THIS_INIT>
		@Trystmt = 115,							// <TRYSTMT>
		@Type = 116,							   // <TYPE>
		@Var_typelist = 117,					   // <VAR_TYPELIST>
		@Vardeclist = 118,						 // <VARDECLIST>
		@Vardecs = 119							 // <VARDECS>
	}

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

	//public object program;	 //You might derive a specific object

	public bool Setup(string FilePath)
	{
		return parser.LoadTables(FilePath);
	}

	public bool Parse(TextReader reader)
	{
		//This procedure starts the GOLD Parser Engine and handles each of the
		//messages it returns. Each time a reduction is made, you can create new
		//custom object and reassign the .CurrentReduction property. Otherwise, 
		//the system will use the Reduction object that was returned.
		//
		//The resulting tree will be a pure representation of the language 
		//and will be ready to implement.

		GOLD.ParseMessage response;
		bool done;					  //Controls when we leave the loop
		bool accepted = false;		  //Was the parse successful?

		parser.Open(reader);
		parser.TrimReductions = false;  //Please read about this feature before enabling  

		done = false;
		while (!done)
		{
			response = parser.Parse();

			switch (response)
			{
				case GOLD.ParseMessage.LexicalError:
					//Cannot recognize token
					FailMessage = "Lexical Error:\n" +
 "Position: " + parser.CurrentPosition().Line + ", " + parser.CurrentPosition().Column + "\n" +
 "Read: " + parser.CurrentToken().Data;
					done = true;
					break;

				case GOLD.ParseMessage.SyntaxError:
					//Expecting a different token
					FailMessage = "Syntax Error:\n" +
 "Position: " + parser.CurrentPosition().Line + ", " + parser.CurrentPosition().Column + "\n" +
 "Read: " + parser.CurrentToken().Data + "\n" +
 "Expecting: " + parser.ExpectedSymbols().Text();
					done = true;
					break;

				case GOLD.ParseMessage.Reduction:
					//Create a customized object to store the reduction


					//parser.CurrentReduction = CreateNewObject(parser.CurrentReduction as GOLD.Reduction);

					break;
				case GOLD.ParseMessage.Accept:
					//Accepted!
					//program = parser.CurrentReduction   //The root node!	 
					Root = (GOLD.Reduction)parser.CurrentReduction;	//The root node! 
					done = true;
					accepted = true;
					break;

				case GOLD.ParseMessage.TokenRead:
					//You don't have to do anything here.
					break;

				case GOLD.ParseMessage.InternalError:
					//INTERNAL ERROR! Something is horribly wrong.
					done = true;
					break;

				case GOLD.ParseMessage.NotLoadedError:
					//This error occurs if the CGT was not loaded.				   
					FailMessage = "Tables not loaded";
					done = true;
					break;

				case GOLD.ParseMessage.GroupError:
					//GROUP ERROR! Unexpected end of file
					FailMessage = "Runaway group";
					done = true;
					break;
			}
		} //while

		return accepted;
	}

	private object CreateNewObject(GOLD.Reduction r)
	{
		object result = null;

		switch (r.Parent.TableIndex())
		{
			case (short)ProductionIndex.Program:				 
				// <PROGRAM> ::= <CLASS>
				break;

			case (short)ProductionIndex.Program2:				 
				// <PROGRAM> ::= <METHOD>
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
				break;

			case (short)ProductionIndex.This_init_This_Lparan_Rparan:				 
				// <THIS_INIT> ::= this '(' <ARGLIST> ')'
				break;

			case (short)ProductionIndex.Super_init_Super_Lparan_Rparan:				 
				// <SUPER_INIT> ::= super '(' <ARGLIST> ')'
				break;

			case (short)ProductionIndex.Block_Begin_End:				 
				// <BLOCK> ::= <VARDECS> begin <STATEMENTS> end
				break;

			case (short)ProductionIndex.Block_Begin_End2:				 
				// <BLOCK> ::= begin <STATEMENTS> end
				break;

			case (short)ProductionIndex.Vardeclist_Id_Semi:				 
				// <VARDECLIST> ::= <TYPE> Id ';'
				break;

			case (short)ProductionIndex.Vardeclist_Id_Semi2:				 
				// <VARDECLIST> ::= <TYPE> Id <VAR_TYPELIST> ';'
				break;

			case (short)ProductionIndex.Var_typelist_Comma_Id:				 
				// <VAR_TYPELIST> ::= ',' Id
				break;

			case (short)ProductionIndex.Var_typelist_Comma_Id2:				 
				// <VAR_TYPELIST> ::= <VAR_TYPELIST> ',' Id
				break;

			case (short)ProductionIndex.Vardecs_Declare:				 
				// <VARDECS> ::= declare <VARDECLIST>
				break;

			case (short)ProductionIndex.Vardecs_Declare2:				 
				// <VARDECS> ::= declare <VARDECLIST> <VARDECS>
				break;

			case (short)ProductionIndex.Name_Id:				 
				// <NAME> ::= Id
				break;

			case (short)ProductionIndex.Name_Dot_Id:				 
				// <NAME> ::= <NAME> '.' Id
				break;

			case (short)ProductionIndex.Assignment_Eq:				 
				// <ASSIGNMENT> ::= <NAME> '=' <EXPRESSION>
				break;

			case (short)ProductionIndex.Factor_This:				 
				// <FACTOR> ::= this
				break;

			case (short)ProductionIndex.Factor_Super:				 
				// <FACTOR> ::= super
				break;

			case (short)ProductionIndex.Factor_Number:				 
				// <FACTOR> ::= Number
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
				break;

			case (short)ProductionIndex.Expression_Plus:				 
				// <EXPRESSION> ::= <EXPRESSION> '+' <EXPRESSION_TERM>
				break;

			case (short)ProductionIndex.Expression_Minus:				 
				// <EXPRESSION> ::= <EXPRESSION> '-' <EXPRESSION_TERM>
				break;

			case (short)ProductionIndex.Expression_term:				 
				// <EXPRESSION_TERM> ::= <EXPRESSION_FACTOR>
				break;

			case (short)ProductionIndex.Expression_term_Times:				 
				// <EXPRESSION_TERM> ::= <EXPRESSION_TERM> '*' <EXPRESSION_FACTOR>
				break;

			case (short)ProductionIndex.Expression_term_Div:				 
				// <EXPRESSION_TERM> ::= <EXPRESSION_TERM> '/' <EXPRESSION_FACTOR>
				break;

			case (short)ProductionIndex.Expression_factor:				 
				// <EXPRESSION_FACTOR> ::= <EXPRESSION_BINARY>
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
				break;

			case (short)ProductionIndex.Expression_unary_Lbracket_Rbracket:				 
				// <EXPRESSION_UNARY> ::= <EXPRESSION_PRIMARY> '[' <EXPRESSION> ']'
				break;

			case (short)ProductionIndex.Expression_unary_Lparan_Rparan:				 
				// <EXPRESSION_UNARY> ::= <EXPRESSION_PRIMARY> '(' <ARGLIST> ')'
				break;

			case (short)ProductionIndex.Expression_primary:				 
				// <EXPRESSION_PRIMARY> ::= <NAME>
				break;

			case (short)ProductionIndex.Expression_primary2:				 
				// <EXPRESSION_PRIMARY> ::= <FUNCTION_CALL>
				break;

			case (short)ProductionIndex.Expression_primary3:				 
				// <EXPRESSION_PRIMARY> ::= <FACTOR>
				break;

			case (short)ProductionIndex.Statements:				 
				// <STATEMENTS> ::= <STATEMENT>
				break;

			case (short)ProductionIndex.Statements2:				 
				// <STATEMENTS> ::= <STATEMENTS> <STATEMENT>
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
				break;

			case (short)ProductionIndex.Statement_Semi3:				 
				// <STATEMENT> ::= <INPUTSTMT> ';'
				break;

			case (short)ProductionIndex.Statement_Semi4:				 
				// <STATEMENT> ::= <OUTPUTSTMT> ';'
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
				// <OUTPUTSTMT> ::= output '<<' <EXPRESSION>
				break;

			case (short)ProductionIndex.Outputstmt_Output_Ltlt_Stringliteral:				 
				// <OUTPUTSTMT> ::= output '<<' StringLiteral
				break;

			case (short)ProductionIndex.Outputstmt_Output_Ltlt_Charliteral:				 
				// <OUTPUTSTMT> ::= output '<<' CharLiteral
				break;

			case (short)ProductionIndex.Inputstmt_Input_Gtgt:				 
				// <INPUTSTMT> ::= input '>>' <NAME>
				break;

			case (short)ProductionIndex.Type:				 
				// <TYPE> ::= <STRUCTURE_TYPE>
				break;

			case (short)ProductionIndex.Type2:				 
				// <TYPE> ::= <PRIMITIVE_TYPE>
				break;

			case (short)ProductionIndex.Type3:				 
				// <TYPE> ::= <ARRAY_TYPE>
				break;

			case (short)ProductionIndex.Primitive_type_Integer:				 
				// <PRIMITIVE_TYPE> ::= integer
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
				break;

			case (short)ProductionIndex.M_type:				 
				// <M_TYPE> ::= <TYPE>
				break;

			case (short)ProductionIndex.M_type_Void:				 
				// <M_TYPE> ::= void
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

		return result;
	}
	
}; //MyParser
