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
using System.IO;

using compiler.Controls;
using compiler.Dialogs;

namespace compiler
{
    public partial class CoolCompiler : Window
    {
        private CoolParser myParser = new CoolParser();

        public CoolCompiler()
        {
            InitializeComponent();
            this.DataContext = this;

            LoadGrammarFile();
            TestCoolCore();
        }

        private void TestCoolCore()
        {
            CoolCore.Language l = null;
            l = CoolCore.Language.FromFile(@"./Data/Cool.egt");
        }


        private void LoadGrammarFile()
        {
            try
            {
                string pathToGrammarEGT = String.Format(@"{0}\{1}",
                                                 System.IO.Directory.GetCurrentDirectory(),
                                                 "./Data/Cool.egt").Replace("\\", "/");
                myParser.Setup(pathToGrammarEGT);
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("The error occured while trying to load EGT grammar file.\n{0}\nThe application will be closed.", ex.Message),
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }

        private void miNew_Click(object sender, RoutedEventArgs e)
        {
            this.tc.Visibility = System.Windows.Visibility.Visible;

            // Create tab for new file
            CoolTabItem tabItem = new CoolTabItem();
            this.tc.Items.Add(tabItem);
            this.tc.SelectedItem = tabItem;
        }
        
        #region --- CloseCommand ---

        private Utils.RelayCommand _cmdCloseCommand;
        /// <summary>
        /// Returns a command that closes a TabItem.
        /// </summary>
        public ICommand CloseCommand
        {
            get
            {
                if (_cmdCloseCommand == null)
                {
                    _cmdCloseCommand = new Utils.RelayCommand(
                        param => this.CloseTab_Execute(param),
                        param => this.CloseTab_CanExecute(param)
                        );
                }
                return _cmdCloseCommand;
            }
        }

        /// <summary>
        /// Called when the command is to be executed.
        /// </summary>
        /// <param name="parm">
        /// The TabItem in which the Close-button was clicked.
        /// </param>
        private void CloseTab_Execute(object parm)
        {
            TabItem ti = parm as TabItem;
            if (ti != null)
                tc.Items.Remove(parm);
        }

        /// <summary>
        /// Called when the availability of the Close command needs to be determined.
        /// </summary>
        /// <param name="parm">
        /// The TabItem for which to determine the availability of the Close-command.
        /// </param>
        private bool CloseTab_CanExecute(object parm)
        {
            //For the sample, the closing of TabItems will only be
            //unavailable for disabled TabItems and the very first TabItem.
            TabItem ti = parm as TabItem;
            if (ti != null)
                //We have a valid reference to a TabItem, so return 
                //true if the TabItem is enabled.
                return ti.IsEnabled;

            //If no reference to a TabItem could be obtained, the command 
            //cannot be executed
            return false;
        }

        #endregion

        #region --- Closing Operations ---
        private void miExit_Click(object sender, RoutedEventArgs e)
        {
            if (this.PrepareToClose())
            {
                this.Close();
            }
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = !this.PrepareToClose();
        }

        private bool PrepareToClose()
        {
            bool need_to_show_warning_msg = false;
            bool result = true;
            foreach (CoolTabItem ti in this.tc.Items)
            {
                if (ti.NeedToSave)
                {
                    need_to_show_warning_msg = true;
                    break;
                }
            }

            if (need_to_show_warning_msg)
            {
                ExitDialogBox edb = new ExitDialogBox();
                if (edb.ShowDialog() == true)
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            return result;
        }
        #endregion

        private void miOpen_Click(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = ""; // Default file name
            dlg.DefaultExt = ".cool"; // Default file extension
            dlg.Filter = "Cool documents (.cool)|*.cool"; // Filter files by extension
            dlg.Multiselect = false;
            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Create tab for existed file
                CoolTabItem tabItem = new CoolTabItem(dlg.FileName);
                this.tc.Items.Add(tabItem);
                this.tc.SelectedItem = tabItem;
            }
        }

        private void miClose_Click(object sender, RoutedEventArgs e)
        {

        }

        private void miSave_Click(object sender, RoutedEventArgs e)
        {
            // Configure save file dialog box
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = ""; // Default file name
            dlg.DefaultExt = ".cool"; // Default file extension
            dlg.Filter = "Cool documents (.cool)|*.cool"; // Filter files by extension

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                (this.tc.SelectedItem as CoolTabItem).Save(dlg.FileName);
            }
        }

        /// <summary>
        /// Обработчик события нажатия на кнопку лексического анализа
        /// </summary>
        private void miLA_Click(object sender, RoutedEventArgs e)
        {
            if (tc.Items.Count > 0)
            {
                string code = (tc.SelectedItem as CoolTabItem).Code;
                if (!String.IsNullOrEmpty(code))
                {
                    Results resForm = myParser.Parse(new StringReader(code))
                        ? new Results(Results.AnalyseType.lexical, false, "", myParser.Root)
                        : new Results(Results.AnalyseType.lexical, true, myParser.FailMessage, null);

                    resForm.ShowDialog();
                }
                else
                    MessageBox.Show("Empty code", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
                MessageBox.Show("No code for parsing", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Обработчик события нажатия на кнопку синтаксического анализа
        /// </summary>
        private void miSA_Click(object sender, RoutedEventArgs e)
        {
            if (tc.Items.Count > 0)
            {
                string code = (tc.SelectedItem as CoolTabItem).Code;
                if (!String.IsNullOrEmpty(code))
                {  //парсинг
                    Results resForm = myParser.Parse(new StringReader(code))
                        ? new Results(Results.AnalyseType.syntax, false, "", myParser.Root)
                        : new Results(Results.AnalyseType.syntax, true, myParser.FailMessage, null);

                    resForm.ShowDialog();
                }
                else
                    MessageBox.Show("Empty code", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
                MessageBox.Show("No code for parsing", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
