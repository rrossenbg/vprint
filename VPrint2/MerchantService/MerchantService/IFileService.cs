/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System.ServiceModel;
using ReceivingServiceLib;

namespace MerchantService
{
    [ServiceContract]
    public interface IFileService
    {
        [OperationContract]
        [FaultContract(typeof(MyApplicationFault))]
        FileMessage2 DownloadFile2(FileInfo2 message);

        [OperationContract]
        [FaultContract(typeof(MyApplicationFault))]
        FileMessage5 DownloadFile5(FileInfo5 message);

        [OperationContract]
        [FaultContract(typeof(MyApplicationFault))]
        void UploadFile2(FileMessage2 message);

        [OperationContract]
        [FaultContract(typeof(MyApplicationFault))]
        void UploadFile5(FileMessage5 message);
    }
}
