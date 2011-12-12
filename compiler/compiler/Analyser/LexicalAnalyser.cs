using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace compiler
{
    class LexicalAnalyser
    {
        readonly string xmlName = "tokens.xml";
        private XmlTextWriter _tokensXmlWriter;

        public LexicalAnalyser()
        {
        }

        public string ShowTokens(GOLD.Reduction reduction)
        {
            WriteTokensToXML(reduction);
            return ReadTokensFromXML();
        }

        private string ReadTokensFromXML()
        {
            string tokens = String.Empty;

            try
            {
                XmlTextReader tokensXmlReader = new XmlTextReader(String.Format(@"{0}\{1}",
                                                                  System.IO.Directory.GetCurrentDirectory(),
                                                                  xmlName));
                while (tokensXmlReader.Read())
                {
                    if (tokensXmlReader.NodeType == XmlNodeType.Element && tokensXmlReader.Name == "token")
                    {
                        tokens += String.Format("Token \"{0}\"\n \tLine: {1}\n \tPosition: {2}\n \tValue: {3}\n\n",
                                                tokensXmlReader.GetAttribute("type"),                   
                                                Convert.ToString(Convert.ToInt32(tokensXmlReader.GetAttribute("line")) + 1),
                                                tokensXmlReader.GetAttribute("position"),
                                                tokensXmlReader.GetAttribute("value"));
                    }
                }

                tokensXmlReader.Close();
                return tokens;
            }
            catch
            {
                return "";
            }
        }

        private void WriteTokensToXML(GOLD.Reduction reduction)
        {
            _tokensXmlWriter = new XmlTextWriter(
                String.Format(@"{0}\{1}", System.IO.Directory.GetCurrentDirectory(), xmlName),
                Encoding.Default);

            _tokensXmlWriter.WriteStartElement("tokens");
            Recursion(reduction);
            _tokensXmlWriter.WriteEndElement();

            _tokensXmlWriter.Close();
        }

        private void Recursion(GOLD.Reduction reduction)
        {
            for (int i = 0; i < reduction.Count(); i++)
            {
                switch (reduction[i].Type())
                {
                    case GOLD.SymbolType.Nonterminal:
                        GOLD.Reduction branch = (GOLD.Reduction)reduction[i].Data;
                        Recursion(branch);
                        break;
                    default:
                        _tokensXmlWriter.WriteStartElement("token");

                        var reduct = reduction[i] as GOLD.Token;
                        _tokensXmlWriter.WriteAttributeString("line", reduct.Position().Line.ToString());
                        _tokensXmlWriter.WriteAttributeString("position", reduct.Position().Column.ToString());
                        _tokensXmlWriter.WriteAttributeString("type", reduct.Parent.Name());
                        _tokensXmlWriter.WriteAttributeString("value", reduct.Data.ToString());

                        _tokensXmlWriter.WriteEndElement();
                        break;
                }
            }
        }
    }
}
