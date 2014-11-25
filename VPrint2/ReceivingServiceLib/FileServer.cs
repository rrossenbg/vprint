/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;
using System.IO;
using System.Net.Security;
using System.ServiceModel;
using System.ServiceModel.Channels;
using VPrinting;

namespace ReceivingServiceLib
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, AddressFilterMode = AddressFilterMode.Any)]
    [ErrorHandlingBehavior(ExceptionToFaultConverter = typeof(MyServiceFaultProvider))]
    public class FileServer
    {
        public static event EventHandler<ValueEventArgs<Tuple<string, string, DateTime>>> NewCall;

        public FileDownloadMessage DownloadFile(FileDownloadMessage msg)
        {
            try
            {
                SecurityCheckThrow(msg.s1, msg.s2);
                RecordCallHistory("DownloadFile");

                FileDownloadMessage download = new FileDownloadMessage();
                Stream fileStream = new FileStream(msg.Filename, FileMode.Open);
                download.FileByteStream = fileStream;
                return download;
            }
            catch (Exception ex)
            {
                throw new FaultException<MyApplicationFault>(new MyApplicationFault(), ex.Message);
            }
        }

        public void UploadFile(FileUploadMessage msg)
        {
            try
            {
                SecurityCheckThrow(msg.s1, msg.s2);
                RecordCallHistory("UploadFile");

                var uploadRootFolder = new DirectoryInfo(Global.Strings.UPLOADROOT);
                var sessionDir = uploadRootFolder.Combine(msg.SessionId);
                sessionDir.EnsureDirectory();
                var fileInfo = sessionDir.CombineFileName(msg.FileName);

                using (var file = fileInfo.Open(FileMode.Create))
                    msg.FileByteStream.CopyTo(file);
            }
            catch (Exception ex)
            {
                throw new FaultException<MyApplicationFault>(new MyApplicationFault(), ex.Message);
            }
        }

        #region SECURITY

        private void SecurityCheckThrow(string s1, string s2)
        {
            if (!SecurityCheck(s1, s2))
                throw new Exception("Wrong caller id");
        }

        private bool SecurityCheck(string s1, string s2)
        {
            var e1 = s1.DecryptString();
            var e2 = s2.DecryptString();
            var re2 = e2.Reverse();
            return string.Equals(e1, re2, StringComparison.InvariantCultureIgnoreCase);
        }

        private void RecordCallHistory(string method)
        {
            string ip = GetClientIP();
            FireNewCall(ip, method);
        }

        private string GetClientIP()
        {
            OperationContext context = OperationContext.Current;
            MessageProperties messageProperties = context.IncomingMessageProperties;
            RemoteEndpointMessageProperty endpointProperty = messageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            return endpointProperty.Address;
        }

        private void FireNewCall(string ip, string method)
        {
            if (NewCall != null)
                NewCall(this, new ValueEventArgs<Tuple<string, string, DateTime>>(new Tuple<string, string, DateTime>(ip, method, DateTime.Now)));
        }

        #endregion
    }

    [MessageContract]
    public class FileDownloadMessage
    {
        [MessageHeader(MustUnderstand = true)]
        public string Filename;
        [MessageHeader(MustUnderstand = true, ProtectionLevel = ProtectionLevel.EncryptAndSign)]
        public string s1;
        [MessageHeader(MustUnderstand = true, ProtectionLevel = ProtectionLevel.EncryptAndSign)]
        public string s2;

        [MessageHeader]
        public string SessionId;

        [MessageBodyMember(Order = 1)]
        public Stream FileByteStream;
    }

    [MessageContract]
    public class FileUploadMessage
    {
        [MessageHeader(MustUnderstand = true)]
        public string FileName;
        [MessageHeader(MustUnderstand = true, ProtectionLevel = ProtectionLevel.EncryptAndSign)]
        public string s1;
        [MessageHeader(MustUnderstand = true, ProtectionLevel = ProtectionLevel.EncryptAndSign)]
        public string s2;

        [MessageHeader]
        public string SessionId;

        [MessageBodyMember(Order = 1)]
        public Stream FileByteStream;
    }
}
