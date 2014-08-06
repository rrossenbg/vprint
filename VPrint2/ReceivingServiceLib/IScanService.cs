/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.ServiceModel;
using ReceivingServiceLib.Common.Data;
using ReceivingServiceLib.Data;

namespace ReceivingServiceLib
{
    [ServiceContract]
    public interface IScanService
    {
        #region SCAN

        [OperationContract]
        [FaultContract(typeof(MyApplicationFault))]
        void Delete(string fileName, int countryId, int retailerId, int voucherId, string s1, string s2);

        [OperationContract]
        [FaultContract(typeof(MyApplicationFault))]
        VoucherInfo2 ReadVoucherInfo(int Id, string copyToFolder, string s1, string s2);

        [OperationContract]
        [FaultContract(typeof(MyApplicationFault))]
        List<VoucherInfo> ReadData(int countryId, int retailerId, string s1, string s2);

        [OperationContract]
        [FaultContract(typeof(MyApplicationFault))]
        byte[] ReadData2(int id, bool isVoucher, int start, int length, string s1, string s2);

        [OperationContract]
        [FaultContract(typeof(MyApplicationFault))]
        void SaveData(string serverDirName, string fileName, byte[] data, string s1, string s2);

        [OperationContract]
        [FaultContract(typeof(MyApplicationFault))]
        void SaveDataAsync(string serverDirName, string fileName, byte[] data, long position, string s1, string s2);

        [OperationContract]//(IsOneWay = true)]
        [FaultContract(typeof(MyApplicationFault))]
        void CommitVoucherChanges(string serverDirName, int jobId, int countryId, int retailerId, int voucherId, int? folderId, string siteCode, string barCode, 
            int locationId, int userId, string s1, string s2);

        [OperationContract]
        [FaultContract(typeof(MyApplicationFault))]
        void CommitFileChanges(string serverDirName, int countryId, int? folderId,
            int locationId, int userId, string s1, string s2);

        [OperationContract]//(IsOneWay = true)]
        [FaultContract(typeof(MyApplicationFault))]
        void ValidateVoucher(int countryId, bool ss, int retailerId, int voucherId, bool voucherMustExist, string s1, string s2);

        [OperationContract]
        [FaultContract(typeof(MyApplicationFault))]
        string FindVoucher(int countryId, int voucherId, int voucherIdCD, string s1, string s2);

        #endregion

        #region REPORTS

        [OperationContract]//(Action = "*", ReplyAction = "*")]
        [FaultContract(typeof(MyApplicationFault))]
        void SaveHistory(int operatorCountryId, int operatorUserId, OperationHistory operationType, Guid operationId, 
            int brIsoId, int brId, int vId, int v2Id, int count, string details, string s1, string s2);

        [OperationContract]//(Action = "*", ReplyAction = "*")]
        [FaultContract(typeof(MyApplicationFault))]
        List<HistoryByCountryInfo> ReadHistory(int operatorCountryId, int? operatorUserId, OperationHistory operationType, DateTime from, DateTime to, string s1, string s2);

        [OperationContract]//(Action = "*", ReplyAction = "*")]
        [FaultContract(typeof(MyApplicationFault))]
        List<RetailerPrinterInfo> ReadRetailerPrinterInfo(int countryId, string s1, string s2);

        [OperationContract]//(Action = "*", ReplyAction = "*")]
        [FaultContract(typeof(MyApplicationFault))]
        List<fileInfo> SelectFilesBySql(string whereClause, string s1, string s2);

        [OperationContract]//(Action = "*", ReplyAction = "*")]
        [FaultContract(typeof(MyApplicationFault))]
        void UpdateVouchersOrFilesBySql(string setSql, string whereClause, bool isVoucher, string s1, string s2);

        #endregion

        #region FOLDERS

        [OperationContract]//(Action = "*", ReplyAction = "*")]
        [FaultContract(typeof(MyApplicationFault))]
        void AddFolder(int? toParentId, string name, int countryId, int userId, string s1, string s2);

        [OperationContract]//(Action = "*", ReplyAction = "*")]
        [FaultContract(typeof(MyApplicationFault))]
        void UpdateFolder(int folderId, string name, int? parentId, int countryId, int userId, string s1, string s2);

        [OperationContract]//(Action = "*", ReplyAction = "*")]
        [FaultContract(typeof(MyApplicationFault))]
        void DeleteFolder(int folderId, string s1, string s2);

