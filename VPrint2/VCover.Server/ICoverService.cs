using System;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace VCover.Server
{
    [ServiceContract]
    public interface ICoverService
    {
        [OperationContract]
        string[] ReadData(Guid id);

        [OperationContract]
        void SaveData(Guid id, string imagePath);

        //[OperationContract]
        //CompositeType GetDataUsingDataContract(CompositeType composite);
    }

    [DataContract]
    public class CompositeType
    {
        bool boolValue = true;
        string stringValue = "Hello ";

        [DataMember]
        public bool BoolValue
        {
            get { return boolValue; }
            set { boolValue = value; }
        }

        [DataMember]
        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
        }
    }
}
