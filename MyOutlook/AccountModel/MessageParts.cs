using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyOutlook.AccountModel
{
    public class MessageParts
    {
        public string Uid { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Cc { get; set; }
        public string Date { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

    }
}
