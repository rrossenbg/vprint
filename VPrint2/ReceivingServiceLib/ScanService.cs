/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security;
using System.ServiceModel;
using System.ServiceModel.Channels;
using ReceivingServiceLib.Common.Data;
using ReceivingServiceLib.Data;
using ReceivingServiceLib.Services;
using VPrint.Common.Pdf;
using VPrinting;
using VPrinting.Pdf;

namespace ReceivingServiceLib
{
    //DO NOT ADD TRACE CODE!!!! 
    //[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, AddressFilterMode = AddressFilterMode.Any)]
    [ErrorHandlingBehavior(ExceptionToFaultConverter = typeof(MyServiceFaultProvider))]
    public partial class ScanService : IScanService
    {
        public static event EventHandler<ValueEventArgs<Tuple<string, string, DateTime>>> NewCall;
        public static event EventHandler<ValueEventArgs<Tuple<VoucherDataAccess.SelectVoucherInfoData, DirectoryInfo>>> ExtractVoucher;

        #region SCAN

        private class MyServiceFaultProvider : IExceptionToFaultConverter
        {
            public object ConvertExceptionToFaultDetail(Exception error)
            {
                if (error is InvalidOperationException)
                    return new InvalidOperationFault(error as InvalidOperationException);
                return null;
            }
        }

        public static readonly SynchronizedCollection<string> FileLocks = new SynchronizedCollection<string>();
        private static readonly ConcurrentDictionary<int, byte[]> ByteBuffers = new ConcurrentDictionary<int, byte[]>();

        static ScanService()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="countryId"></param>
        /// <param name="retailerId"></param>
        /// <param name="voucherId"></param>
        public void Delete(string fileName, int countryId, int retailerId, int voucherId, string s1, string s2)
        {
            try
            {
                SecurityCheckThrow(s1, s2);
                RecordCallHistory("Delete");

                var uploadRootFolder = new DirectoryInfo(Global.Strings.UPLOADROOT);
                var f = uploadRootFolder.Combine(countryId.ToString()).Combine(retailerId.ToString()).Combine(voucherId.ToString()).CombineFileName(fileName);
                if (f.Exists)
                    f.Delete();
            }
            catch (Exception ex)
            {
                throw new FaultException<MyApplicationFault>(new MyApplicationFault(), ex.Message);
            }
        }

        public List<VoucherInfo> ReadData(int countryId, int retailerId, string s1, string s2)
        {
            try
            {
                SecurityCheckThrow(s1, s2);
                RecordCallHistory("ReadData");

                var result = VoucherDataAccess.Instance.SelectVouchers(countryId, retailerId).ConvertAll<VoucherInfo>((i) => new VoucherInfo(i));
                return result;
            }
            catch (Exception ex)
            {
                throw new FaultException<MyApplicationFault>(new MyApplicationFault(), ex.Message);
            }
        }

        public byte[] ReadData2(int id, bool isVoucher, int start, int length, string s1, string s2)
        {
            try
            {
                SecurityCheckThrow(s1, s2);
                RecordCallHistory("ReadData2");

                if (!ByteBuffers.ContainsKey(id))
                    ByteBuffers[id] = VoucherDataAccess.Instance.SelectImageById(id, isVoucher);

                byte[] buffer = ByteBuffers[id];
                byte[] result = new byte[length];

                Array.Copy(buffer, result, length);

                return result;
            }
            catch (Exception ex)
            {
                throw new FaultException<MyApplicationFault>(new MyApplicationFault(), ex.Message);
            }
        }

        /// <summary>
        /// VScan calls WCF service to save data on the server. It's buffered operation.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="data"></param>
        public void SaveData(string serverDirName, string fileName, byte[] data, string s1, string s2)
        {
            try
            {
                SecurityCheckThrow(s1, s2);
                RecordCallHistory("SaveData");

                if (!FileLocks.Contains(serverDirName, StringComparer.InvariantCultureIgnoreCase))
                    FileLocks.Add(serverDirName);

                var uploadRootFolder = new DirectoryInfo(Global.Strings.UPLOADROOT);
                var serverDir = uploadRootFolder.Combine(serverDirName);
                serverDir.EnsureDirectory();
                var finfo = serverDir.CombineFileName(fileName);
                using (var file = finfo.Open(FileMode.OpenOrCreate))
                {
                    file.Seek(file.Length, SeekOrigin.Begin);
                    file.Write(data, 0, data.Length);
                }
            }
            catch (Exception ex)
            {
                throw new FaultException<MyApplicationFault>(new MyApplicationFault(), ex.Message);
            }
        }

