using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

public class CoolParser
    {
        private GOLD.Parser parser = new GOLD.Parser();

        public GOLD.Reduction Root;     //Store the top of the tree
        public string FailMessage;

        private enum SymbolIndex
        {
            @Eof = 0,                                  // (EOF)
            @Error = 1,                                // (Error)
            @Comment = 2,                              // Comment
            @Newline = 3,                              // NewLine
            @Whitespace = 4,                           // Whitespace
            @Timesdiv = 5,                             // '*/'
            @Divtimes = 6,                             // '/*'
            @Divdiv = 7,                               // '//'
            @Minus = 8,                                // '-'
            @Num = 9,                                  // '#'
            @Lparan = 10,                              // '('
            @Rparan = 11,                              // ')'
            @Times = 12,                               // '*'
            @Comma = 13,                               // ','
            @Dot = 14,                                 // '.'
            @Div = 15,                                 // '/'
            @Coloncolon = 16,                          // '::'
            @Semi = 17,                                // ';'
            @Lbracket = 18,                            // '['
            @Lbracketrbracket = 19,                    // '[]'
            @Rbracket = 20,                            // ']'
            @Plus = 21,                                // '+'
            @Lt = 22,                                  // '<'
            @Ltlt = 23,                                // '<<'
            @Lteq = 24,                                // '<='
            @Eq = 25,                                  // '='
            @Eqeq = 26,                                // '=='
            @Gt = 27,                                  // '>'
            @Gteq = 28,                                // '>='
            @Gtgt = 29,                                // '>>'
            @And = 30,                                 // and
            @Begin = 31,                               // begin
            @Boolean = 32,                             // boolean
            @Break = 33,                               // break
            @Call = 34,                                // call
            @Cast = 35,                                // cast
            @Catch = 36,                               // catch
            @Charliteral = 37,                         // CharLiteral
            @Class = 38,                               // class
            @Continue = 39,                            // continue
            @Declare = 40,                             // declare
            @Else = 41,                                // else
            @Elsif = 42,                               // elsif
            @End = 43,                                 // end
            @Exit = 44,                                // exit
            @Extends = 45,                             // extends
            @False = 46,                               // false
            @Id = 47,                                  // Id
            @If = 48,                                  // if
            @Input = 49,                               // input
            @Integer = 50,                             // integer
            @Is = 51,                                  // is
            @Loop = 52,                                // loop
            @Method = 53,                              // method
            @Mod = 54,                                 // mod
            @New = 55,                                 // new
            @Not = 56,                                 // not
            @Null = 57,                                // null
            @Number = 58,                              // Number
            @Or = 59,                                  // or
            @Output = 60,                              // output
            @Private = 61,                             // private
            @Protected = 62,                           // protected
            @Public = 63,                              // public
            @Return = 64,                              // return
            @Stringliteral = 65,                       // StringLiteral
            @Super = 66,                               // super
            @Then = 67,                                // then
            @This = 68,                                // this
            @Throw = 69,                               // throw
            @True = 70,                                // true
            @Try = 71,                                 // try
            @Void = 72,                                // void
            @Access_spec = 73,                         // <ACCESS_SPEC>
            @Addop = 74,                               // <ADDOP>
            @Allocator = 75,                           // <ALLOCATOR>
            @Arglist = 76,                             // <ARGLIST>
            @Assignstmt = 77,                          // <ASSIGNSTMT>
            @Bexpr = 78,                               // <BEXPR>
            @Block = 79,                               // <BLOCK>
            @Body = 80,                                // <BODY>
            @Callstmt = 81,                            // <CALLSTMT>
            @Cast_expr = 82,                           // <CAST_EXPR>
            @Catch_clause = 83,                        // <CATCH_CLAUSE>
            @Catch_clausee = 84,                       // <CATCH_CLAUSEE>
            @Cexpr = 85,                               // <CEXPR>
            @Class2 = 86,                              // <CLASS>
            @Class_member = 87,                        // <CLASS_MEMBER>
            @Class_memberr = 88,                       // <CLASS_MEMBERR>
            @Elseif_part = 89,                         // <ELSEIF_PART>
            @Elsepart = 90,                            // <ELSEPART>
            @Expr = 91,                                // <EXPR>
            @Exprr = 92,                               // <EXPRR>
            @Factor = 93,                              // <FACTOR>
            @Field_decl = 94,                          // <FIELD_DECL>
            @Field_decll = 95,                         // <FIELD_DECLL>
            @Ifstmt = 96,                              // <IFSTMT>
            @Inputstmt = 97,                           // <INPUTSTMT>
            @Loopstmt = 98,                            // <LOOPSTMT>
            @M_type = 99,                              // <M_TYPE>
            @Member_part = 100,                        // <MEMBER_PART>
            @Member_partt = 101,                       // <MEMBER_PARTT>
            @Method2 = 102,                            // <METHOD>
            @Method_decl = 103,                        // <METHOD_DECL>
            @Method_id = 104,                          // <METHOD_ID>
            @Multop = 105,                             // <MULTOP>
            @Optional_id = 106,                        // <OPTIONAL_ID>
            @Outputstmt = 107,                         // <OUTPUTSTMT>
            @Parameter_decl = 108,                     // <PARAMETER_DECL>
            @Parameter_decll = 109,                    // <PARAMETER_DECLL>
            @Parameters = 110,                         // <PARAMETERS>
            @Program = 111,                            // <PROGRAM>
            @Relop = 112,                              // <RELOP>
            @Simpleexpr = 113,                         // <SIMPLEEXPR>
            @Simpleexprr = 114,                        // <SIMPLEEXPRR>
            @Stmt = 115,                               // <STMT>
            @Stmtlist = 116,                           // <STMTLIST>
            @Stmtlistt = 117,                          // <STMTLISTT>
            @String_or_char = 118,                     // <STRING_OR_CHAR>
            @Super_class = 119,                        // <SUPER_CLASS>
            @Super_init = 120,                         // <SUPER_INIT>
            @Term = 121,                               // <TERM>
            @Termm = 122,                              // <TERMM>
            @This_init = 123,                          // <THIS_INIT>
            @Trystmt = 124,                            // <TRYSTMT>
            @Type = 125,                               // <TYPE>
            @Type_id = 126,                            // <TYPE_ID>
            @Typee = 127,                              // <TYPEE>
            @Typeee = 128,                             // <TYPEEE>
            @Value_or_ref = 129,                       // <VALUE_OR_REF>
            @Vardeclist = 130,                         // <VARDECLIST>
            @Vardeclistt = 131,                        // <VARDECLISTT>
            @Vardecs = 132                             // <VARDECS>
        }

        private enum ProductionIndex
        {
            @Program = 0,                              // <PROGRAM> ::= <CLASS> <PROGRAM>
            @Program2 = 1,                             // <PROGRAM> ::= <METHOD> <PROGRAM>
            @Program3 = 2,                             // <PROGRAM> ::= 
            @Access_spec_Private = 3,                  // <ACCESS_SPEC> ::= private
            @Access_spec_Protected = 4,                // <ACCESS_SPEC> ::= protected
            @Access_spec_Public = 5,                   // <ACCESS_SPEC> ::= public
            @Addop_Plus = 6,                           // <ADDOP> ::= '+'
            @Addop_Minus = 7,                          // <ADDOP> ::= '-'
            @Allocator_New_Lparan_Rparan = 8,          // <ALLOCATOR> ::= new <TYPE_ID> '(' <ARGLIST> ')'
            @Allocator_New_Lbracket_Rbracket = 9,      // <ALLOCATOR> ::= new <TYPE_ID> '[' <EXPR> ']'
            @Arglist = 10,                             // <ARGLIST> ::= <EXPRR>
            @Arglist2 = 11,                            // <ARGLIST> ::= 
            @Exprr = 12,                               // <EXPRR> ::= <EXPR>
            @Exprr_Comma = 13,                         // <EXPRR> ::= <EXPR> ',' <EXPRR>
            @Assignstmt_Eq = 14,                       // <ASSIGNSTMT> ::= <FACTOR> '=' <EXPR>
            @Bexpr = 15,                               // <BEXPR> ::= <SIMPLEEXPR>
            @Bexpr2 = 16,                              // <BEXPR> ::= <SIMPLEEXPR> <RELOP> <SIMPLEEXPR>
            @Block_Begin_End = 17,                     // <BLOCK> ::= <VARDECS> begin <STMTLIST> end
            @Body = 18,                                // <BODY> ::= <SUPER_INIT> <THIS_INIT> <BLOCK>
            @Callstmt_Call = 19,                       // <CALLSTMT> ::= call <FACTOR>
            @Cast_expr_Cast_Lparan_Comma_Rparan = 20,  // <CAST_EXPR> ::= cast '(' <TYPE_ID> ',' <EXPR> ')'
            @Catch_clause_Catch_Lparan_Id_Rparan = 21,  // <CATCH_CLAUSE> ::= catch '(' <TYPE_ID> Id ')' <STMTLIST>
            @Cexpr = 22,                               // <CEXPR> ::= <BEXPR>
            @Cexpr_And = 23,                           // <CEXPR> ::= <BEXPR> and <CEXPR>
            @Class_Class_Id_Is_End_Id = 24,            // <CLASS> ::= class Id <SUPER_CLASS> is <CLASS_MEMBERR> end Id
            @Class_memberr = 25,                       // <CLASS_MEMBERR> ::= <CLASS_MEMBER> <CLASS_MEMBERR>
            @Class_memberr2 = 26,                      // <CLASS_MEMBERR> ::= 
            @Class_member = 27,                        // <CLASS_MEMBER> ::= <FIELD_DECL>
            @Class_member2 = 28,                       // <CLASS_MEMBER> ::= <METHOD_DECL>
            @Elsepart_Else = 29,                       // <ELSEPART> ::= else <STMTLIST>
            @Elsepart = 30,                            // <ELSEPART> ::= 
            @Expr = 31,                                // <EXPR> ::= <CEXPR>
            @Expr_Or = 32,                             // <EXPR> ::= <CEXPR> or <EXPR>
            @Factor_Minus = 33,                        // <FACTOR> ::= '-' <FACTOR>
            @Factor_Not = 34,                          // <FACTOR> ::= not <FACTOR>
            @Factor_Number = 35,                       // <FACTOR> ::= Number
            @Factor_False = 36,                        // <FACTOR> ::= false
            @Factor_True = 37,                         // <FACTOR> ::= true
            @Factor_Null = 38,                         // <FACTOR> ::= null
            @Factor = 39,                              // <FACTOR> ::= <ALLOCATOR>
            @Factor2 = 40,                             // <FACTOR> ::= <CAST_EXPR>
            @Factor3 = 41,                             // <FACTOR> ::= <VALUE_OR_REF> <MEMBER_PARTT>
            @Member_partt = 42,                        // <MEMBER_PARTT> ::= <MEMBER_PART> <MEMBER_PARTT>
            @Member_partt2 = 43,                       // <MEMBER_PARTT> ::= 
            @Field_decl_Id_Semi = 44,                  // <FIELD_DECL> ::= <ACCESS_SPEC> <TYPE> Id <FIELD_DECLL> ';'
            @Field_decll_Comma_Id = 45,                // <FIELD_DECLL> ::= ',' Id <FIELD_DECLL>
            @Field_decll = 46,                         // <FIELD_DECLL> ::= 
            @Ifstmt_If_Then_End_If = 47,               // <IFSTMT> ::= if <EXPR> then <STMTLIST> <ELSEIF_PART> <ELSEPART> end if
            @Elseif_part_Elsif_Then = 48,              // <ELSEIF_PART> ::= elsif <EXPR> then <STMTLIST> <ELSEIF_PART>
            @Elseif_part = 49,                         // <ELSEIF_PART> ::= 
            @Inputstmt_Input_Gtgt = 50,                // <INPUTSTMT> ::= input '>>' <FACTOR>
            @Loopstmt_Loop_End_Loop = 51,              // <LOOPSTMT> ::= loop <STMTLIST> end loop
            @Member_part_Dot_Id = 52,                  // <MEMBER_PART> ::= '.' Id
            @Member_part_Dot_Id_Lparan_Rparan = 53,    // <MEMBER_PART> ::= '.' Id '(' <ARGLIST> ')'
            @Member_part_Dot_Id_Lbracket_Rbracket = 54,  // <MEMBER_PART> ::= '.' Id '[' <EXPR> ']'
            @Method_Method_Lparan_Rparan_Is_Id = 55,   // <METHOD> ::= method <M_TYPE> <METHOD_ID> '(' <PARAMETERS> ')' is <BODY> Id
            @Method_decl_Method_Id_Lparan_Rparan_Semi = 56,  // <METHOD_DECL> ::= <ACCESS_SPEC> method <M_TYPE> Id '(' <PARAMETER_DECL> ')' ';'
            @Method_id_Id_Coloncolon_Id = 57,          // <METHOD_ID> ::= Id '::' Id
            @Method_id_Id = 58,                        // <METHOD_ID> ::= Id
            @M_type = 59,                              // <M_TYPE> ::= <TYPE>
            @M_type_Void = 60,                         // <M_TYPE> ::= void
            @Multop_Times = 61,                        // <MULTOP> ::= '*'
            @Multop_Div = 62,                          // <MULTOP> ::= '/'
            @Multop_Mod = 63,                          // <MULTOP> ::= mod
            @Optional_id_Id = 64,                      // <OPTIONAL_ID> ::= Id
            @Optional_id = 65,                         // <OPTIONAL_ID> ::= 
            @Outputstmt_Output_Ltlt = 66,              // <OUTPUTSTMT> ::= output '<<' <EXPR>
            @Outputstmt_Output_Ltlt2 = 67,             // <OUTPUTSTMT> ::= output '<<' <STRING_OR_CHAR>
            @String_or_char_Stringliteral = 68,        // <STRING_OR_CHAR> ::= StringLiteral
            @String_or_char_Charliteral = 69,          // <STRING_OR_CHAR> ::= CharLiteral
            @String_or_char = 70,                      // <STRING_OR_CHAR> ::= 
            @Parameter_decl = 71,                      // <PARAMETER_DECL> ::= <TYPE> <OPTIONAL_ID> <PARAMETER_DECLL>
            @Parameter_decl2 = 72,                     // <PARAMETER_DECL> ::= 
            @Parameter_decll_Comma = 73,               // <PARAMETER_DECLL> ::= ',' <TYPE> <OPTIONAL_ID> <PARAMETER_DECLL>
            @Parameter_decll = 74,                     // <PARAMETER_DECLL> ::= 
            @Parameters_Id = 75,                       // <PARAMETERS> ::= <TYPE> Id <TYPEE>
            @Parameters = 76,                          // <PARAMETERS> ::= 
            @Typee_Comma_Id = 77,                      // <TYPEE> ::= ',' <TYPE> Id <TYPEE>
            @Typee = 78,                               // <TYPEE> ::= 
            @Relop_Eqeq = 79,                          // <RELOP> ::= '=='
            @Relop_Lt = 80,                            // <RELOP> ::= '<'
            @Relop_Lteq = 81,                          // <RELOP> ::= '<='
            @Relop_Gt = 82,                            // <RELOP> ::= '>'
            @Relop_Gteq = 83,                          // <RELOP> ::= '>='
            @Relop_Num = 84,                           // <RELOP> ::= '#'
            @Simpleexpr = 85,                          // <SIMPLEEXPR> ::= <TERM> <SIMPLEEXPRR>
            @Simpleexprr = 86,                         // <SIMPLEEXPRR> ::= <ADDOP> <TERM> <SIMPLEEXPRR>
            @Simpleexprr2 = 87,                        // <SIMPLEEXPRR> ::= 
            @Stmt = 88,                                // <STMT> ::= <BLOCK>
            @Stmt2 = 89,                               // <STMT> ::= <TRYSTMT>
            @Stmt3 = 90,                               // <STMT> ::= <IFSTMT>
            @Stmt4 = 91,                               // <STMT> ::= <LOOPSTMT>
            @Stmt5 = 92,                               // <STMT> ::= <ASSIGNSTMT>
            @Stmt6 = 93,                               // <STMT> ::= <CALLSTMT>
            @Stmt7 = 94,                               // <STMT> ::= <OUTPUTSTMT>
            @Stmt8 = 95,                               // <STMT> ::= <INPUTSTMT>
            @Stmt_Continue = 96,                       // <STMT> ::= continue
            @Stmt_Break = 97,                          // <STMT> ::= break
            @Stmt_Return = 98,                         // <STMT> ::= return
            @Stmt_Return2 = 99,                        // <STMT> ::= return <EXPR>
            @Stmt_Exit = 100,                          // <STMT> ::= exit
            @Stmt_Throw = 101,                         // <STMT> ::= throw <EXPR>
            @Stmtlist_Semi = 102,                      // <STMTLIST> ::= <STMT> ';' <STMTLISTT>
            @Stmtlist = 103,                           // <STMTLIST> ::= 
            @Stmtlistt_Semi = 104,                     // <STMTLISTT> ::= <STMT> ';' <STMTLISTT>
            @Stmtlistt = 105,                          // <STMTLISTT> ::= 
            @Super_init_Super_Lparan_Rparan = 106,     // <SUPER_INIT> ::= super '(' <ARGLIST> ')'
            @Super_init = 107,                         // <SUPER_INIT> ::= 
            @Super_class_Extends_Id = 108,             // <SUPER_CLASS> ::= extends Id
            @Super_class = 109,                        // <SUPER_CLASS> ::= 
            @Term = 110,                               // <TERM> ::= <FACTOR> <TERMM>
            @Termm = 111,                              // <TERMM> ::= <MULTOP> <FACTOR> <TERMM>
            @Termm2 = 112,                             // <TERMM> ::= 
            @This_init_This_Lparan_Rparan = 113,       // <THIS_INIT> ::= this '(' <ARGLIST> ')'
            @This_init = 114,                          // <THIS_INIT> ::= 
            @Trystmt_Try_End_Try = 115,                // <TRYSTMT> ::= try <STMTLIST> <CATCH_CLAUSE> <CATCH_CLAUSEE> end try
            @Catch_clausee = 116,                      // <CATCH_CLAUSEE> ::= <CATCH_CLAUSE> <CATCH_CLAUSEE>
            @Catch_clausee2 = 117,                     // <CATCH_CLAUSEE> ::= 
            @Type = 118,                               // <TYPE> ::= <TYPE_ID>
            @Type_Lbracketrbracket = 119,              // <TYPE> ::= <TYPE_ID> '[]'
            @Type_id_Integer = 120,                    // <TYPE_ID> ::= integer
            @Type_id_Boolean = 121,                    // <TYPE_ID> ::= boolean
            @Type_id_Id = 122,                         // <TYPE_ID> ::= Id
            @Value_or_ref_This = 123,                  // <VALUE_OR_REF> ::= this
            @Value_or_ref_Super = 124,                 // <VALUE_OR_REF> ::= super
            @Value_or_ref_Id = 125,                    // <VALUE_OR_REF> ::= Id
            @Value_or_ref_Id_Lbracket_Rbracket = 126,  // <VALUE_OR_REF> ::= Id '[' <EXPR> ']'
            @Value_or_ref_Id_Lparan_Rparan = 127,      // <VALUE_OR_REF> ::= Id '(' <ARGLIST> ')'
            @Value_or_ref_Lparan_Rparan = 128,         // <VALUE_OR_REF> ::= '(' <EXPR> ')'
            @Vardeclist_Id_Semi = 129,                 // <VARDECLIST> ::= <TYPE> Id <TYPEEE> ';'
            @Typeee_Comma_Id = 130,                    // <TYPEEE> ::= ',' Id <TYPEE>
            @Typeee = 131,                             // <TYPEEE> ::= 
            @Vardecs_Declare = 132,                    // <VARDECS> ::= declare <VARDECLIST> <VARDECLISTT>
            @Vardecs = 133,                            // <VARDECS> ::= 
            @Vardeclistt = 134,                        // <VARDECLISTT> ::= <VARDECLIST> <VARDECLISTT>
            @Vardeclistt2 = 135                        // <VARDECLISTT> ::= 
        }

        //public object program;     //You might derive a specific object

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
            bool done;                      //Controls when we leave the loop
            bool accepted = false;          //Was the parse successful?

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
                        Root = (GOLD.Reduction)parser.CurrentReduction;    //The root node!                                  
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
                    // <PROGRAM> ::= <CLASS> <PROGRAM>
                    break;

                case (short)ProductionIndex.Program2:
                    // <PROGRAM> ::= <METHOD> <PROGRAM>
                    break;

                case (short)ProductionIndex.Program3:
                    // <PROGRAM> ::= 
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

                case (short)ProductionIndex.Addop_Plus:
                    // <ADDOP> ::= '+'
                    break;

                case (short)ProductionIndex.Addop_Minus:
                    // <ADDOP> ::= '-'
                    break;

                case (short)ProductionIndex.Allocator_New_Lparan_Rparan:
                    // <ALLOCATOR> ::= new <TYPE_ID> '(' <ARGLIST> ')'
                    break;

                case (short)ProductionIndex.Allocator_New_Lbracket_Rbracket:
                    // <ALLOCATOR> ::= new <TYPE_ID> '[' <EXPR> ']'
                    break;

                case (short)ProductionIndex.Arglist:
                    // <ARGLIST> ::= <EXPRR>
                    break;

                case (short)ProductionIndex.Arglist2:
                    // <ARGLIST> ::= 
                    break;

                case (short)ProductionIndex.Exprr:
                    // <EXPRR> ::= <EXPR>
                    break;

                case (short)ProductionIndex.Exprr_Comma:
                    // <EXPRR> ::= <EXPR> ',' <EXPRR>
                    break;

                case (short)ProductionIndex.Assignstmt_Eq:
                    // <ASSIGNSTMT> ::= <FACTOR> '=' <EXPR>
                    break;

                case (short)ProductionIndex.Bexpr:
                    // <BEXPR> ::= <SIMPLEEXPR>
                    break;

                case (short)ProductionIndex.Bexpr2:
                    // <BEXPR> ::= <SIMPLEEXPR> <RELOP> <SIMPLEEXPR>
                    break;

                case (short)ProductionIndex.Block_Begin_End:
                    // <BLOCK> ::= <VARDECS> begin <STMTLIST> end
                    break;

                case (short)ProductionIndex.Body:
                    // <BODY> ::= <SUPER_INIT> <THIS_INIT> <BLOCK>
                    break;

                case (short)ProductionIndex.Callstmt_Call:
                    // <CALLSTMT> ::= call <FACTOR>
                    break;

                case (short)ProductionIndex.Cast_expr_Cast_Lparan_Comma_Rparan:
                    // <CAST_EXPR> ::= cast '(' <TYPE_ID> ',' <EXPR> ')'
                    break;

                case (short)ProductionIndex.Catch_clause_Catch_Lparan_Id_Rparan:
                    // <CATCH_CLAUSE> ::= catch '(' <TYPE_ID> Id ')' <STMTLIST>
                    break;

                case (short)ProductionIndex.Cexpr:
                    // <CEXPR> ::= <BEXPR>
                    break;

                case (short)ProductionIndex.Cexpr_And:
                    // <CEXPR> ::= <BEXPR> and <CEXPR>
                    break;

                case (short)ProductionIndex.Class_Class_Id_Is_End_Id:
                    // <CLASS> ::= class Id <SUPER_CLASS> is <CLASS_MEMBERR> end Id
                    break;

                case (short)ProductionIndex.Class_memberr:
                    // <CLASS_MEMBERR> ::= <CLASS_MEMBER> <CLASS_MEMBERR>
                    break;

                case (short)ProductionIndex.Class_memberr2:
                    // <CLASS_MEMBERR> ::= 
                    break;

                case (short)ProductionIndex.Class_member:
                    // <CLASS_MEMBER> ::= <FIELD_DECL>
                    break;

                case (short)ProductionIndex.Class_member2:
                    // <CLASS_MEMBER> ::= <METHOD_DECL>
                    break;

                case (short)ProductionIndex.Elsepart_Else:
                    // <ELSEPART> ::= else <STMTLIST>
                    break;

                case (short)ProductionIndex.Elsepart:
                    // <ELSEPART> ::= 
                    break;

                case (short)ProductionIndex.Expr:
                    // <EXPR> ::= <CEXPR>
                    break;

                case (short)ProductionIndex.Expr_Or:
                    // <EXPR> ::= <CEXPR> or <EXPR>
                    break;

                case (short)ProductionIndex.Factor_Minus:
                    // <FACTOR> ::= '-' <FACTOR>
                    break;

                case (short)ProductionIndex.Factor_Not:
                    // <FACTOR> ::= not <FACTOR>
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

                case (short)ProductionIndex.Factor3:
                    // <FACTOR> ::= <VALUE_OR_REF> <MEMBER_PARTT>
                    break;

                case (short)ProductionIndex.Member_partt:
                    // <MEMBER_PARTT> ::= <MEMBER_PART> <MEMBER_PARTT>
                    break;

                case (short)ProductionIndex.Member_partt2:
                    // <MEMBER_PARTT> ::= 
                    break;

                case (short)ProductionIndex.Field_decl_Id_Semi:
                    // <FIELD_DECL> ::= <ACCESS_SPEC> <TYPE> Id <FIELD_DECLL> ';'
                    break;

                case (short)ProductionIndex.Field_decll_Comma_Id:
                    // <FIELD_DECLL> ::= ',' Id <FIELD_DECLL>
                    break;

                case (short)ProductionIndex.Field_decll:
                    // <FIELD_DECLL> ::= 
                    break;

                case (short)ProductionIndex.Ifstmt_If_Then_End_If:
                    // <IFSTMT> ::= if <EXPR> then <STMTLIST> <ELSEIF_PART> <ELSEPART> end if
                    break;

                case (short)ProductionIndex.Elseif_part_Elsif_Then:
                    // <ELSEIF_PART> ::= elsif <EXPR> then <STMTLIST> <ELSEIF_PART>
                    break;

                case (short)ProductionIndex.Elseif_part:
                    // <ELSEIF_PART> ::= 
                    break;

                case (short)ProductionIndex.Inputstmt_Input_Gtgt:
                    // <INPUTSTMT> ::= input '>>' <FACTOR>
                    break;

                case (short)ProductionIndex.Loopstmt_Loop_End_Loop:
                    // <LOOPSTMT> ::= loop <STMTLIST> end loop
                    break;

                case (short)ProductionIndex.Member_part_Dot_Id:
                    // <MEMBER_PART> ::= '.' Id
                    break;

                case (short)ProductionIndex.Member_part_Dot_Id_Lparan_Rparan:
                    // <MEMBER_PART> ::= '.' Id '(' <ARGLIST> ')'
                    break;

                case (short)ProductionIndex.Member_part_Dot_Id_Lbracket_Rbracket:
                    // <MEMBER_PART> ::= '.' Id '[' <EXPR> ']'
                    break;

                case (short)ProductionIndex.Method_Method_Lparan_Rparan_Is_Id:
                    // <METHOD> ::= method <M_TYPE> <METHOD_ID> '(' <PARAMETERS> ')' is <BODY> Id
                    break;

                case (short)ProductionIndex.Method_decl_Method_Id_Lparan_Rparan_Semi:
                    // <METHOD_DECL> ::= <ACCESS_SPEC> method <M_TYPE> Id '(' <PARAMETER_DECL> ')' ';'
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

                case (short)ProductionIndex.Multop_Times:
                    // <MULTOP> ::= '*'
                    break;

                case (short)ProductionIndex.Multop_Div:
                    // <MULTOP> ::= '/'
                    break;

                case (short)ProductionIndex.Multop_Mod:
                    // <MULTOP> ::= mod
                    break;

                case (short)ProductionIndex.Optional_id_Id:
                    // <OPTIONAL_ID> ::= Id
                    break;

                case (short)ProductionIndex.Optional_id:
                    // <OPTIONAL_ID> ::= 
                    break;

                case (short)ProductionIndex.Outputstmt_Output_Ltlt:
                    // <OUTPUTSTMT> ::= output '<<' <EXPR>
                    break;

                case (short)ProductionIndex.Outputstmt_Output_Ltlt2:
                    // <OUTPUTSTMT> ::= output '<<' <STRING_OR_CHAR>
                    break;

                case (short)ProductionIndex.String_or_char_Stringliteral:
                    // <STRING_OR_CHAR> ::= StringLiteral
                    break;

                case (short)ProductionIndex.String_or_char_Charliteral:
                    // <STRING_OR_CHAR> ::= CharLiteral
                    break;

                case (short)ProductionIndex.String_or_char:
                    // <STRING_OR_CHAR> ::= 
                    break;

                case (short)ProductionIndex.Parameter_decl:
                    // <PARAMETER_DECL> ::= <TYPE> <OPTIONAL_ID> <PARAMETER_DECLL>
                    break;

                case (short)ProductionIndex.Parameter_decl2:
                    // <PARAMETER_DECL> ::= 
                    break;

                case (short)ProductionIndex.Parameter_decll_Comma:
                    // <PARAMETER_DECLL> ::= ',' <TYPE> <OPTIONAL_ID> <PARAMETER_DECLL>
                    break;

                case (short)ProductionIndex.Parameter_decll:
                    // <PARAMETER_DECLL> ::= 
                    break;

                case (short)ProductionIndex.Parameters_Id:
                    // <PARAMETERS> ::= <TYPE> Id <TYPEE>
                    break;

                case (short)ProductionIndex.Parameters:
                    // <PARAMETERS> ::= 
                    break;

                case (short)ProductionIndex.Typee_Comma_Id:
                    // <TYPEE> ::= ',' <TYPE> Id <TYPEE>
                    break;

                case (short)ProductionIndex.Typee:
                    // <TYPEE> ::= 
                    break;

                case (short)ProductionIndex.Relop_Eqeq:
                    // <RELOP> ::= '=='
                    break;

                case (short)ProductionIndex.Relop_Lt:
                    // <RELOP> ::= '<'
                    break;

                case (short)ProductionIndex.Relop_Lteq:
                    // <RELOP> ::= '<='
                    break;

                case (short)ProductionIndex.Relop_Gt:
                    // <RELOP> ::= '>'
                    break;

                case (short)ProductionIndex.Relop_Gteq:
                    // <RELOP> ::= '>='
                    break;

                case (short)ProductionIndex.Relop_Num:
                    // <RELOP> ::= '#'
                    break;

                case (short)ProductionIndex.Simpleexpr:
                    // <SIMPLEEXPR> ::= <TERM> <SIMPLEEXPRR>
                    break;

                case (short)ProductionIndex.Simpleexprr:
                    // <SIMPLEEXPRR> ::= <ADDOP> <TERM> <SIMPLEEXPRR>
                    break;

                case (short)ProductionIndex.Simpleexprr2:
                    // <SIMPLEEXPRR> ::= 
                    break;

                case (short)ProductionIndex.Stmt:
                    // <STMT> ::= <BLOCK>
                    break;

                case (short)ProductionIndex.Stmt2:
                    // <STMT> ::= <TRYSTMT>
                    break;

                case (short)ProductionIndex.Stmt3:
                    // <STMT> ::= <IFSTMT>
                    break;

                case (short)ProductionIndex.Stmt4:
                    // <STMT> ::= <LOOPSTMT>
                    break;

                case (short)ProductionIndex.Stmt5:
                    // <STMT> ::= <ASSIGNSTMT>
                    break;

                case (short)ProductionIndex.Stmt6:
                    // <STMT> ::= <CALLSTMT>
                    break;

                case (short)ProductionIndex.Stmt7:
                    // <STMT> ::= <OUTPUTSTMT>
                    break;

                case (short)ProductionIndex.Stmt8:
                    // <STMT> ::= <INPUTSTMT>
                    break;

                case (short)ProductionIndex.Stmt_Continue:
                    // <STMT> ::= continue
                    break;

                case (short)ProductionIndex.Stmt_Break:
                    // <STMT> ::= break
                    break;

                case (short)ProductionIndex.Stmt_Return:
                    // <STMT> ::= return
                    break;

                case (short)ProductionIndex.Stmt_Return2:
                    // <STMT> ::= return <EXPR>
                    break;

                case (short)ProductionIndex.Stmt_Exit:
                    // <STMT> ::= exit
                    break;

                case (short)ProductionIndex.Stmt_Throw:
                    // <STMT> ::= throw <EXPR>
                    break;

                case (short)ProductionIndex.Stmtlist_Semi:
                    // <STMTLIST> ::= <STMT> ';' <STMTLISTT>
                    break;

                case (short)ProductionIndex.Stmtlist:
                    // <STMTLIST> ::= 
                    break;

                case (short)ProductionIndex.Stmtlistt_Semi:
                    // <STMTLISTT> ::= <STMT> ';' <STMTLISTT>
                    break;

                case (short)ProductionIndex.Stmtlistt:
                    // <STMTLISTT> ::= 
                    break;

                case (short)ProductionIndex.Super_init_Super_Lparan_Rparan:
                    // <SUPER_INIT> ::= super '(' <ARGLIST> ')'
                    break;

                case (short)ProductionIndex.Super_init:
                    // <SUPER_INIT> ::= 
                    break;

                case (short)ProductionIndex.Super_class_Extends_Id:
                    // <SUPER_CLASS> ::= extends Id
                    break;

                case (short)ProductionIndex.Super_class:
                    // <SUPER_CLASS> ::= 
                    break;

                case (short)ProductionIndex.Term:
                    // <TERM> ::= <FACTOR> <TERMM>
                    break;

                case (short)ProductionIndex.Termm:
                    // <TERMM> ::= <MULTOP> <FACTOR> <TERMM>
                    break;

                case (short)ProductionIndex.Termm2:
                    // <TERMM> ::= 
                    break;

                case (short)ProductionIndex.This_init_This_Lparan_Rparan:
                    // <THIS_INIT> ::= this '(' <ARGLIST> ')'
                    break;

                case (short)ProductionIndex.This_init:
                    // <THIS_INIT> ::= 
                    break;

                case (short)ProductionIndex.Trystmt_Try_End_Try:
                    // <TRYSTMT> ::= try <STMTLIST> <CATCH_CLAUSE> <CATCH_CLAUSEE> end try
                    break;

                case (short)ProductionIndex.Catch_clausee:
                    // <CATCH_CLAUSEE> ::= <CATCH_CLAUSE> <CATCH_CLAUSEE>
                    break;

                case (short)ProductionIndex.Catch_clausee2:
                    // <CATCH_CLAUSEE> ::= 
                    break;

                case (short)ProductionIndex.Type:
                    // <TYPE> ::= <TYPE_ID>
                    break;

                case (short)ProductionIndex.Type_Lbracketrbracket:
                    // <TYPE> ::= <TYPE_ID> '[]'
                    break;

                case (short)ProductionIndex.Type_id_Integer:
                    // <TYPE_ID> ::= integer
                    break;

                case (short)ProductionIndex.Type_id_Boolean:
                    // <TYPE_ID> ::= boolean
                    break;

                case (short)ProductionIndex.Type_id_Id:
                    // <TYPE_ID> ::= Id
                    break;

                case (short)ProductionIndex.Value_or_ref_This:
                    // <VALUE_OR_REF> ::= this
                    break;

                case (short)ProductionIndex.Value_or_ref_Super:
                    // <VALUE_OR_REF> ::= super
                    break;

                case (short)ProductionIndex.Value_or_ref_Id:
                    // <VALUE_OR_REF> ::= Id
                    break;

                case (short)ProductionIndex.Value_or_ref_Id_Lbracket_Rbracket:
                    // <VALUE_OR_REF> ::= Id '[' <EXPR> ']'
                    break;

                case (short)ProductionIndex.Value_or_ref_Id_Lparan_Rparan:
                    // <VALUE_OR_REF> ::= Id '(' <ARGLIST> ')'
                    break;

                case (short)ProductionIndex.Value_or_ref_Lparan_Rparan:
                    // <VALUE_OR_REF> ::= '(' <EXPR> ')'
                    break;

                case (short)ProductionIndex.Vardeclist_Id_Semi:
                    // <VARDECLIST> ::= <TYPE> Id <TYPEEE> ';'
                    break;

                case (short)ProductionIndex.Typeee_Comma_Id:
                    // <TYPEEE> ::= ',' Id <TYPEE>
                    break;

                case (short)ProductionIndex.Typeee:
                    // <TYPEEE> ::= 
                    break;

                case (short)ProductionIndex.Vardecs_Declare:
                    // <VARDECS> ::= declare <VARDECLIST> <VARDECLISTT>
                    break;

                case (short)ProductionIndex.Vardecs:
                    // <VARDECS> ::= 
                    break;

                case (short)ProductionIndex.Vardeclistt:
                    // <VARDECLISTT> ::= <VARDECLIST> <VARDECLISTT>
                    break;

                case (short)ProductionIndex.Vardeclistt2:
                    // <VARDECLISTT> ::= 
                    break;

            }  //switch

            return result;
        }
    }