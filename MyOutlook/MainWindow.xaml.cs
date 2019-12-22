using MailKit;
using MailKit.Search;
using MyOutlook.AccountModel;
using MyOutlook.AcountModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;

namespace MyOutlook
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string SenderEmailAddress { get; set; }
        public List<List<IMailFolder>> AccountsSubfolders { get; set; }
        public List<MessageParts> MessagesListTemp { get; set; }
        public int MessageBodyFont { get; set; } 
        public List<AddressBookLine> myAddrBook { get; set; }
        public int IndexOfDynamicList { get; set; }
        public string FolderName { get; set; }
        public BinarySearchQuery query { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            AccountsSubfolders = new List<List<IMailFolder>>();
            AccountsAdding();
            MessagesListTemp = new List<MessageParts>();
            // OperateData.Operating.DownloadBodyParts();
            MessageBodyFont = 14;
            myAddrBook = new List<AddressBookLine>();
            myAddrBook = OperateData.Operating.AddressBookUpload();
        }

        private void ListeBoxSelectedChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedList = sender as ListBox;

            if (selectedList != null)
            {
                SenderEmailAddress = selectedList.Items[0].ToString();
                string indexOfList = selectedList.Name;
                indexOfList = indexOfList.Substring(indexOfList.Length-1);
                IndexOfDynamicList = Convert.ToInt32(indexOfList);
                FolderName = selectedList.SelectedItem.ToString();
                //MessageBox.Show(FolderName);

            }
        }

        private async void SendMessageMenu_Click(Object sender, System.EventArgs e)
        {
            MessageWindow myMessage = new MessageWindow(SenderEmailAddress, myAddrBook, OperateData.Operating.AccountsUpload());
            if (myMessage.ShowDialog() == true)
            {
                try
                {
                    List<string> ReceipentEmails = new List<string>();
                    List<Attach> AttachmentsList = new List<Attach>();
                    AttachmentsList = myMessage.AttachmentsList;

                    ReceipentEmails.AddRange(OperateData.Operating.AccountsUpload(myMessage.AddressWhom.Text));
                    if (myMessage.AddressCopy.Text !="")
                        ReceipentEmails.AddRange(OperateData.Operating.AccountsUpload(myMessage.AddressCopy.Text));

                    await OperateData.Operating.MessageSendingAsync(myMessage.AddressFrom.Text, ReceipentEmails, AttachmentsList,
                    myMessage.MessageTheme, myMessage.MessageBody);

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

        }

        
        private async void RefreshFoldersMenu_Click(Object sender, System.EventArgs e)
        {
            messagesDetailsDataGrid.ItemsSource = null;
            //MessagesListTemp = OperateData.Operating.DownloadBodyParts();
            query = null;
            MessagesListTemp = await OperateData.Operating.CustomsDownloadBodyPartsAsync(OperateData.Operating.AccountsUpload()[IndexOfDynamicList], FolderName, query);
            messagesDetailsDataGrid.ItemsSource = MessagesListTemp;
            if (MessagesListTemp.Count > 0)
            AddingMessagePartsToWindow(MessagesListTemp[0]);
            else MessageBox.Show("Selected folder is empty!");
        }

        private void ExitMenu_Click(Object sender, System.EventArgs e)
        {
            OperateData.Operating.SaveAddressBook(myAddrBook);
            this.Close();
        }

        private void SettingsMenu_Click(Object sender, System.EventArgs e)
        {
            SettingWindow mySettings = new SettingWindow();
            if (mySettings.ShowDialog() == true)
            {
                try
                {
                Account myAcc = new Account();
                myAcc.SMPTServerDetails = mySettings.ServerDetailsTbx.Text;
                myAcc.SMPTPortDetails = Convert.ToInt32(mySettings.PortDetailsTbx.Text);
                myAcc.SMPTEmailAddress = mySettings.EmailAddressTbx.Text;
                myAcc.SMPTEmailBoxPassword = mySettings.EmailBoxPasswordTbx.Text;
                myAcc.SMPTUserFirstName = mySettings.FirstNameTbx.Text;
                myAcc.SMPTUserSecondName = mySettings.SecondNameTbx.Text;

                myAcc.IMAPServerDetails = mySettings.IncServerDetailsTbx.Text;
                myAcc.IMAPPortDetails = Convert.ToInt32(mySettings.IncPortDetailsTbx.Text);
                myAcc.IMAPEmailAddress = mySettings.IncEmailAddressTbx.Text;
                myAcc.IMAPEmailBoxPassword = mySettings.IncEmailBoxPasswordTbx.Text;

                if (OperateData.Operating.AccountsSave(myAcc) == true)
                    {
                        AccountsAdding();
                        MessageBox.Show("Account saved succesfully!");
                    }
                    else
                        MessageBox.Show("We could not save account!");

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
               
       private async void GetMessagesForperiodMenuClick(Object sender, System.EventArgs e)
        {
            SearchRequestDatesWindow mySearchDates = new SearchRequestDatesWindow();

            if (mySearchDates.ShowDialog() == true)
            {
                if (mySearchDates.ToDatePicker.SelectedDate > mySearchDates.FromDatePicker.SelectedDate)
                {
                  query = SearchQuery.DeliveredAfter(mySearchDates.FromDatePicker.SelectedDate.Value.Date).And(SearchQuery.DeliveredBefore(mySearchDates.ToDatePicker.SelectedDate.Value.Date));
                    MessagesListTemp = await OperateData.Operating.CustomsDownloadBodyPartsAsync(OperateData.Operating.AccountsUpload()[IndexOfDynamicList], FolderName, query);
                    messagesDetailsDataGrid.ItemsSource = MessagesListTemp;
                    if (MessagesListTemp.Count > 0)
                        AddingMessagePartsToWindow(MessagesListTemp[0]);
                    else MessageBox.Show("Selected folder is empty!");
                }

            }

        }

        private async void GetMessgesFromSenderMenuClick(Object sender, System.EventArgs e)
        {
            SearchRequestStringsWindow mySearchString = new SearchRequestStringsWindow("Enter Email Address To Sort Messages With It");

            if (mySearchString.ShowDialog() == true)
            {
                if (mySearchString.SortRequestTbx.Text != "")
                {
                    query = SearchQuery.FromContains(mySearchString.SortRequestTbx.Text).Or(SearchQuery.CcContains(mySearchString.SortRequestTbx.Text));
                    MessagesListTemp = await OperateData.Operating.CustomsDownloadBodyPartsAsync(OperateData.Operating.AccountsUpload()[IndexOfDynamicList], FolderName, query);
                    messagesDetailsDataGrid.ItemsSource = MessagesListTemp;
                    if (MessagesListTemp.Count > 0)
                        AddingMessagePartsToWindow(MessagesListTemp[0]);
                    else MessageBox.Show("Selected folder is empty!");
                }
                else MessageBox.Show("Enter a Request to Sort Messages!");

            }
        }

        private async void GetMessgesContainingMenuClick(Object sender, System.EventArgs e)
        {

            SearchRequestStringsWindow mySearchString = new SearchRequestStringsWindow();

            if (mySearchString.ShowDialog() == true)
            {
                if (mySearchString.SortRequestTbx.Text != "")
                {
                    query = SearchQuery.SubjectContains(mySearchString.SortRequestTbx.Text).Or(SearchQuery.BodyContains(mySearchString.SortRequestTbx.Text));
                    MessagesListTemp = await OperateData.Operating.CustomsDownloadBodyPartsAsync(OperateData.Operating.AccountsUpload()[IndexOfDynamicList], FolderName, query);
                    messagesDetailsDataGrid.ItemsSource = MessagesListTemp;
                    if (MessagesListTemp.Count > 0)
                        AddingMessagePartsToWindow(MessagesListTemp[0]);
                    else MessageBox.Show("Selected folder is empty!");
                }
                else MessageBox.Show("Enter a Request to Sort Messages!");

            }

        }

        public void AccountsAdding()
        {
            List<Account> tempList = new List<Account>();
            tempList = OperateData.Operating.AccountsUpload();
            if(AccountsSubfolders.Count > 0) AccountsSubfolders.Clear();
            if (tempList.Count > 0)
            {
                int counter = 0;
                ClearChildrenFromCell();
                foreach (var element in tempList) // adding subfolders for every account
                    AccountsSubfolders.Add(OperateData.Operating.GettingImapSubfoldersList(element));

                foreach (var element in tempList)
                {
                    ListBox listBox = new ListBox();
                    TextBlock myTB = new TextBlock();

                    myTB.Text = element.SMPTEmailAddress;
                    //myTB.FontWeight = FontWeights.Bold;
                    myTB.Inlines.Add(new Bold(new Run("Bold")));
                    listBox.Name = "listBox" + counter.ToString();
                    listBox.Items.Add(myTB.Text);
                   
                    foreach (IMailFolder folder in AccountsSubfolders[counter])
                    {
                        listBox.Items.Add(folder.Name);
                    }

                    listBox.VerticalAlignment = VerticalAlignment.Top;
                    listBox.Background = new SolidColorBrush(Color.FromRgb(204, 230, 255));
                    listBox.SelectionChanged += ListeBoxSelectedChanged;

                    if (counter > 0)
                        listBox.Margin = new Thickness(0, 135, 0, 0);
                    Grid.SetColumn(listBox, 0);
                    Grid.SetRow(listBox, 1);
                    ContextMenu contextMenu = new ContextMenu();
                    MenuItem item = new MenuItem();
                    item.Header = "Remove MailBox";
                    item.Name = "RemoveMailBox";
                    item.Click += DeletingAccount;
                    contextMenu.Items.Add(item);
                    listBox.ContextMenu = contextMenu;

                    ScrollViewer sv = new ScrollViewer();
                    sv.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                    Grid.SetColumn(sv, 0);
                    Grid.SetRow(sv, 1);
                    MainGrid.Children.Add(sv);

                    MainGrid.Children.Add(listBox);
                    counter++;
                }

            }
            
        }

        public void ClearChildrenFromCell() // deleting accounts from specific cell
        {
            int getRow = 1, getCol = 0;
            for (int i = 0; i < MainGrid.Children.Count; i++)
                if ((Grid.GetRow(MainGrid.Children[i]) == getRow)
                    && (Grid.GetColumn(MainGrid.Children[i]) == getCol))
                {
                    MainGrid.Children.Remove(MainGrid.Children[i]);
                    break;
                }
        }

        private void DeletingAccount(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;
            if (menuItem.Name == "RemoveMailBox" && SenderEmailAddress != "")
            {
                 if (OperateData.Operating.AccountDelete(SenderEmailAddress) == true)
                {
                    ClearChildrenFromCell();
                    AccountsAdding();
                    MessageBox.Show("The account is deleted!");
                }
                else
                    MessageBox.Show("Error in account deleting!");
            }

        }

        private async Task ExpandFolderAsync(Task task, object state)
        {
            var folder = (IMailFolder)state;
            await task;

            var subfolders = await folder.GetSubfoldersAsync();
            //await LoadSubfoldersAsync(folder);
        }

        private void datagrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems != null && e.AddedItems.Count != 0)
            {
                foreach (MessageParts item in e.AddedItems)
                {
                    AddingMessagePartsToWindow(item);
                }
            }

        }

        private void AddingMessagePartsToWindow(MessageParts item)
        {
            MessageTheme.Text = item.Subject;
            MessageFrom.Text = item.From;
            MessageTime.Text = item.Date;
            MessageWhom.Text = item.To;
            MessageBody.Text = item.Body;
        }

        private void AddressBookMenuClick(Object sender, System.EventArgs e)
        {
            AddressBook myBook = new AddressBook(myAddrBook);
            myBook.Owner = this;
            myBook.Show();
        }

        private void MessageOnAccountInvoke(object sender, RoutedEventArgs e)
        {
            if(OperateData.Operating.AccountsUpload().Count < 1)
            MessageBox.Show("Please, enter at least one Email account!");
        }


        

       

        //async Task LoadSubfoldersAsync(IMailFolder folder)
        //{
        //    var subfolders = await folder.GetSubfoldersAsync();
        //    var sorted = new List<IMailFolder>(subfolders);

        //    sorted.Sort(new FolderComparer());

        //    await LoadSubfoldersAsync(folder, sorted);
        //}

        //public event EventHandler<FolderSelectedEventArgs> FolderSelected;

        //protected override void OnSelectedValueChanged(EventArgs e)
        //{
        //    var handler = FolderSelected;

        //    if (handler != null)
        //        handler(this, new FolderSelectedEventArgs((IMailFolder)e.Name));

        //    base.OnAfterSelect(e);
        //}

    }
}