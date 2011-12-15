using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;

namespace compiler.Dialogs
{
    public partial class Results : Window
    {
        private GOLD.Reduction _root;
        public enum AnalyseType { lexical, syntax };

        public Results(string out_file)
        {
            InitializeComponent();
            this.ShowInTaskbar = true;
            StreamReader il = File.OpenText(System.IO.Path.GetFullPath(out_file));
            txtTree.Text = il.ReadToEnd();
            il.Close();            
        }

        public Results(AnalyseType analyser, bool errors, string failMessage, GOLD.Reduction root)
        {
            InitializeComponent();
            this.ShowInTaskbar = true;
            this.Title = analyser == AnalyseType.lexical
                ? "Lexical analyzer results"
                : "Syntax analyzer results";

            if (errors)
                txtTree.Text = String.Format("An error occured while trying to parse code.\n{0}", failMessage);
            else
            {
                _root = root;

                switch (analyser)
                {
                    case (AnalyseType.lexical):
                        ShowTokens();
                        break;
                    case (AnalyseType.syntax):
                        ShowParseTree();
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Нарисовать дерево разбора
        /// </summary>
        private void ShowParseTree()
        {
            txtTree.Text = "No errors.\nThe parse tree is: \n\n";
            SyntaxAnalyser mySyntaxAnalyser = new SyntaxAnalyser();
            txtTree.Text += mySyntaxAnalyser.DrawReductionTree(_root);
        }

        /// <summary>
        /// Показать найденные токены
        /// </summary>
        private void ShowTokens()
        {
            txtTree.Text = "No errors.\nFound tokens: \n\n";
            LexicalAnalyser myLexicalAnalyser = new LexicalAnalyser();
            txtTree.Text += myLexicalAnalyser.ShowTokens(_root);
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