        public void SaveDataAsync(string serverDirName, string fileName, byte[] data, long position, string s1, string s2)
        {
            try
            {
                SecurityCheckThrow(s1, s2);
                RecordCallHistory("SaveDataAsync");

                if (!FileLocks.Contains(serverDirName, StringComparer.InvariantCultureIgnoreCase))
                    FileLocks.Add(serverDirName);

                var uploadRootFolder = new DirectoryInfo(Global.Strings.UPLOADROOT);
                var serverDir = uploadRootFolder.Combine(serverDirName);
                serverDir.EnsureDirectory();
                var finfo = serverDir.CombineFileName(fileName);

                using (var file = finfo.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    file.Seek(position, SeekOrigin.Begin);
                    file.Write(data, 0, data.Length);
                }
            }
            catch (Exception ex)
            {
                throw new FaultException<MyApplicationFault>(new MyApplicationFault(), ex.Message);
            }
        }

        /// <summary>
        /// VScan calls WCF service to complete data transfer on the server side.
        /// </summary>
        /// <param name="serverDirName"></param>
        /// <param name="countryId"></param>
        /// <param name="retailerId"></param>
        /// <param name="voucherId"></param>
        /// <param name="siteCode"></param>
        /// <param name="locationId"></param>
        /// <param name="userId"></param>
        public void CommitVoucherChanges(string serverDirName, int jobId, int countryId, int retailerId, int voucherId, int? folderId,
            string siteCode, string barCode, int locationId, int userId, string s1, string s2)
        {
            try
            {
                SecurityCheckThrow(s1, s2);
                RecordCallHistory("CommitVoucherChanges");

                var uploadRootFolder = new DirectoryInfo(Global.Strings.UPLOADROOT);
                var directory = uploadRootFolder.Combine(serverDirName);
                if (directory.Exists)
                {
                    var xmlName = directory.CombineFileName("data.xml");
                    ZipFileAccess.Instance.SaveVoucherXml(xmlName, jobId, countryId,
                        retailerId, voucherId, folderId, siteCode, barCode, userId, locationId, serverDirName, typeId: 2);
                }
            }
            catch (Exception ex)
            {
                throw new FaultException<MyApplicationFault>(new MyApplicationFault(), ex.Message);
            }
            finally
            {
                FileLocks.Remove(serverDirName);
            }
        }

        /// <summary>
        /// VScan calls WCF service to complete data transfer on the server side.
        /// </summary>
        /// <param name="serverDirName"></param>
        /// <param name="countryId"></param>
        /// <param name="retailerId"></param>
        /// <param name="voucherId"></param>
        /// <param name="siteCode"></param>
        /// <param name="locationId"></param>
        /// <param name="userId"></param>
        public void CommitVoucherChangesModify(string serverDirName, int jobId, int countryId, int retailerId, int voucherId, int? folderId,
            string siteCode, string barCode, int locationId, int userId, ChangeContentType action, string s1, string s2)
        {
            try
            {
                SecurityCheckThrow(s1, s2);
                RecordCallHistory("CommitVoucherChangesModify");

                var uploadRootFolder = new DirectoryInfo(Global.Strings.UPLOADROOT);
                var directory = uploadRootFolder.Combine(serverDirName);
                if (directory.Exists)
                {
                    var xmlName = directory.CombineFileName(GetXmlFileNamePerAction(action));
                    ZipFileAccess.Instance.SaveVoucherXml(xmlName, jobId, countryId,
                        retailerId, voucherId, folderId, siteCode, barCode, userId, locationId, serverDirName, typeId: 2);
                }
            }
            catch (Exception ex)
            {
                throw new FaultException<MyApplicationFault>(new MyApplicationFault(), ex.Message);
            }
            finally
            {
                FileLocks.Remove(serverDirName);
            }
        }

