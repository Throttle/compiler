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

namespace compiler.Dialogs
{
    /// <summary>
    /// Interaction logic for Errors.xaml
    /// </summary>
    public partial class Results : Window
    {
        private GOLD.Reduction _root;

        public Results(bool errors, string failMessage, GOLD.Reduction root)
        {
            InitializeComponent();

            if (errors)
                ShowErrorMessage(failMessage);
            else
            {
                _root = root;
                DrawParseTree();
            }
        }

        private void ShowErrorMessage(string failMessage)
        {
            txtTree.Text = String.Format("An error occured while trying to parse code.\n{0}", failMessage);
        }

        private void DrawParseTree()
        {
            txtTree.Text = "No errors.\nThe parse tree is: \n\n";
            DrawReductionTree();
        }

        private void DrawReductionTree()
        {
            StringBuilder tree = new StringBuilder();

            tree.AppendLine("+-" + _root.Parent.Text(false));
            DrawReduction(tree, _root, 1);

            txtTree.Text += tree.ToString();
        }

        private void DrawReduction(StringBuilder tree, GOLD.Reduction reduction, int indent)
        {
            int n;
            string indentText = "";

            for (n = 1; n <= indent; n++)
            {
                indentText += "| ";
            }

            //=== Display the children of the reduction
            for (n = 0; n < reduction.Count(); n++)
            {
                switch (reduction[n].Type())
                {
                    case GOLD.SymbolType.Nonterminal:
                        GOLD.Reduction branch = (GOLD.Reduction)reduction[n].Data;

                        tree.AppendLine(indentText + "+-" + branch.Parent.Text(false));
                        DrawReduction(tree, branch, indent + 1);
                        break;

                    default:
                        string leaf = (string)reduction[n].Data;

                        tree.AppendLine(indentText + "+-" + leaf);
                        break;
                }
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
