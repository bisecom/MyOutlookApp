using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MyOutlook
{
    /// <summary>
    /// Логика взаимодействия для SearchRequestStringsWindow.xaml
    /// </summary>
    public partial class SearchRequestStringsWindow : Window
    {
        public bool EmailCheck { get; set; }
        public SearchRequestStringsWindow()
        {
            InitializeComponent();
            EmailCheck = false;
        }

        public SearchRequestStringsWindow(string labelText)
        {
            InitializeComponent();
            MainTBl.Text = labelText;
            EmailCheck = true;
        }

        private void OkBtnClick(Object sender, System.EventArgs e)
        {
            this.DialogResult = true;
        }

        private void DataChangedWhomCCAddress(object sender, RoutedEventArgs e)
        {
            if(EmailCheck)
            AddressSpellingCheck(this.SortRequestTbx.Text);
        }

        private void AddressSpellingCheck(string str)
        {
            Regex regex = new Regex(@"^((\w{2,}|\d{2,})(\.))|(\w{2,}|\d{2,})@((\w{2,}|\d{2,})(\.))(\w{2,4}|\d{2,4})$");
            MatchCollection matches = regex.Matches(str);
            if (matches.Count > 0)
                return;
            else
                MessageBox.Show("Email has not correct format!");
        }

    }
}
