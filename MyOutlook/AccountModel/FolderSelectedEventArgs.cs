using MailKit;
using System;

namespace MyOutlook.AccountModel
{

    class FolderSelectedEventArgs : EventArgs
    {
        public FolderSelectedEventArgs(IMailFolder folder)
        {
            Folder = folder;
        }

        public IMailFolder Folder
        {
            get; private set;
        }
    }
}