        public void CommitVoucherChangesModify_V2(string serverDirName, int jobId, int countryId, int retailerId, int voucherId, int? folderId,
            string siteCode, string barCode, int locationId, int userId, int typeId, ChangeContentType action, string s1, string s2)
        {
            try
            {
                SecurityCheckThrow(s1, s2);
                RecordCallHistory("CommitVoucherChangesModify");

                var uploadRootFolder = new DirectoryInfo(Global.Strings.UPLOADROOT);
                var directory = uploadRootFolder.Combine(serverDirName);
                if (directory.Exists)
                {
                    var xmlName = directory.CombineFileName(GetXmlFileNamePerAction(action));
                    ZipFileAccess.Instance.SaveVoucherXml(xmlName, jobId, countryId,
                        retailerId, voucherId, folderId, siteCode, barCode, userId, locationId, serverDirName, typeId);
                }
            }
            catch (Exception ex)
            {
                throw new FaultException<MyApplicationFault>(new MyApplicationFault(), ex.Message);
            }
            finally
            {
                FileLocks.Remove(serverDirName);
            }
        }

        private string GetXmlFileNamePerAction(ChangeContentType action)
        {
            switch (action)
            {
                case ChangeContentType.INIT:
                    return "data.xml";

                case ChangeContentType.ADD:
                    return "adddata.xml";

                case ChangeContentType.INSERT:
                    return "insertdata.xml";

                case ChangeContentType.UPDATE:
                    return "updatedata.xml";

                case ChangeContentType.REMOVE:
                    return "removedata.xml";

                case ChangeContentType.DELETE:
                    return "deletedata.xml";
                default:
                    return string.Empty;
            }
        }

        public void CommitFileChanges(string serverDirName, int countryId, int? folderId,
            int locationId, int userId, string s1, string s2)
        {
            try
            {
                SecurityCheckThrow(s1, s2);
                RecordCallHistory("CommitFileChanges");

                var uploadRootFolder = new DirectoryInfo(Global.Strings.UPLOADROOT);
                var directory = uploadRootFolder.Combine(serverDirName);
                if (directory.Exists)
                {
                    var xmlName = directory.CombineFileName("cover.xml");
                    ZipFileAccess.Instance.SaveCoversheetXml(xmlName, countryId,
                        folderId, userId, locationId, serverDirName);
                }
            }
            catch (Exception ex)
            {
                throw new FaultException<MyApplicationFault>(new MyApplicationFault(), ex.Message);
            }
            finally
            {
                FileLocks.Remove(serverDirName);
            }
        }

        public void ValidateVoucher(int countryId, bool ss, int retailerId, int voucherId, bool voucherMustExist, string s1, string s2)
        {
            try
            {
                SecurityCheckThrow(s1, s2);
                RecordCallHistory("ValidateVoucher");

                var da = VoucherDataAccess.Instance;

                if (!Global.Data.ContainsKey(Strings.SingleSaleCountries))
                    Global.Data[Strings.SingleSaleCountries] = da.GetConfigValue(Strings.SingleSaleCountries);

                string sscountries = Convert.ToString(Global.Data[Strings.SingleSaleCountries]);

                if (ss)
                {
                    if (!sscountries.Contains(countryId.ToString()))
                        throw new InvalidDataException(countryId + " is not Single Sale country");
                }
                else//ds
                {
                    if (sscountries.Contains(countryId.ToString()))
                        throw new InvalidDataException(countryId + " is not Double Sale country");
                }

                da.ValidateVoucherThrow(countryId, retailerId, voucherId, voucherMustExist);
            }
            catch (Exception ex)
            {
                throw new FaultException<MyApplicationFault>(new MyApplicationFault(), ex.Message);
            }
        }

        public string FindVoucher(int countryId, int voucherId, int voucherIdCD, string s1, string s2)
        {
            try
            {
                SecurityCheckThrow(s1, s2);
                RecordCallHistory("FindVoucher");

                var da = VoucherDataAccess.Instance;
                return da.FindVoucher(countryId, voucherId, voucherIdCD);
            }
            catch (Exception ex)
            {
                throw new FaultException<MyApplicationFault>(new MyApplicationFault(), ex.Message);
            }
        }

        public string FindVoucherPart(int countryId, int voucherId, int voucherIdCD, int part, string s1, string s2)
        {
            try
            {
                SecurityCheckThrow(s1, s2);
                RecordCallHistory("FindVoucher");

                var da = VoucherDataAccess.Instance;
                return da.FindVoucher(countryId, voucherId, voucherIdCD, part);
            }
            catch (Exception ex)
            {
                throw new FaultException<MyApplicationFault>(new MyApplicationFault(), ex.Message);
            }
        }