        [OperationContract]//(Action = "*", ReplyAction = "*")]
        [FaultContract(typeof(MyApplicationFault))]
        void DeleteFile(int id, bool isVoucher, string s1, string s2);

        [OperationContract]//(Action = "*", ReplyAction = "*")]
        [FaultContract(typeof(MyApplicationFault))]
        void DeleteAllFilesInFolder(int folderId, string s1, string s2);

        [OperationContract]//(Action = "*", ReplyAction = "*")]
        [FaultContract(typeof(MyApplicationFault))]
        void RenameFolder(int folderId, string name, string s1, string s2);

        [OperationContract]//(Action = "*", ReplyAction = "*")]
        [FaultContract(typeof(MyApplicationFault))]
        List<FolderInfo> SelectFoldersByParent(int? parentId, int createdByIsoId, string s1, string s2);

        [OperationContract]//(Action = "*", ReplyAction = "*")]
        [FaultContract(typeof(MyApplicationFault))]
        List<fileInfo> SelectFilesByFolder(int folderId, string s1, string s2);

        [OperationContract]//(Action = "*", ReplyAction = "*")]
        [FaultContract(typeof(MyApplicationFault))]
        List<file2Info> SelectCoversByFolder(int folderId, string s1, string s2);

        [OperationContract]//(Action = "*", ReplyAction = "*")]
        [FaultContract(typeof(MyApplicationFault))]
        byte[] SelectFileById(int fileId, bool isVoucher, bool signed, int startFrom, string s1, string s2);

        #endregion

        #region VERSION UPDATE

        [OperationContract]//(Action = "*", ReplyAction = "*")]
        [FaultContract(typeof(MyApplicationFault))]
        List<UpdateFileInfo> GetVersionInfo(string currentVersion, string s1, string s2);

        [OperationContract]//(Action = "*", ReplyAction = "*")]
        [FaultContract(typeof(MyApplicationFault))]
        byte[] ReadVersionFile(string fileName, long from, string s1, string s2);

        #endregion

        #region TRANSFER FILE

        [OperationContract]//(Action = "*", ReplyAction = "*")]
        [FaultContract(typeof(MyApplicationFault))]
        List<TransferFileInfo> GetTransferFile(int countryId, int beginNumber, int endNumber, string siteCode, string s1, string s2);

        #endregion

        #region GENERAL

        ArrayList RetrieveTableData(string fieldList, string tableName, string where, string s1, string s2);

        int UpdateTableData(ArrayList table, string s1, string s2);

        #endregion

        #region PTF

        [OperationContract]
        [FaultContract(typeof(MyApplicationFault))]
        VoucherInfo3 FindVoucherTRSByVoucherNumber(int countryId, int voucherId, string s1, string s2);

        [OperationContract]
        [FaultContract(typeof(MyApplicationFault))]
        VoucherInfo3 FindVoucherTRSBySiteCode(string siteCode, int location, string s1, string s2);

        #endregion
    }

    #region DATA OBJECTS

    [DataContract]
    public class VoucherInfo
    {
        [DataMember(Order = 0)]
        public int Id { get; set; }
        [DataMember(Order = 1)]
        public int Number { get; set; }
        [DataMember(Order = 2)]
        public string SiteCode { get; set; }
        [DataMember(Order = 3)]
        public string BarCode { get; set; }
        [DataMember(Order = 4)]
        public DateTime ScanDate { get; set; }
        [DataMember(Order = 5)]
        public int Location { get; set; }
        [DataMember(Order = 6)]
        public int OperatorId { get; set; }
        [DataMember(Order = 7)]
        public string SessionId { get; set; }
        [DataMember(Order = 8)]
        public string Message { get; set; }
        [DataMember(Order = 9)]
        public string Name { get; set; }

        public VoucherInfo()
        {
        }

        public VoucherInfo(DataAccess.SelectVouchersData data)
        {
            //id, v_number, sitecode, barcode, scandate, location, operator_id, session_id
            Id = data.id;
            Number = data.v_number;
            SiteCode = data.sitecode;
            BarCode = data.barcode;
            ScanDate = data.scandate;
            Location = data.location;
            OperatorId = data.operator_id;
            SessionId = data.session_id;
            Name = data.v_name;
        }
    }

    [DataContract]
    public class VoucherInfo2
    {
        [DataMember(Order = 0)]
        public int Id { get; set; }
        [DataMember(Order = 1)]
        public int IsoId { get; set; }
        [DataMember(Order = 2)]
        public int RetailerId { get; set; }
        [DataMember(Order = 3)]
        public int VoucherId { get; set; }
        [DataMember(Order = 4)]
        public string SessionId { get; set; }
        [DataMember(Order = 5)]
        public string Message { get; set; }

