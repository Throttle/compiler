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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace compiler.Controls
{
    public partial class CoolTabItem : TabItem
    {
        private string backup = "";

        public bool NeedToSave
        {
            get { return this.OldCode != this.Code; }
        }


        public string Code
        {
            get { return this.tbCode.Text; }
        }


        public string OldCode
        {
            get { return this.backup; }
        }
        

        public CoolTabItem()
        {
            InitializeComponent();
            this.Header = "new *";
        }


        public CoolTabItem(string file_path)
        {
            InitializeComponent();
            this.LoadFromFile(file_path);
        }


        private void LoadFromFile(string file_path)
        {
            System.IO.TextReader tr = new System.IO.StreamReader(file_path);
            this.tbCode.Text = tr.ReadToEnd();
            tr.Close();

            this.Header = System.IO.Path.GetFileName(file_path);
            this.backup = this.Code;
        }


        public void Save(string file_path)
        {
            System.IO.TextWriter wr = new System.IO.StreamWriter(file_path);
            wr.Write(this.Code);
            wr.Close();

            this.Header = System.IO.Path.GetFileName(file_path);
            this.backup = this.Code;
        }
    }
}