        public int[] FindVoucherImage(int countryId, int voucherId, int voucherIdCD, string s1, string s2)
        {
            try
            {
                SecurityCheckThrow(s1, s2);
                RecordCallHistory("FindVoucherImage");

                var da = VoucherDataAccess.Instance;
                return da.FindVoucherImage(countryId, voucherId, voucherIdCD).ToArray();
            }
            catch (Exception ex)
            {
                throw new FaultException<MyApplicationFault>(new MyApplicationFault(), ex.Message);
            }
        }

        #endregion

        #region CCC COVER

        public string ReadCoverInfo(int id, string s1, string s2)
        {
            try
            {
                SecurityCheckThrow(s1, s2);
                RecordCallHistory("ReadCoverInfo");

                var da = VoucherDataAccess.Instance;
                var result = da.SelectCoverInfo(id);
                return result;
            }
            catch (Exception ex)
            {
                throw new FaultException<MyApplicationFault>(new MyApplicationFault(), ex.Message);
            }
        }

        #endregion

        #region REPORTS

        /// <summary>
        /// MVC site calls WCF service to read info about voucher
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="copyToFolder"></param>
        /// <returns></returns>
        public VoucherInfo2 ReadVoucherInfo(int Id, string copyToFolder, string s1, string s2)
        {
            try
            {
                if (!SecurityCheck(s1, s2))
                    throw new Exception("Wrong caller id");

                RecordCallHistory("ReadVoucherInfo");

                var result = VoucherDataAccess.Instance.SelectVoucherInfo(Id);

                var info = new VoucherInfo2(result);
                if (info.SessionId == null)
                {
                    info.Message = "No session Id found for this voucher.";
                    return info;
                }

                var webroot = new DirectoryInfo(copyToFolder);
                webroot.EnsureDirectory();

                var sessionFolder = webroot.Combine(info.SessionId);
                if (!sessionFolder.Exists || sessionFolder.IsEmpty())
                {
                    sessionFolder.EnsureDirectory();

                    var fac = new ZipFileAccess();
                    var fromDir = fac.CreateDirectoryHerarchy(Global.Strings.VOCUHERSFOLDER, info.IsoId, info.RetailerId, info.VoucherId);
                    var count = fromDir.CopyFiles(sessionFolder, true);
                    if (count == 0)
                    {
                        if (ExtractVoucher != null)
                            ExtractVoucher(this, new ValueEventArgs<Tuple<VoucherDataAccess.SelectVoucherInfoData, DirectoryInfo>>(new Tuple<VoucherDataAccess.SelectVoucherInfoData, DirectoryInfo>(result, fromDir)));
                        //Export file from DB to hierarchy folder
                    }
                }

                return info;
            }
            catch (Exception ex)
            {
                throw new FaultException<MyApplicationFault>(new MyApplicationFault(), ex.Message);
            }
        }

        public void SaveHistory(int operatorCountryId, int operatorUserId, OperationHistory operationType, Guid operationId, int brIsoId, int brId, int vId, int v2Id, int count, string details, string s1, string s2)
        {
            try
            {
                if (!SecurityCheck(s1, s2))
                    throw new Exception("Wrong caller id");

                RecordCallHistory("SaveHistory");

                VoucherDataAccess.Instance.SaveHistory(operatorCountryId, operatorUserId, (int)operationType, operationId, brIsoId, brId, vId, v2Id, count, details);
            }
            catch (Exception ex)
            {
                throw new FaultException<MyApplicationFault>(new MyApplicationFault(), ex.Message);
            }
        }

        public List<HistoryByCountryInfo> ReadHistory(int operatorCountryId, int? operatorUserId, OperationHistory operationType, DateTime from, DateTime to, string s1, string s2)
        {
            try
            {
                SecurityCheckThrow(s1, s2);
                RecordCallHistory("ReadHistory");

                var list = VoucherDataAccess.Instance.SelectHistoryByCountryAndOperator(operatorCountryId, operatorUserId, (int)operationType, from, to);

                var resultList = new List<HistoryByCountryInfo>();
                foreach (var data in list)
                    resultList.Add(new HistoryByCountryInfo(data));
                return resultList;
            }
            catch (Exception ex)
            {
                throw new FaultException<MyApplicationFault>(new MyApplicationFault(), ex.Message);
            }
        }

