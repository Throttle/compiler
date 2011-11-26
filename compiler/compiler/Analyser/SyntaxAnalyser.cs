using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace compiler
{
    class SyntaxAnalyser
    {
        public SyntaxAnalyser()
        {
        }

        public string DrawReductionTree(GOLD.Reduction root)
        {
            StringBuilder tree = new StringBuilder();

            tree.AppendLine("+-" + root.Parent.Text(false));
            DrawReduction(tree, root, 1);

            return tree.ToString();
        }

        private void DrawReduction(StringBuilder tree, GOLD.Reduction reduction, int indent)
        {
            int n;
            string indentText = "";

            for (n = 1; n <= indent; n++)
                indentText += "| ";

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
    }
}
