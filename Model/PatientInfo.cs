using System.Runtime.Serialization;

namespace PatientManagement.Model
{
    [DataContract]
    public class PatientInfo
    {
        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public string LastName { get; set; }
        [DataMember]
        public string Status { get; set; }
        [DataMember]
        public string MailId { get; set; }
        [DataMember]
        public string BloodGroup { get; set; }
        [DataMember]
        public int Age { get; set; }
        [DataMember]
        public long PatientId { get; set; }
        [DataMember]
        public byte[] PatientDocument { get; set; }
        [DataMember]
        public string PatientDocumentName { get; set; }
    }
}