        public VoucherInfo2(DataAccess.SelectVoucherInfoData data)
        {
            Id = data.vid;
            IsoId = data.isoId;
            RetailerId = data.branch_id;
            VoucherId = data.v_number;
            SessionId = data.session_Id;
        }
    }

    [DataContract]
    public class VoucherInfo3
    {
        [DataMember(Order = 0)]
        public int IsoId { get; set; }
        [DataMember(Order = 1)]
        public int RetailerId { get; set; }
        [DataMember(Order = 2)]
        public int VoucherId { get; set; }
        [DataMember(Order = 3)]
        public string SiteCode { get; set; }
        [DataMember(Order = 4)]
        public DateTime VoucherDate { get; set; }
        [DataMember(Order = 5)]
        public string FinalCountry { get; set; }

        public VoucherInfo3(PTFDataAccess.FindVoucher_VoucherInfo data)
        {
            IsoId = data.v_iso_id;
            RetailerId = data.v_br_id;
            VoucherId = data.v_number;
            SiteCode = data.sitecode;
            VoucherDate = data.v_date_voucher;
            FinalCountry = data.v_final_country;
        }
    }

    [DataContract]
    public class MyApplicationFault
    {
    }

    [DataContract]
    public class InvalidOperationFault
    {
        [DataMember]
        public InvalidOperationException Exception { get; private set; }

        public InvalidOperationFault(InvalidOperationException e)
        {
            Exception = e;
        }
    }

    [Serializable]
    [DataContract]
    public enum OperationHistory : int
    {
        [EnumMember]
        NA = 0,
        [EnumMember]
        Login = 1,
        [EnumMember]
        Logout = 2,

        #region PRINT / SCAN

        [EnumMember]
        Print = 3,
        [EnumMember]
        RePrint = 4,
        [EnumMember]
        Scan = 5,
        [EnumMember]
        Coversheet = 6,
        #endregion

        #region SETUP

        [EnumMember]
        DispatchVoucher = 10,

        [EnumMember]
        SetupPrint = 13,

        [EnumMember]
        SetupScan = 14,

        [EnumMember]
        ShowScannedImage = 20,

        /// <summary>
        /// Admin's
        /// </summary>
        [EnumMember]
        PrintLayoutUpdate = 101,

        [EnumMember]
        ScanLayoutUpdate = 102,

        #endregion

        #region FOLDERS

        [EnumMember]
        FolderAdded = 200,
        [EnumMember]
        FolderRenamed = 201,
        [EnumMember]
        FolderDeleted = 202,

        [EnumMember]
        FileDeleted = 207,

        [EnumMember]
        AllFilesDeleted = 208,

        [EnumMember]
        FileDownloaded = 210,

        [EnumMember]
        CoverFileDownloaded = 250,

        #endregion

        [EnumMember]
        Error = 127,
    }

    [DataContract]
    public class HistoryByCountryInfo
    {
        [DataMember(Order = 0)]
        public int Index { get; set; }
        [DataMember(Order = 1)]
        public int IsoId { get; set; }
        [DataMember(Order = 2)]
        public int UserId { get; set; }
        [DataMember(Order = 3)]
        public DateTime DateCreated { get; set; }
        [DataMember(Order = 4)]
        public OperationHistory OperType { get; set; }
        [DataMember(Order = 5)]
        public int BrIsoId { get; set; }
        [DataMember(Order = 6)]
        public int BrId { get; set; }
        [DataMember(Order = 7)]
        public int VID { get; set; }
        [DataMember(Order = 8)]
        public int V2ID { get; set; }
        [DataMember(Order = 9)]
        public int Count { get; set; }
        [DataMember(Order = 10)]
        public Guid SessionId { get; set; }
        [DataMember(Order = 11)]
        public string Details { get; set; }

        public HistoryByCountryInfo()
        {
        }

        public HistoryByCountryInfo(DataAccess.HistoryByCountryData data)
        {
            Index = data.Index;
            IsoId = data.h_iso_id;
            UserId = data.h_user_id;
            DateCreated = data.h_datetime;
            OperType = data.h_operation;
            BrIsoId = data.h_br_iso_id;
            BrId = data.h_br_id;
            VID = data.h_v_id;
            V2ID = data.h_v2_id;
            Count = data.h_count;
            SessionId = data.h_uniq_id;
            Details = data.h_details;
        }
    }

