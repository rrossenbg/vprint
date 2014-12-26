/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;
using System.IO;
using System.Net.Security;
using System.ServiceModel;
using System.ServiceModel.Channels;
using ReceivingServiceLib;
using VPrinting;

namespace MerchantService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, AddressFilterMode = AddressFilterMode.Any)]
    [ErrorHandlingBehavior(ExceptionToFaultConverter = typeof(MyServiceFaultProvider))]
    public class FileService : IFileService
    {
        public static Strings Strings { get; set; }

        public static event EventHandler<ValueEventArgs<Tuple<string, string, DateTime>>> NewCall;

        static FileService()
        {
            Strings = Strings.Read();
        }

        public FileMessage2 DownloadFile2(FileInfo2 msg)
        {
            try
            {
                SecurityCheckThrow(msg.s1, msg.s2);
                RecordCallHistory("DownloadFile");

                DataAccess da = new DataAccess();
                var vinfo = da.SelectVoucherInfo(msg.CountryId, msg.VoucherId);

                ZipFileAccess zipda = new ZipFileAccess();
                var voucherFolder = zipda.CreateDirectoryHerarchy(Strings.FILESERVERFOLDER, msg.CountryId, vinfo.branch_id, msg.VoucherId);
                var sessionFileName = voucherFolder.CombineFileName(vinfo.session_Id);

                FileMessage2 download = new FileMessage2(msg);

                if (sessionFileName.Exists)
                {
                    Stream fileStream = new FileStream(sessionFileName.FullName, FileMode.Open);
                    download.FileByteStream = fileStream;
                }
                return download;
            }
            catch (Exception ex)
            {
                throw new FaultException<MyApplicationFault>(new MyApplicationFault(), ex.Message);
            }
        }

        public FileMessage5 DownloadFile5(FileInfo5 msg)
        {
            try
            {
                SecurityCheckThrow(msg.s1, msg.s2);
                RecordCallHistory("DownloadFile");

                ZipFileAccess da = new ZipFileAccess();
                var voucherFolder = da.CreateDirectoryHerarchy(Strings.FILESERVERFOLDER, msg.CountryId, msg.RetailerId, msg.VoucherId);
                var sessionFileName = voucherFolder.CombineFileName(msg.SessionId);

                FileMessage5 download = new FileMessage5(msg);

                if (sessionFileName.Exists)
                {
                    Stream fileStream = new FileStream(sessionFileName.FullName, FileMode.Open);
                    download.FileByteStream = fileStream;
                }
                return download;
            }
            catch (Exception ex)
            {
                throw new FaultException<MyApplicationFault>(new MyApplicationFault(), ex.Message);
            }
        }

        public void UploadFile5(FileMessage5 msg)
        {
            try
            {
                SecurityCheckThrow(msg.s1, msg.s2);
                RecordCallHistory("UploadFile");

                ZipFileAccess zipda = new ZipFileAccess();
                var voucherFolder = zipda.CreateDirectoryHerarchy(Strings.FILESERVERFOLDER, msg.CountryId, msg.RetailerId, msg.VoucherId);
                var sessionFileName = voucherFolder.CombineFileName(msg.SessionId);

                using (var file = sessionFileName.Open(FileMode.Create))
                    msg.FileByteStream.CopyTo(file);
            }
            catch (Exception ex)
            {
                throw new FaultException<MyApplicationFault>(new MyApplicationFault(), ex.Message);
            }
        }

        public void UploadFile2(FileMessage2 msg)
        {
            try
            {
                SecurityCheckThrow(msg.s1, msg.s2);
                RecordCallHistory("UploadFile");

                DataAccess da = new DataAccess();
                var vinfo = da.SelectVoucherInfo(msg.CountryId, msg.VoucherId);

                ZipFileAccess zipda = new ZipFileAccess();
                var voucherFolder = zipda.CreateDirectoryHerarchy(Strings.FILESERVERFOLDER, msg.CountryId, vinfo.branch_id, msg.VoucherId);
                var sessionFileName = voucherFolder.CombineFileName(vinfo.session_Id);

                using (var file = sessionFileName.Open(FileMode.Create))
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
    public class FileMessage5
    {
        [MessageHeader(MustUnderstand = true)]
        public string Filename;

        [MessageHeader(MustUnderstand = true, ProtectionLevel = ProtectionLevel.EncryptAndSign)]
        public string s1;
        [MessageHeader(MustUnderstand = true, ProtectionLevel = ProtectionLevel.EncryptAndSign)]
        public string s2;

        [MessageHeader]
        public string SessionId;

        [MessageHeader]
        public int CountryId;

        [MessageHeader]
        public int RetailerId;

        [MessageHeader]
        public int VoucherId;

        [MessageBodyMember(Order = 1)]
        public Stream FileByteStream;

        public FileMessage5()
        {
        }

        public FileMessage5(FileInfo5 info)
        {
            SessionId = info.SessionId;
            CountryId = info.CountryId;
            RetailerId = info.RetailerId;
            CountryId = info.CountryId;
            VoucherId = info.VoucherId;
        }
    }

    [MessageContract]
    public class FileInfo5
    {
        [MessageHeader(MustUnderstand = true)]
        public string Filename;

        [MessageHeader(MustUnderstand = true, ProtectionLevel = ProtectionLevel.EncryptAndSign)]
        public string s1;
        [MessageHeader(MustUnderstand = true, ProtectionLevel = ProtectionLevel.EncryptAndSign)]
        public string s2;

        [MessageHeader]
        public string SessionId;

        [MessageHeader]
        public int CountryId;

        [MessageHeader]
        public int RetailerId;

        [MessageHeader]
        public int VoucherId;
    }

    [MessageContract]
    public class FileMessage2
    {
        [MessageHeader(MustUnderstand = true, ProtectionLevel = ProtectionLevel.EncryptAndSign)]
        public string s1;
        [MessageHeader(MustUnderstand = true, ProtectionLevel = ProtectionLevel.EncryptAndSign)]
        public string s2;

        [MessageHeader]
        public int CountryId;

        [MessageHeader]
        public int VoucherId;

        [MessageBodyMember(Order = 1)]
        public Stream FileByteStream;

        public FileMessage2()
        {
        }

        public FileMessage2(FileInfo2 info)
        {
            CountryId = info.CountryId;
            VoucherId = info.VoucherId;
        }
    }

    [MessageContract]
    public class FileInfo2
    {
        [MessageHeader(MustUnderstand = true, ProtectionLevel = ProtectionLevel.EncryptAndSign)]
        public string s1;
        [MessageHeader(MustUnderstand = true, ProtectionLevel = ProtectionLevel.EncryptAndSign)]
        public string s2;

        [MessageHeader]
        public int CountryId;

        [MessageHeader]
        public int VoucherId;
    }
}
