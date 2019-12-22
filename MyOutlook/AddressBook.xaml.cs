using MyOutlook.AccountModel;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace MyOutlook
{
    /// <summary>
    /// Логика взаимодействия для AddressBook.xaml
    /// </summary>
    public partial class AddressBook : Window
    {
        public List<AddressBookLine> tempAddrBook { get; set; }
        public int TempIndexBookList { get; set; }
        public AddressBook(List<AddressBookLine> myAddrBook)
        {
            InitializeComponent();
            tempAddrBook = myAddrBook;
            AddrBookDGd.ItemsSource = tempAddrBook;
        }

        private void SaveBtnClick(Object sender, System.EventArgs e)
        {
            AddressBookLine myLine = new AddressBookLine();
            myLine.EmailAddress = EmailAddressTbx.Text;
            myLine.FirstName = FirstNameTbx.Text;
            myLine.SecondtName = SecondNameTbx.Text;
            myLine.PhoneNumber = PhoneNumberTbx.Text;
            tempAddrBook.Add(myLine);
            AddrBookDGd.ItemsSource = null;
            AddrBookDGd.ItemsSource = tempAddrBook;
        }

        private void EditBtnClick(Object sender, System.EventArgs e)
        {
            tempAddrBook[TempIndexBookList].EmailAddress = EmailAddressTbx.Text;
            tempAddrBook[TempIndexBookList].FirstName = FirstNameTbx.Text;
            tempAddrBook[TempIndexBookList].SecondtName = SecondNameTbx.Text;
            tempAddrBook[TempIndexBookList].PhoneNumber = PhoneNumberTbx.Text;
            AddrBookDGd.ItemsSource = null;
            AddrBookDGd.ItemsSource = tempAddrBook;
        }

        private void DeleteBtnClick(Object sender, System.EventArgs e)
        {
            tempAddrBook.RemoveAt(TempIndexBookList);
            AddrBookDGd.ItemsSource = null;
            AddrBookDGd.ItemsSource = tempAddrBook;
        }

        private void CloseBtnClick(Object sender, System.EventArgs e)
        {
            //((MainWindow)Application.Current.MainWindow).myAddrBook.Clear();
            ((MainWindow)Application.Current.MainWindow).myAddrBook = tempAddrBook;
            this.Close();
        }

        private void datagrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems != null && e.AddedItems.Count != 0)
            {
                AddressBookLine line = e.AddedItems[0] as AddressBookLine;
                EmailAddressTbx.Text = line.EmailAddress;
                FirstNameTbx.Text = line.FirstName;
                SecondNameTbx.Text = line.SecondtName;
                PhoneNumberTbx.Text = line.PhoneNumber;

                TempIndexBookList = GettingEmailIndexinList(line);

            }

        }


        private void DataChangedAddress(object sender, RoutedEventArgs e)
        {
            AddressSpellingCheck(this.EmailAddressTbx.Text);
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

        private int GettingEmailIndexinList(AddressBookLine line)
        {
            int index = tempAddrBook.FindIndex(a => a.EmailAddress == line.EmailAddress);
            return index;
        }
    }
}
