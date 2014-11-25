/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;
using System.IO;
using System.ServiceModel;
using System.Net.Security;
using System.ServiceModel.Channels;
using VPrinting;

namespace ReceivingServiceLib
{
    [ServiceContract]
    public interface IFileServer
    {
        [OperationContract]
        [FaultContract(typeof(MyApplicationFault))]
        FileDownloadMessage DownloadFile(FileDownloadMessage message);

        [OperationContract]
        [FaultContract(typeof(MyApplicationFault))]
        void UploadFile(FileUploadMessage message);
    }    
}