using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoolCore
{
    class Program
    {
        static void Main(string[] args)
        {
            CoolCore.Language language = null;
            string pathToGrammarEGT = System.IO.Path.GetFullPath("./../../../compiler/Data/Cool.egt");
            string pathToExampleEGT = System.IO.Path.GetFullPath("./../../../compiler/Data/example.cool");
            language = CoolCore.Language.FromFile(pathToGrammarEGT);
            CoolCore.Compiler.Scanner scanner = new CoolCore.Compiler.Scanner(pathToExampleEGT, language);
            CoolCore.Compiler.Parser parser = new CoolCore.Compiler.Parser(scanner, language);
            CoolCore.Compiler.ParseTreeNode tree = parser.CreateParseTree();
            CoolCore.Syntax.Module stree = parser.CreateSyntaxTree();
            Console.WriteLine(tree.Name);
        }
    }
}