        public List<RetailerPrinterInfo> ReadRetailerPrinterInfo(int countryId, string s1, string s2)
        {
            try
            {
                if (!SecurityCheck(s1, s2))
                    throw new Exception("Wrong caller id");

                RecordCallHistory("ReadRetailerPrinterInfo");

                var list = VoucherDataAccess.Instance.SelectRetailerPrinterData(countryId);

                var resultList = new List<RetailerPrinterInfo>();
                foreach (var data in list)
                    resultList.Add(new RetailerPrinterInfo(data));
                return resultList;
            }
            catch (Exception ex)
            {
                throw new FaultException<MyApplicationFault>(new MyApplicationFault(), ex.Message);
            }
        }

        public List<fileInfo> SelectFilesBySql(string whereClause, string s1, string s2)
        {
            try
            {
                SecurityCheckThrow(s1, s2);
                RecordCallHistory("SelectFilesBySql");

                if (string.IsNullOrWhiteSpace(whereClause))
                    throw new ArgumentException("whereClause");

                if (whereClause.Contains((c) => c == ';'))
                    throw new SecurityException("Wrong data");

                var dblist = VoucherDataAccess.Instance.SelectVouchersBySql(whereClause);
                var list = dblist.ConvertAll(f => new fileInfo(f));
                return list;
            }
            catch (Exception ex)
            {
                throw new FaultException<MyApplicationFault>(new MyApplicationFault(), ex.Message);
            }
        }

        public void UpdateVouchersOrFilesBySql(string setSql, string whereClause, bool isVoucher, string s1, string s2)
        {
            try
            {
                SecurityCheckThrow(s1, s2);
                RecordCallHistory("UpdateVouchersOrFilesBySql");

                if (string.IsNullOrWhiteSpace(whereClause))
                    throw new ArgumentException("whereClause");

                if (whereClause.Contains((c) => c == ';'))
                    throw new SecurityException("Wrong data");

                if (string.IsNullOrWhiteSpace(setSql))
                    throw new ArgumentException("setClause");

                if (isVoucher)
                    VoucherDataAccess.Instance.UpdateVouchersBySql(setSql, whereClause);
                else
                    VoucherDataAccess.Instance.UpdateFilesBySql(setSql, whereClause);
            }
            catch (Exception ex)
            {
                throw new FaultException<MyApplicationFault>(new MyApplicationFault(), ex.Message);
            }
        }

        #endregion

        #region FOLDERS

        public void AddFolder(int? toParentId, string name, int countryId, int userId, string s1, string s2)
        {
            try
            {
                SecurityCheckThrow(s1, s2);
                RecordCallHistory("AddFolder");

                VoucherDataAccess.Instance.AddFolder(toParentId, name, countryId, userId);
            }
            catch (Exception ex)
            {
                throw new FaultException<MyApplicationFault>(new MyApplicationFault(), ex.Message);
            }
        }

        public void DeleteFolder(int folderId, string s1, string s2)
        {
            try
            {
                SecurityCheckThrow(s1, s2);
                RecordCallHistory("DeleteFolder");

                VoucherDataAccess.Instance.DeleteFolder(folderId);
            }
            catch (Exception ex)
            {
                throw new FaultException<MyApplicationFault>(new MyApplicationFault(), ex.Message);
            }
        }

        public void DeleteFile(int id, bool isVoucher, string s1, string s2)
        {
            try
            {
                SecurityCheckThrow(s1, s2);
                RecordCallHistory("DeleteFile");

                VoucherDataAccess.Instance.DeleteVoucherOrFile(id, isVoucher);
            }
            catch (Exception ex)
            {
                throw new FaultException<MyApplicationFault>(new MyApplicationFault(), ex.Message);
            }
        }

        public void DeleteAllFilesInFolder(int folderId, string s1, string s2)
        {
            try
            {
                SecurityCheckThrow(s1, s2);
                RecordCallHistory("DeleteAllFilesInFolder");

                var db = VoucherDataAccess.Instance;
                db.DeleteAllFilesInFolder(folderId, false);
                db.DeleteAllFilesInFolder(folderId, true);
            }
            catch (Exception ex)
            {
                throw new FaultException<MyApplicationFault>(new MyApplicationFault(), ex.Message);
            }
        }

        public void RenameFolder(int folderId, string name, string s1, string s2)
        {
            try
            {
                SecurityCheckThrow(s1, s2);
                RecordCallHistory("RenameFolder");

                VoucherDataAccess.Instance.RenameFolder(folderId, name);
            }
            catch (Exception ex)
            {
                throw new FaultException<MyApplicationFault>(new MyApplicationFault(), ex.Message);
            }
        }

