using System;
using System.Runtime.Serialization.Json;
using System.IO;
using MyOutlook.AcountModel;
using System.Collections.Generic;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Controls;
using MyOutlook.AccountModel;
using MailKit.Net.Imap;
using MailKit.Security;
using MailKit;
using MailKit.Search;
using MimeKit;
using System.Threading;
using System.Windows.Data;
using System.Linq;

namespace MyOutlook.OperateData
{ 
    public static class Operating
    {
        static IList<IMessageSummary> MessagesList { get; set; }
        static Operating() { }

        //---------------------SMTP--------------------------------------------
        public static bool AccountsSave(Account account)
        {
            List<Account> myAccounts = new List <Account>(3);
            myAccounts = AccountsUpload();
            if (myAccounts.Count > 2)
            {
                return false;
            }
            else
            {
                myAccounts.Add(account);
                DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(List<Account>));
                using (FileStream fs = new FileStream("AccountsData.json", FileMode.OpenOrCreate))
                {
                    jsonFormatter.WriteObject(fs, myAccounts);
                    return true;
                }
            }
        }

        public static List<Account> AccountsUpload()
        {
            List<Account> myAccounts = new List<Account>(3);
            DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(List<Account>));
            try
            {
                using (FileStream fs = new FileStream("AccountsData.json", FileMode.OpenOrCreate))
                {
                    myAccounts = (List<Account>)jsonFormatter.ReadObject(fs);
                }
            }
            catch (Exception ex) { }
            return myAccounts;
        }

        public static bool AccountDelete(string str)
        {
            List<Account> myAccounts = new List<Account>(3);
            myAccounts = AccountsUpload();
            for(int i = 0; i< myAccounts.Count; i++)
            {
                if (myAccounts[i].SMPTEmailAddress == str)
                { myAccounts.RemoveAt(i);
                    DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(List<Account>));
                    using (FileStream fs = new FileStream("AccountsData.json", FileMode.Create))
                    {
                        jsonFormatter.WriteObject(fs, myAccounts);
                    }
                    return true;
                };
            }
            
            return false;
        }

        public static async Task<bool> MessageSendingAsync(string SenderEmailAddress, List<string> ReceipentEmails, List<Attach> AttachmentsList,
            TextBox Theme, TextBox BodyMessage)
        {
            List<Account> myAccounts = new List<Account>(3);
            myAccounts = AccountsUpload();
            Account accountToSendFrom = myAccounts.Find(i => i.SMPTEmailAddress == SenderEmailAddress);

            MailAddress from = new MailAddress(accountToSendFrom.SMPTEmailAddress, accountToSendFrom.SMPTUserSecondName + " " + accountToSendFrom.SMPTUserFirstName);
            SmtpClient smtp = new SmtpClient(accountToSendFrom.SMPTServerDetails, accountToSendFrom.SMPTPortDetails);
            smtp.Credentials = new NetworkCredential(accountToSendFrom.SMPTEmailAddress, accountToSendFrom.SMPTEmailBoxPassword);
            smtp.EnableSsl = true;
            try
            {
                foreach (var receipent in ReceipentEmails)
                {
                    MailAddress to = new MailAddress(receipent);
                    MailMessage m = new MailMessage(from, to);
                    m.Subject = Theme.Text;
                    m.Body = BodyMessage.Text;
                    if (AttachmentsList.Count > 0)
                    {
                        foreach (var file in AttachmentsList)
                        {
                            m.Attachments.Add(new Attachment(file.CompleteFilePath));
                        }
                    }
                    await smtp.SendMailAsync(m);
                    //Console.WriteLine("Письмо отправлено");
                }

                return true;
            }
            catch (Exception ex) { }

            return false;
        }

        public static List<string> AccountsUpload(string emailsFromTextBox)
        {
            List<string> ReceipentEmails = new List<string>();
            string[] emailAddresses = emailsFromTextBox.Split(';').Select(x => x.Trim()).ToArray();

            foreach (string emailAddress in emailAddresses)
            {
                ReceipentEmails.Add(emailAddress);
            }
            return ReceipentEmails;
        }

        
        //---------------------IMAP------MailKit-------------------------------

       
        public static async Task<List<MessageParts>> CustomsDownloadBodyPartsAsync(Account element, string FolderName, BinarySearchQuery queryCustom)
        {
            List<MessageParts> MessagesListRender = new List<MessageParts>();
            List<IMailFolder> myListFolders = new List<IMailFolder>();
            using (var client = new ImapClient())
            {
                await client.ConnectAsync(element.IMAPServerDetails, element.IMAPPortDetails, SecureSocketOptions.SslOnConnect);
                await client.AuthenticateAsync(element.IMAPEmailAddress, element.IMAPEmailBoxPassword);
                await client.Inbox.OpenAsync(FolderAccess.ReadOnly);

                var personal = client.GetFolder(client.PersonalNamespaces[0]);

                IMailFolder CompleteFolder = personal.GetSubfolders()[1];

                foreach (var folder in CompleteFolder.GetSubfolders())
                {
                    myListFolders.Add(folder);
                }

                int indexOfFolder = myListFolders.FindIndex(a => a.Name == FolderName);
                if (indexOfFolder > 0)
                {
                    await myListFolders[indexOfFolder].OpenAsync(FolderAccess.ReadOnly);

                    if (queryCustom == null)
                    {
                        DateTime DateNow = new DateTime();
                        DateNow = DateTime.Now;
                        var query = SearchQuery.DeliveredAfter(new DateTime(2019, 05, 01)).And(SearchQuery.DeliveredBefore(DateNow));
                        var uids = await myListFolders[indexOfFolder].SearchAsync(query);
                        const MessageSummaryItems SummaryItems = MessageSummaryItems.UniqueId | MessageSummaryItems.Envelope | MessageSummaryItems.Flags | MessageSummaryItems.BodyStructure;
                        var timeout = new CancellationToken();
                        MessagesList = await myListFolders[indexOfFolder].FetchAsync(uids, SummaryItems, timeout);
                    }
                    if(queryCustom != null)
                    {
                        var uids = myListFolders[indexOfFolder].Search(queryCustom);
                        const MessageSummaryItems SummaryItems = MessageSummaryItems.UniqueId | MessageSummaryItems.Envelope | MessageSummaryItems.Flags | MessageSummaryItems.BodyStructure;
                        var timeout = new CancellationToken();
                        MessagesList = await myListFolders[indexOfFolder].FetchAsync(uids, SummaryItems, timeout);
                    }

                    foreach (var message in MessagesList)
                    {
                        MessageParts temp = new MessageParts();
                        temp.Uid = message.UniqueId.ToString();

                        var bodyPart = message.TextBody;
                        var body = (TextPart)myListFolders[indexOfFolder].GetBodyPart(message.UniqueId, bodyPart);
                        temp.Body = body.Text;

                        string str = message.Envelope.Date.ToString();
                        int index = str.IndexOf("+") > 0 ? str.IndexOf("+") : str.IndexOf("-");
                        temp.Date = str.Substring(0, index);

                        temp.From = message.Envelope.From.ToString();
                        temp.Cc = message.Envelope.Cc.ToString();
                        temp.Subject = message.Envelope.Subject.ToString();
                        temp.To = message.Envelope.To.ToString();
                        MessagesListRender.Add(temp);
                    }
                }
                client.Disconnect(true);
            }
            return MessagesListRender;
        }


        public static List<IMailFolder> GettingImapSubfoldersList(Account element)
        {
            List<IMailFolder> myListFolders = new List<IMailFolder>();

            using (var client = new ImapClient())
            {
                client.Connect(element.IMAPServerDetails, element.IMAPPortDetails, SecureSocketOptions.SslOnConnect);

                client.Authenticate(element.IMAPEmailAddress, element.IMAPEmailBoxPassword);

                client.Inbox.Open(FolderAccess.ReadOnly);

                var personal = client.GetFolder(client.PersonalNamespaces[0]);

                IMailFolder CompleteFolder = personal.GetSubfolders()[1];

                foreach (var folder in CompleteFolder.GetSubfolders())
                {
                    myListFolders.Add(folder);
                }

                client.Disconnect(true);
            }
                return myListFolders;
        }


    //----------------------------AddressBook-------------------------------

        public static bool SaveAddressBook(List<AddressBookLine> myAddrBook)
        {
            DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(List<AddressBookLine>));
            using (FileStream fs = new FileStream("AddressBook.json", FileMode.OpenOrCreate))
            {
                jsonFormatter.WriteObject(fs, myAddrBook);
                return true;
            }
        }

        public static List<AddressBookLine> AddressBookUpload()
        {
            List<AddressBookLine> myAddrBook = new List<AddressBookLine>();
            DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(List<AddressBookLine>));
            try
            {
                using (FileStream fs = new FileStream("AddressBook.json", FileMode.OpenOrCreate))
                {
                    myAddrBook = (List<AddressBookLine>)jsonFormatter.ReadObject(fs);
                }
            }
            catch (Exception ex) { }
            return myAddrBook;
        }


    }


    }

