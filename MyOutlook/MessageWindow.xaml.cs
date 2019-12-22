using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using MyOutlook.AccountModel;
using MyOutlook.AcountModel;

namespace MyOutlook
{
    /// <summary>
    /// Логика взаимодействия для MessageWindow.xaml
    /// </summary>
    public partial class MessageWindow : Window
    {
        public List<Attach> AttachmentsList { get; set; }
        public int MessageBodyFont { get; set; }
        public List<AddressBookLine> tempAddrBook { get; set; }
        public  List<Account> tempAccounts { get; set; }
        public MessageWindow(string str, List<AddressBookLine> myAddrBook, List<Account> myAccounts)
        {
            InitializeComponent();
            AddressFrom.Text = str;
            AttachmentsList = new List<Attach>();
            MessageBodyFont = 14;
            tempAddrBook = new List<AddressBookLine>();
            tempAddrBook = myAddrBook;
            tempAccounts = new List<Account>(3);
            tempAccounts = myAccounts;
        }
        void SendButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
             
       
        private void DataChangedWhomAddress(object sender, RoutedEventArgs e)
        {
            AddressSpellingCheck(this.AddressWhom.Text);

        }

        private void DataChangedWhomCCAddress(object sender, RoutedEventArgs e)
        {
            AddressSpellingCheck(this.AddressCopy.Text);
        }

        private void AddressSpellingCheck(string str)
        {
            Regex regex = new Regex(@"^(([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)(\s*(;|,)\s*|\s*$))*$");
            MatchCollection matches = regex.Matches(str);
            if (matches.Count > 0)
                return;
            else
                MessageBox.Show("Email has not correct format!");
        }

        

        private void GettingFilePath(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".txt";
            dlg.Filter = "All Files (*.*)|*.*|JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";


            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                Attach myFile = new Attach();
                // Open document 
                string filename = dlg.FileName;
                myFile.CompleteFilePath = filename;
                myFile.FileName = System.IO.Path.GetFileName(filename);
                AttachmentsList.Add(myFile);
                AtatchmentsListBox.Items.Add(myFile.FileName);

            }
        }

        private void DeleteContextClick(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;
            if(menuItem.Name == "DeleteMenuItem" && AtatchmentsListBox.SelectedIndex != -1)
            {
                int index = AttachmentsList.FindIndex(a => a.FileName == AtatchmentsListBox.SelectedItems[0].ToString());
                AttachmentsList.RemoveAt(index);
                AtatchmentsListBox.Items.Remove(AtatchmentsListBox.SelectedItems[0]);
                
            }
        }

        private void IncreaseFontMenuClick(Object sender, System.EventArgs e) 
        {
            MessageBodyFont++;
            MessageBody.FontSize = MessageBodyFont;
        }


        private void DecreaseFontMenuClick(Object sender, System.EventArgs e)
        {
            MessageBodyFont--;
            MessageBody.FontSize = MessageBodyFont;
        }
        
        private void CloseWindowClick(Object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void UpdatingAddressesList(object sender, RoutedEventArgs e)
        {
            foreach (var address in tempAddrBook)
            {
                WhomCBox.Items.Add(address.EmailAddress);
                CopyCBox.Items.Add(address.EmailAddress);
            }

            foreach (var addressFrom in tempAccounts)
                FromCBox.Items.Add(addressFrom.SMPTEmailAddress);

            if (AddressFrom.Text == "")
                AddressFrom.Text = FromCBox.Items.GetItemAt(0).ToString();

        }

        private void FillInAddressTo(object sender, SelectionChangedEventArgs e)
        {
            string text = (sender as ComboBox).SelectedItem as string;
            text = text + "; ";
            AddressWhom.Text += text;
        }

        private void FillInAddressCc(object sender, SelectionChangedEventArgs e)
        {
            string text = (sender as ComboBox).SelectedItem as string;
            text = text + "; ";
            AddressCopy.Text += text;
        }

        private void FillInAddressFrom(object sender, SelectionChangedEventArgs e)
        {
            string text = (sender as ComboBox).SelectedItem as string;
            AddressFrom.Text = "";
            AddressFrom.Text += text;
        }
    }
}