        public void UpdateFolder(int folderId, string name, int? parentId, int countryId, int userId, string s1, string s2)
        {
            try
            {
                SecurityCheckThrow(s1, s2);
                RecordCallHistory("UpdateFolder");

                VoucherDataAccess.Instance.UpdateFolder(folderId, name, parentId);
            }
            catch (Exception ex)
            {
                throw new FaultException<MyApplicationFault>(new MyApplicationFault(), ex.Message);
            }
        }

        public List<FolderInfo> SelectFoldersByParent(int? parentId, int createdByIsoId, string s1, string s2)
        {
            try
            {
                SecurityCheckThrow(s1, s2);
                RecordCallHistory("SelectFoldersByParent");

                var dblist = VoucherDataAccess.Instance.SelectAllByParent(parentId, createdByIsoId);
                var list = dblist.ConvertAll(f => new FolderInfo(f));
                return list;
            }
            catch (Exception ex)
            {
                throw new FaultException<MyApplicationFault>(new MyApplicationFault(), ex.Message);
            }
        }

        public List<fileInfo> SelectFilesByFolder(int folderId, string s1, string s2)
        {
            try
            {
                SecurityCheckThrow(s1, s2);
                RecordCallHistory("SelectFilesByFolder");

                var dblist = VoucherDataAccess.Instance.SelectVouchersByFolder(folderId);
                var list = dblist.ConvertAll(f => new fileInfo(f));
                return list;
            }
            catch (Exception ex)
            {
                throw new FaultException<MyApplicationFault>(new MyApplicationFault(), ex.Message);
            }
        }

        public List<fileInfo> SelectFilesByFolder2(int folderId, int skip, int take, string s1, string s2)
        {
            try
            {
                SecurityCheckThrow(s1, s2);
                RecordCallHistory("SelectFilesByFolder2");

                var dblist = VoucherDataAccess.Instance.SelectVouchersByFolder(folderId, skip, skip + take);
                var list = dblist.ConvertAll(f => new fileInfo(f));
                return list;
            }
            catch (Exception ex)
            {
                throw new FaultException<MyApplicationFault>(new MyApplicationFault(), ex.Message);
            }
        }

        public List<file2Info> SelectCoversByFolder(int folderId, string s1, string s2)
        {
            try
            {
                SecurityCheckThrow(s1, s2);
                RecordCallHistory("SelectCoversByFolder");

                var dblist = VoucherDataAccess.Instance.SelectFilesByFolder(folderId);
                var list = dblist.ConvertAll(f => new file2Info(f));
                return list;
            }
            catch (Exception ex)
            {
                throw new FaultException<MyApplicationFault>(new MyApplicationFault(), ex.Message);
            }
        }