    [DataContract]
    public class RetailerPrinterInfo
    {
        [DataMember(Order = 0)]
        public int CountryId { get; set; }
        [DataMember(Order = 1)]
        public int HeadOfficeId { get; set; }
        [DataMember(Order = 2)]
        public int RetailerId { get; set; }
        [DataMember(Order = 3)]
        public int FormatId { get; set; }
        [DataMember(Order = 4)]
        public string FormatName { get; set; }
        [DataMember(Order = 5)]
        public string FormatType { get; set; }
        [DataMember(Order = 6)]
        public string PrinterPath { get; set; }

        public RetailerPrinterInfo()
        {
        }

        public RetailerPrinterInfo(DataAccess.RetailerPrinterData data)
        {
            CountryId = data.CountryId;
            HeadOfficeId = data.HeadOfficeId;
            RetailerId = data.RetailerId;
            FormatId = data.FormatId;

            FormatName = data.FormatName;
            FormatType = data.FormatType;
            PrinterPath = data.PrinterPath;
        }
    }

    [DataContract]
    public class FolderInfo
    {
        [DataMember(Order = 0)]
        public string Name { get; set; }

        [DataMember(Order = 1)]
        public int Id { get; set; }

        [DataMember(Order = 2)]
        public int? ParentId { get; set; }

        public FolderInfo()
        {
        }

        public FolderInfo(DataAccess.FolderData data)
        {
            Name = data.Name;
            Id = data.Id;
            ParentId = data.ParentId;
        }
    }

    [DataContract]
    public class fileInfo
    {
        [DataMember(Order = 0)]
        public int Id { get; set; }
        [DataMember(Order = 1)]
        public int FolderId { get; set; }
        [DataMember(Order = 2)]
        public string SessionId { get; set; }
        [DataMember(Order = 3)]
        public int CountryId { get; set; }
        [DataMember(Order = 4)]
        public int RetailerId { get; set; }
        [DataMember(Order = 5)]
        public int VoucherId { get; set; }
        [DataMember(Order = 6)]
        public string SiteCode { get; set; }
        [DataMember(Order = 6)]
        public string Name { get; set; }

        public fileInfo()
        {
        }

        public fileInfo(DataAccess.fileData data)
        {
            Id = data.Id;
            FolderId = data.FolderId;
            SessionId = data.SessionId;
            CountryId = data.CountryId;
            RetailerId = data.RetailerId;
            VoucherId = data.VoucherId;
            SiteCode = data.SiteCode;
            Name = data.Name;
        }
    }

    [DataContract]
    public class file2Info
    {
        [DataMember(Order = 0)]
        public int Id { get; set; }
        [DataMember(Order = 1)]
        public int FolderId { get; set; }
        [DataMember(Order = 2)]
        public int CountryID { get; set; }
        [DataMember(Order = 3)]
        public string SessionId { get; set; }
        [DataMember(Order = 4)]
        public int Location { get; set; }
        [DataMember(Order = 5)]
        public int Operator { get; set; }
        [DataMember(Order = 6)]
        public string Name { get; set; }

        public file2Info()
        {
        }

        public file2Info(DataAccess.file2Data data)
        {
            Id = data.Id;
            FolderId = data.FolderId;
            SessionId = data.SessionId;
            CountryID = data.CountryId;
            Location = data.Location;
            Operator = data.Operator;
            Name = data.Name;
        }
    }

    [DataContract]
    public class UpdateFileInfo
    {
        [DataMember(Order = 0)]
        public string Name { get; set; }
        [DataMember(Order = 1)]
        public long Length { get; set; }

        public UpdateFileInfo()
        {
        }

        public UpdateFileInfo(FileInfo info)
        {
            Name = info.Name;
            Length = info.Length;
        }
    }

    [DataContract]
    public class TransferFileInfo
    {
        [DataMember(Order = 0)]
        public string InvNo { get; set; }
        [DataMember(Order = 1)]
        public int BranchId { get; set; }
        [DataMember(Order = 2)]
        public string SiteLocationNo { get; set; }
        [DataMember(Order = 3)]
        public int VoucherNumber { get; set; }

        public TransferFileInfo()
        {
        }

        public TransferFileInfo(DataAccess.TransferFileData data)
        {
            InvNo = data.InvNo;
            BranchId = data.BranchId;
            SiteLocationNo = data.SiteLocationNo;
            VoucherNumber = data.VoucherNumber;
        }
    }

    #endregion
}
