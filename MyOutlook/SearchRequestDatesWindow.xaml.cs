using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Логика взаимодействия для SearchRequestDatesWindow.xaml
    /// </summary>
    public partial class SearchRequestDatesWindow : Window
    {
        public SearchRequestDatesWindow()
        {
            InitializeComponent();
        }

        private void OkBtnClick(Object sender, System.EventArgs e)
        {
            this.DialogResult = true;
        }

        private void PickerSelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            DateTime? selectedDate = ToDatePicker.SelectedDate;

            if(selectedDate < FromDatePicker.SelectedDate)

            MessageBox.Show("Please, enter second date later then first date!");
        }
    }
}