        /// <summary>
        /// Read voucher/file from database
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="isVoucher"></param>
        /// <param name="signed"></param>
        /// <param name="startFrom"></param>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        public byte[] SelectFileById(int fileId, bool isVoucher, bool signed, int startFrom, string s1, string s2)
        {
            //if (!Debugger.IsAttached)
            //    Debugger.Launch();

            const int READSIZE = 16384;

            using (var buffer = new CachedMemoryBuffer<int>(fileId))
            {
                try
                {
                    SecurityCheckThrow(s1, s2);
                    RecordCallHistory("SelectFileById");

                    if (buffer.IsFirstRun)
                    {
                        if (signed)
                        {
                            if (!isVoucher)
                                throw new NotImplementedException("Signing for files not implemented yet.");

                            #region signed voucher
                            //signed voucher
                            var db = VoucherDataAccess.Instance;
                            var vinfo = db.SelectVoucherInfo(fileId);
                            var countryName = ISOs.ResourceManager.GetString(string.Concat('_', vinfo.isoId));
                            var buf = db.SelectImageById(fileId, true);

                            DirectoryInfo exportDirectory = null;
                            FileInfo exportZipFile = null;
                            DirectoryInfo operationDirectory = null;
                            FileInfo operationZipFile = null;

                            try
                            {
                                var uploadRootFolder = new DirectoryInfo(Global.Strings.UPLOADROOT);
                                uploadRootFolder.EnsureDirectory();

                                exportDirectory = uploadRootFolder.Combine(vinfo.session_Id);
                                exportDirectory.EnsureDirectory();

                                operationDirectory = uploadRootFolder.Combine(vinfo.session_Id + "_result");
                                operationDirectory.EnsureDirectory();

                                exportZipFile = uploadRootFolder.CombineFileName(vinfo.session_Id + ".zip");
                                operationZipFile = uploadRootFolder.CombineFileName(vinfo.session_Id + "result.zip");

                                File.WriteAllBytes(exportZipFile.FullName, buf);

                                var files = ZipFileAccess.Instance.ExtractFileZip(exportZipFile.FullName, exportDirectory.FullName);
                                var imageFileToSing = files.Max((f, f1) => f.Length > f1.Length);

                                PdfCreationInfo crInfo = new PdfCreationInfo()
                                {
                                    Title = string.Concat("Voucher ", vinfo.v_number),
                                    Subject = string.Concat("Retailer ", vinfo.branch_id),
                                    Author = string.Concat("PTF ", countryName),
                                    Creator = string.Concat("PTF ", countryName),
                                };

                                PdfSignInfo signInfo = new PdfSignInfo()
                                {
                                    pfxFilePath = Global.Strings.pfxFileFullPath,
                                    pfxKeyPass = "",
                                    DocPass = null,
                                    SignImagePath = Global.Strings.PTFLogoFileFullPath,
                                    ReasonForSigning = string.Concat("Voucher ", vinfo.v_number),
                                    Location = "Madrid"
                                };

                                using (var bmp = (Bitmap)Image.FromFile(imageFileToSing.FullName))
                                {
                                    var pdfFileName = pdfFileAccess.Instance.CreateSignPdf(bmp, "barcode", vinfo.branch_id, vinfo.v_number, crInfo, signInfo);

                                    var imageFileName = operationDirectory.CombineFileName(vinfo.session_Id + ".pdf").FullName;
                                    File.Move(pdfFileName, imageFileName);

                                    ZipFileAccess.Instance.CreateZip(operationZipFile.FullName, operationDirectory.FullName, "File created at: " + DateTime.Now);
                                    buffer.Buffer = operationZipFile.ReadAllBytes();
                                }
                            }
                            finally
                            {
                                exportDirectory.DeleteSafe(true);
                                exportZipFile.DeleteSafe();
                                operationZipFile.DeleteSafe();
                            }
                            #endregion
                        }
                        else
                        {
                            //voucher || file
                            bool isProtected = false;
                            byte[] buf = VoucherDataAccess.Instance.SelectVoucherById(fileId, isVoucher, out isProtected);

                            if (isProtected)
                            {
                                var downloadRoot = new DirectoryInfo(Global.Strings.DOWNLOADROOT);
                                downloadRoot.CreateIfNotExist();
                                var binInfo = downloadRoot.CombineFileName(string.Concat(fileId, ".bin"));
                                binInfo.WriteAllBytes(buf);
                                var zipInfo = downloadRoot.CombineFileName(string.Concat(fileId, ".zip"));
                                binInfo.DecriptFile(zipInfo);
                                buffer.Buffer = zipInfo.ReadAllBytes();
                                zipInfo.DeleteSafe();
                                binInfo.DeleteSafe();
                            }
                            else
                            {
                                buffer.Buffer = buf;
                            }
                        }
                    }

                    byte[] arr = buffer.Get(startFrom, READSIZE);
                    return arr;
                }
                catch (Exception ex)
                {
                    buffer.Remove();
                    throw new FaultException<MyApplicationFault>(new MyApplicationFault(), ex.Message);
                }
            }
        }

        #endregion

        #region VERSION UPDATE

