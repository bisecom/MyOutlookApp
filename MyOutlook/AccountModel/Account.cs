using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;

namespace MyOutlook.AcountModel
{
    [DataContract]
    public class Account
    {
        [DataMember]
        public string SMPTServerDetails { get; set; }
        [DataMember]
        public int SMPTPortDetails { get; set; }
        [DataMember]
        public string SMPTEmailAddress { get; set; }
        [DataMember]
        public string SMPTEmailBoxPassword { get; set; }

        [DataMember]
        public string SMPTUserFirstName { get; set; }
        [DataMember]
        public string SMPTUserSecondName { get; set; }

        [DataMember]
        public string IMAPServerDetails { get; set; }
        [DataMember]
        public int IMAPPortDetails { get; set; }
        [DataMember]
        public string IMAPEmailAddress { get; set; }
        [DataMember]
        public string IMAPEmailBoxPassword { get; set; }

        public Account(string SMPTServerDetails, int SMPTPortDetails, string SMPTEmailAddress, string SMPTEmailBoxPassword, string SMPTUserFirstName,
            string SMPTUserSecondName, string IMAPServerDetails, int IMAPPortDetails, string IMAPEmailAddress, string IMAPEmailBoxPassword)
        {
            this.SMPTServerDetails = SMPTServerDetails;
            this.SMPTPortDetails = SMPTPortDetails;
            this.SMPTEmailAddress = SMPTEmailAddress;
            this.SMPTEmailBoxPassword = SMPTEmailBoxPassword;
            this.SMPTUserFirstName = SMPTUserFirstName;
            this.SMPTUserSecondName = SMPTUserSecondName;

            this.IMAPServerDetails = IMAPServerDetails;
            this.IMAPPortDetails = IMAPPortDetails;
            this.IMAPEmailAddress = IMAPEmailAddress;
            this.IMAPEmailBoxPassword = IMAPEmailBoxPassword;

    }
        public Account() { }


    }
}
