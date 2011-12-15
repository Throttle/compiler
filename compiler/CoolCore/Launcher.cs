using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoolCore.Compilation;
using System.IO;
using System.Diagnostics;

namespace CoolCore
{
    class Program
    {
        static void Main(string[] args)
        {
            CoolCore.Language language = null;
            string pathToGrammarEGT = System.IO.Path.GetFullPath("./../../../compiler/Data/Cool.egt");
            string pathToExampleEGT = System.IO.Path.GetFullPath("./../../../compiler/Data/example_arrays.cool");
            language = CoolCore.Language.FromFile(pathToGrammarEGT);
            CoolCore.Compiler.Scanner scanner = new CoolCore.Compiler.Scanner(pathToExampleEGT, language);
            CoolCore.Compiler.Parser parser = new CoolCore.Compiler.Parser(scanner, language);
            CoolCore.Compiler.ParseTreeNode tree = parser.CreateParseTree();
            CoolCore.Module module = parser.CreateSyntaxTree();
            
            Generator generator = new Generator(module);


            string path = module.Name + ".exe";

            if (File.Exists(path))
                File.Delete(path);

            generator.Compile(path);

            //
            // Use ildasm to decompile executable.
            //

            
            if(File.Exists("il.txt"))
                File.Delete("il.txt");

            //Process ildasm = new Process();
            //ildasm.StartInfo.FileName = "ildasm.exe";
            //ildasm.StartInfo.Arguments = "/OUT=il.txt " + path;
            //ildasm.Start();
            //ildasm.WaitForExit();

            //StreamReader il = File.OpenText("il.txt");
            //Console.WriteLine(il.ReadToEnd());
            //il.Close();

            
        }
    }
}
