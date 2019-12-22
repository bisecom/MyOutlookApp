using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MyOutlook.AccountModel
{
    [DataContract]
    public class AddressBookLine
    {
        [DataMember]
        public string EmailAddress { get; set; }
        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public string SecondtName { get; set; }
        [DataMember]
        public string PhoneNumber { get; set; }

        public AddressBookLine()
        {
        }

    }
}