        public List<UpdateFileInfo> GetVersionInfo(string currentVersion, string s1, string s2)
        {
            const string VERSIONFILENAME = "version.txt";
            try
            {
                SecurityCheckThrow(s1, s2);
                RecordCallHistory("GetVersionInfo");

                var versionFolder = new DirectoryInfo(Global.Strings.VERSIONFOLDER);
                versionFolder.EnsureDirectory();

                var versionFile = versionFolder.CombineFileName(VERSIONFILENAME);
                var text = versionFile.ReadAllText();

                var list = new List<UpdateFileInfo>();

                var serverVersion = new Version(text);
                var clientVersion = new Version(currentVersion);
                if (clientVersion == serverVersion)
                    return list;

                foreach (var file in versionFolder.GetFiles())
                {
                    if (file.Name.EqualNoCase(VERSIONFILENAME))
                        continue;

                    var info = new UpdateFileInfo(file);
                    list.Add(info);
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new FaultException<MyApplicationFault>(new MyApplicationFault(), ex.Message);
            }
        }

        public byte[] ReadVersionFile(string fileName, long from, string s1, string s2)
        {
            const int READSIZE = 16384;
            try
            {
                SecurityCheckThrow(s1, s2);
                RecordCallHistory("ReadVersionFile");

                var versionFolder = new DirectoryInfo(Global.Strings.VERSIONFOLDER);
                versionFolder.EnsureDirectory();

                var file = versionFolder.CombineFileName(fileName);
                if (file.Length - from <= 0)
                    return null;

                byte[] buffer = new byte[file.Length - from > READSIZE ? READSIZE : file.Length - from];
                file.Read((int)from, buffer.Length, buffer);
                return buffer;
            }
            catch (Exception ex)
            {
                throw new FaultException<MyApplicationFault>(new MyApplicationFault(), ex.Message);
            }
        }

        #endregion

        #region TRANSFER FILE

        public List<TransferFileInfo> GetTransferFile(int countryId, int beginNumber, int endNumber, string siteCode, string s1, string s2)
        {
            try
            {
                SecurityCheckThrow(s1, s2);
                RecordCallHistory("GetTransferFile");

                var list = VoucherDataAccess.Instance.GetTransferFileReport(countryId, beginNumber, endNumber, siteCode).ConvertAll((d) => new TransferFileInfo(d));
                return list;
            }
            catch (Exception ex)
            {
                throw new FaultException<MyApplicationFault>(new MyApplicationFault(), ex.Message);
            }
        }

        #endregion

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

        #region GENERAL

        public ArrayList RetrieveTableData(string fieldList, string tableName, string where, string s1, string s2)
        {
            try
            {
                SecurityCheckThrow(s1, s2);
                RecordCallHistory("RetrieveTableData");

                if (fieldList.IsNullOrWhiteSpace())
                    throw new ArgumentException("fieldList");
                if (tableName.IsNullOrWhiteSpace())
                    throw new ArgumentException("tableName");
                if (where.IsNullOrWhiteSpace())
                    throw new ArgumentException("where");

                return new VoucherDataAccess().RetrieveTableData(fieldList, tableName, where);
            }
            catch (Exception ex)
            {
                throw new FaultException<MyApplicationFault>(new MyApplicationFault(), ex.Message);
            }
        }

        public int UpdateTableData(ArrayList table, string s1, string s2)
        {
            try
            {
                SecurityCheckThrow(s1, s2);
                RecordCallHistory("UpdateTableData");

                if (table == null || table.Count == 0)
                    throw new ArgumentException("table");

                var htable = table.ToHashtable<string, object>();
                return new VoucherDataAccess().UpdateTableData(htable);
            }
            catch (Exception ex)
            {
                throw new FaultException<MyApplicationFault>(new MyApplicationFault(), ex.Message);
            }
        }

        #endregion

        #region TRS

        public VoucherInfo3 FindVoucherTRSByVoucherNumber(int countryId, int voucherId, string s1, string s2)
        {
            try
            {
                SecurityCheckThrow(s1, s2);
                RecordCallHistory("FindVoucherTRSByVoucherNumber");

                var da = new PTFDataAccess();
                var v = da.FindVoucher(countryId, voucherId);
                return new VoucherInfo3(v);
            }
            catch (Exception ex)
            {
                throw new FaultException<MyApplicationFault>(new MyApplicationFault(), ex.Message);
            }
        }

        public VoucherInfo3 FindVoucherTRSBySiteCode(string siteCode, int location, string s1, string s2)
        {
            try
            {
                SecurityCheckThrow(s1, s2);
                RecordCallHistory("FindVoucherTRSBySiteCode");

                var da = new PTFDataAccess();
                var v = da.FindVoucher(siteCode, location);
                return new VoucherInfo3(v);
            }
            catch (Exception ex)
            {
                throw new FaultException<MyApplicationFault>(new MyApplicationFault(), ex.Message);
            }
        }

        #endregion

        #region PR

        public VoucherInfo3 FindVoucherPRBySiteCode(string siteCode, int location, string s1, string s2)
        {
            try
            {
                SecurityCheckThrow(s1, s2);
                RecordCallHistory("FindVoucherPRBySiteCode");

                var da = new RefundServiceDataAccess();
                var v = da.CallRefundService(string.Concat(siteCode, location));
                return new VoucherInfo3(v);
            }
            catch (Exception ex)
            {
                throw new FaultException<MyApplicationFault>(new MyApplicationFault(), ex.Message);
            }
        }

        #endregion
    }
}