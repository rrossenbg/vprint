/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.ServiceModel;
using System.Threading.Tasks;
using ReceivingServiceLib.Data;
using System.Diagnostics;
using System.Drawing;

namespace ReceivingServiceLib
{
    //DO NOT ADD TRACE CODE!!!! 
    //[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, AddressFilterMode = AddressFilterMode.Any)]
    [ErrorHandlingBehavior(ExceptionToFaultConverter = typeof(MyServiceFaultProvider))]
    public class ScanService : IScanService
    {
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
                var result = DataAccess.Instance.SelectVouchers(countryId, retailerId).ConvertAll<VoucherInfo>((i) => new VoucherInfo(i));
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

                if (!ByteBuffers.ContainsKey(id))
                    ByteBuffers[id] = DataAccess.Instance.SelectImageById(id, isVoucher);

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

                var uploadRootFolder = new DirectoryInfo(Global.Strings.UPLOADROOT);
                var directory = uploadRootFolder.Combine(serverDirName);
                if (directory.Exists)
                {
                    var xmlName = directory.CombineFileName("data.xml");
                    zipFileAccess.Instance.SaveVoucherXml(xmlName, jobId, countryId,
                        retailerId, voucherId, folderId, siteCode, barCode, userId, locationId, serverDirName);
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

        public void CommitFileChanges(string serverDirName, int countryId, int? folderId,
            int locationId, int userId, string s1, string s2)
        {
            try
            {
                SecurityCheckThrow(s1, s2);

                var uploadRootFolder = new DirectoryInfo(Global.Strings.UPLOADROOT);
                var directory = uploadRootFolder.Combine(serverDirName);
                if (directory.Exists)
                {
                    var xmlName = directory.CombineFileName("cover.xml");
                    zipFileAccess.Instance.SaveCoversheetXml(xmlName, countryId,
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

                var da = DataAccess.Instance;

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

                var da = DataAccess.Instance;
                return da.FindVoucher(countryId, voucherId, voucherIdCD);
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

                var result = DataAccess.Instance.SelectVoucherInfo(Id);

                var info = new VoucherInfo2(result);
                if (info.SessionId == null)
                {
                    info.Message = "No session Id found for this voucher.";
                    return info;
                }

                var fac = new zipFileAccess();
                var fromDir = fac.CreateDirectoryHerarchy(Global.Strings.VOCUHERSFOLDER, info);

                var webroot = new DirectoryInfo(copyToFolder);
                webroot.EnsureDirectory();

                var sessionFolder = webroot.Combine(info.SessionId);
                sessionFolder.DeleteSafe(true);
                sessionFolder.EnsureDirectory();

                var count = fromDir.CopyFiles(sessionFolder, true);
                if (count == 0)
                {
                    //Export file from DB
                    var t = Task.Factory.StartNew((o) =>
                    {
                        try
                        {
                            var result2 = o.cast<DataAccess.SelectVoucherInfoData>();

                            byte[] data = DataAccess.Instance.SelectImageById(result2.vid, true);

                            var exportRoot = new DirectoryInfo(Global.Strings.VOCUHERSEXPORTFOLDER);
                            exportRoot.EnsureDirectory();

                            var zipfile = exportRoot.CombineFileName((result2.session_Id ?? Guid.NewGuid().ToString()) + ".zip");
                            zipfile.WriteAllBytes(data);
                        }
                        catch (Exception ex)
                        {
                            throw new FaultException<MyApplicationFault>(new MyApplicationFault(), ex.Message);
                        }
                    }, result, TaskCreationOptions.LongRunning);
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

                DataAccess.Instance.SaveHistory(operatorCountryId, operatorUserId, (int)operationType, operationId, brIsoId, brId, vId, v2Id, count, details);
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

                var list = DataAccess.Instance.SelectHistoryByCountryAndOperator(operatorCountryId, operatorUserId, (int)operationType, from, to);

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

                var list = DataAccess.Instance.SelectRetailerPrinterData(countryId);

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

                if (string.IsNullOrWhiteSpace(whereClause))
                    throw new ArgumentException("whereClause");

                if (whereClause.Contains((c) => c == ';'))
                    throw new SecurityException("Wrong data");

                var dblist = DataAccess.Instance.SelectVouchersBySql(whereClause);
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

                if (string.IsNullOrWhiteSpace(whereClause))
                    throw new ArgumentException("whereClause");

                if (whereClause.Contains((c) => c == ';'))
                    throw new SecurityException("Wrong data");

                if (string.IsNullOrWhiteSpace(setSql))
                    throw new ArgumentException("setClause");

                if (isVoucher)
                    DataAccess.Instance.UpdateVouchersBySql(setSql, whereClause);
                else
                    DataAccess.Instance.UpdateFilesBySql(setSql, whereClause);
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
                DataAccess.Instance.AddFolder(toParentId, name, countryId, userId);
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
                DataAccess.Instance.DeleteFolder(folderId);
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
                DataAccess.Instance.DeleteFile(id, isVoucher);
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
                var db = DataAccess.Instance;
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
                DataAccess.Instance.RenameFolder(folderId, name);
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
                DataAccess.Instance.UpdateFolder(folderId, name, parentId);
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
                var dblist = DataAccess.Instance.SelectAllByParent(parentId, createdByIsoId);
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
                var dblist = DataAccess.Instance.SelectVouchersByFolder(folderId);
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
                var dblist = DataAccess.Instance.SelectFilesByFolder(folderId);
                var list = dblist.ConvertAll(f => new file2Info(f));
                return list;
            }
            catch (Exception ex)
            {
                throw new FaultException<MyApplicationFault>(new MyApplicationFault(), ex.Message);
            }
        }

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

                    if (buffer.IsFirstRun)
                    {
                        if (isVoucher)
                        {
                            if (signed)
                            {
                                //signed voucher
                                var db = DataAccess.Instance;
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

                                    var files = fileAccess.Instance.ExtractFileZip(exportZipFile.FullName, exportDirectory.FullName);
                                    var imageFileToSing = files.Max((f, f1) => f.Length > f1.Length);

                                    using (var bmp = (Bitmap)Image.FromFile(imageFileToSing.FullName))
                                    {
                                        var resultFile = pdfFileAccess.Instance.CreateSignPdf(bmp, "barcode", countryName, "Madrid", vinfo.branch_id, vinfo.v_number);
                                        var imageFileName = operationDirectory.CombineFileName(vinfo.session_Id + ".pdf").FullName;
                                        File.Move(resultFile, imageFileName);

                                        fileAccess.Instance.CreateZip(operationZipFile.FullName, operationDirectory.FullName, "File created at: " + DateTime.Now);
                                        buffer.Buffer = operationZipFile.ReadAllBytes();
                                    }
                                }
                                finally
                                {
                                    exportDirectory.DeleteSafe(true);
                                    exportZipFile.DeleteSafe();
                                    operationZipFile.DeleteSafe();
                                }
                            }
                            else
                            {
                                //cover page
                                buffer.Buffer = DataAccess.Instance.SelectVoucherById(fileId, true);
                            }
                        }
                        else
                        {
                            //voucher
                            buffer.Buffer = DataAccess.Instance.SelectVoucherById(fileId, false);
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

                var list = DataAccess.Instance.GetTransferFileReport(countryId, beginNumber, endNumber, siteCode).ConvertAll((d) => new TransferFileInfo(d));
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
            return e1 == e2.Reverse();
        }

        #endregion

    }
}