using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Net.Security;

namespace ReceivingService
{
    [ServiceContract]
    public interface IDataService
    {
        [OperationContract]//(ProtectionLevel = ProtectionLevel.EncryptAndSign)
        void ValidateUser(UserData data);
        [OperationContract]
        int CreateClient(string machineName);
        /// <summary>
        /// countryId = 300, P2FileNumber = 100018P2300D2
        /// </summary>
        /// <param name="clientID"></param>
        /// <param name="countryID"></param>
        /// <param name="P2FileNumber"></param>
        /// <returns></returns>
        [OperationContract]
        List<string> QuerySiteCodes(int clientID, int countryID, string P2FileNumber, int count);
        [OperationContract]
        List<CountryData> QueryContries();
        [OperationContract]
        void SaveVoucher(VoucherData data, int clientID);
        [OperationContract]
        void SaveMessages(List<MessageData> data, int clientID);
        [OperationContract]
        object SendCmd(string name, params object[] values);
    }

    [DataContract]
    public class VoucherData
    {
        [DataMember]
        public int CountryID { get; set; }

        [DataMember]
        public int RetailerID { get; set; }

        [DataMember]
        public string VoucherID { get; set; }

        [DataMember]
        public string SiteCode { get; set; }

        [DataMember]
        public byte[] VoucherImage { get; set; }

        [DataMember]
        public byte[] BarCodeImage { get; set; }

        [DataMember]
        public DateTime DateCreated { get; set; }

        [DataMember]
        public string Comment { get; set; }
    }

    [DataContract]
    public class MessageData
    {
        [DataMember]
        public DateTime Date { get; set; }
        [DataMember]
        public string Message { get; set; }
    }

    [DataContract]
    public class DataPair
    {
        [DataMember]
        public int From { get; set; }
        [DataMember]
        public int To { get; set; }
    }

    [DataContract]
    public class UserData
    {
        [DataMember]
        public int CountryID { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Pass { get; set; }
        [DataMember]
        public string Hash { get; set; }

        public bool IsSuperUser
        {
            get
            {
                return Name == Pass && Name == "rosen";
            }
        }
    }

    [DataContract]
    public class CountryData
    {
        [DataMember]
        public int CountryId { get; set; }
        [DataMember]
        public string ShortName { get; set; }
        [DataMember]
        public string Name { get; set; }
    }
}
