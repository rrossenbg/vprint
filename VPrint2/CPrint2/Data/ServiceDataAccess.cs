/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

#define SCANNING

using System;
using System.Diagnostics;
using System.IO;
using CPrint2;
using CPrint2.ScanServiceRef;
using CPrint2.Common;

namespace CPrint2.Data
{
    public class ServiceDataAccess
    {
        public static ServiceDataAccess Instance { get { return new ServiceDataAccess(); } }

        #region SCAN

        public void SendFile(FileInfo info, string serverSessionId, Tuple<string, string> keys)
        {
            var client = ScanServiceClient.CreateProxy(Program.SCAN_IP);
            try
            {
                var name = info.Name.ToUniqueFileName();
                info.SlimCopy((data, read) => { client.SaveData(serverSessionId, name, data.Copy(read), keys.Item1, keys.Item2); });
            }
            finally
            {
                ((IDisposable)client).Dispose();
            }
        }

        public void SendFileAsync(FileInfo info, string serverSessionId, Tuple<string, string> keys)
        {
            var client = ScanServiceClient.CreateProxy(Program.SCAN_IP);
            try
            {
                var name = info.Name.ToUniqueFileName();
                info.SlimCopyAsync((data, pos, read) => { client.SaveDataAsync(serverSessionId, name, data.Copy(read), pos, keys.Item1, keys.Item2); });
            }
            finally
            {
                ((IDisposable)client).Dispose();
            }
        }

        public void CommitVoucherChanges(string serverSessionId, int jobId, int countryId, int retailerId, int voucherId,
            int? folderId, string siteCode, string barCode, Tuple<string, string> keys)
        {
            var client2 = ScanServiceClient.CreateProxy(Program.SCAN_IP);
            try
            {
                client2.CommitVoucherChanges(serverSessionId, jobId, countryId, retailerId,
                    voucherId, folderId, siteCode, barCode, Program.currentUser.CountryID, Program.currentUser.UserID, keys.Item1, keys.Item2);
            }
            finally
            {
                ((IDisposable)client2).Dispose();
            }
        }

        public void CommitFileChanges(string serverSessionId, int countryId, int? folderId, Tuple<string, string> keys)
        {
            var client2 = ScanServiceClient.CreateProxy(Program.SCAN_IP);
            try
            {
                client2.CommitFileChanges(serverSessionId, countryId, folderId, Program.currentUser.CountryID, Program.currentUser.UserID, keys.Item1, keys.Item2);
            }
            finally
            {
                ((IDisposable)client2).Dispose();
            }
        }

        public void ValidateVoucherThrow(int countryId, bool ss, int retailerId, int voucherId, bool voucherMustExist)
        {
            var keys = Security.CreateInstance().GenerateSecurityKeys();
            var client = ScanServiceClient.CreateProxy(Program.SCAN_IP);
            try
            {
                client.ValidateVoucher(countryId, ss, retailerId, voucherId, voucherMustExist, keys.Item1, keys.Item2);
            }
            finally
            {
                ((IDisposable)client).Dispose();
            }
        }

        public string FindVoucher(int countryId, int voucherId, int voucherIdCD)
        {
            var client = ScanServiceClient.CreateProxy(Program.SCAN_IP);
            try
            {
                var keys = Security.CreateInstance().GenerateSecurityKeys();
                var result = client.FindVoucher(countryId, voucherId, voucherIdCD, keys.Item1, keys.Item2);
                return result;
            }
            finally
            {
                ((IDisposable)client).Dispose();
            }
        }

        #endregion

        #region SEARCH

        public HistoryByCountryInfo[] ReadHistory(OperationHistory data, DateTime fromTime, DateTime toTime)
        {
            IScanService client = null;
            try
            {
                client = ScanServiceClient.CreateProxy(Program.SCAN_IP);
                var keys = Security.CreateInstance().GenerateSecurityKeys();
                var list = client.ReadHistory(Program.currentUser.CountryID, Program.currentUser.UserID,
                    data, fromTime, toTime, keys.Item1, keys.Item2);
                return list;
            }
            finally
            {
                ((IDisposable)client).DisposeSf();
            }
        }

        public void SaveHistory(OperationHistory operationType, string details)
        {
            var keys = Security.CreateInstance().GenerateSecurityKeys();
            SaveHistory(operationType, Guid.NewGuid(), 0, 0, 0, 0, 0, details, keys);
        }

        public void SaveHistory(OperationHistory operationType, int brIsoId, int brId, int vId, int v2Id, int count, string details = "")
        {
            var keys = Security.CreateInstance().GenerateSecurityKeys();
            SaveHistory(operationType, Guid.NewGuid(), brIsoId, brId, vId, v2Id, count, details, keys);
        }

        public void SaveHistory(OperationHistory operationType, Guid operationId,
            int brIsoId, int brId, int vId, int v2Id, int count, string details, Tuple<string, string> keys)
        {
            IScanService client = null;
            try
            {
                client = ScanServiceClient.CreateProxy(Program.SCAN_IP);
                client.SaveHistory(Program.currentUser.CountryID, Program.currentUser.UserID, operationType, operationId, brIsoId, brId, vId, v2Id, count, details,
                    keys.Item1, keys.Item2);
            }
            finally
            {
                ((IDisposable)client).DisposeSf();
            }
        }

        public fileInfo[] SelectFilesBySql(string whereCause)
        {
            IScanService client = null;
            try
            {
                client = ScanServiceClient.CreateProxy(Program.SCAN_IP);
                var keys = Security.CreateInstance().GenerateSecurityKeys();
                var list = client.SelectFilesBySql(whereCause, keys.Item1, keys.Item2);
                return list;
            }
            finally
            {
                ((IDisposable)client).DisposeSf();
            }
        }

        #endregion

        #region FOLDERS

        public void AddFolder(int? parentId, string name)
        {
            IScanService client = null;
            try
            {
                client = ScanServiceClient.CreateProxy(Program.SCAN_IP);
                var keys = Security.CreateInstance().GenerateSecurityKeys();
                client.AddFolder(parentId, name, Program.currentUser.CountryID, Program.currentUser.UserID, keys.Item1, keys.Item2);
            }
            finally
            {
                ((IDisposable)client).DisposeSf();
            }
        }

        public void DeleteFolder(int id)
        {
            IScanService client = null;
            try
            {
                client = ScanServiceClient.CreateProxy(Program.SCAN_IP);
                var keys = Security.CreateInstance().GenerateSecurityKeys();
                client.DeleteFolder(id, keys.Item1, keys.Item2);
            }
            finally
            {
                ((IDisposable)client).DisposeSf();
            }
        }

        public void DeleteFile(int id, bool isVoucher)
        {
            IScanService client = null;
            try
            {
                client = ScanServiceClient.CreateProxy(Program.SCAN_IP);
                var keys = Security.CreateInstance().GenerateSecurityKeys();
                client.DeleteFile(id, isVoucher, keys.Item1, keys.Item2);
            }
            finally
            {
                ((IDisposable)client).DisposeSf();
            }
        }

        public void DeleteAllFilesInFolder(int folderId)
        {
            IScanService client = null;
            try
            {
                client = ScanServiceClient.CreateProxy(Program.SCAN_IP);
                var keys = Security.CreateInstance().GenerateSecurityKeys();
                client.DeleteAllFilesInFolder(folderId, keys.Item1, keys.Item2);
            }
            finally
            {
                ((IDisposable)client).DisposeSf();
            }
        }

        public void RenameFolder(int id, string name)
        {
            IScanService client = null;
            try
            {
                client = ScanServiceClient.CreateProxy(Program.SCAN_IP);
                var keys = Security.CreateInstance().GenerateSecurityKeys();
                client.RenameFolder(id, name, keys.Item1, keys.Item2);
            }
            finally
            {
                ((IDisposable)client).DisposeSf();
            }
        }

        public FolderInfo[] ReadFolderList(int? parentId, int countryId)
        {
            IScanService client = null;
            try
            {
                client = ScanServiceClient.CreateProxy(Program.SCAN_IP);
                var keys = Security.CreateInstance().GenerateSecurityKeys();
                var list = client.SelectFoldersByParent(parentId, countryId, keys.Item1, keys.Item2);
                return list;
            }
            finally
            {
                ((IDisposable)client).DisposeSf();
            }
        }

        public fileInfo[] ReadFileList(int folderId)
        {
            IScanService client = null;
            try
            {
                client = ScanServiceClient.CreateProxy(Program.SCAN_IP);
                var keys = Security.CreateInstance().GenerateSecurityKeys();
                var list = client.SelectFilesByFolder(folderId, keys.Item1, keys.Item2);
                return list;
            }
            finally
            {
                ((IDisposable)client).DisposeSf();
            }
        }

        public file2Info[] ReadCoverList(int folderId)
        {
            IScanService client = null;
            try
            {
                client = ScanServiceClient.CreateProxy(Program.SCAN_IP);
                var keys = Security.CreateInstance().GenerateSecurityKeys();
                var list = client.SelectCoversByFolder(folderId, keys.Item1, keys.Item2);
                return list;
            }
            finally
            {
                ((IDisposable)client).DisposeSf();
            }
        }

        public string ReceiveFile(int fileId, bool isVoucher, string folder)
        {
            var finfo = new FileInfo(Path.Combine(folder, "dwn".Unique(".zip")));

            IScanService client = null;

            try
            {
                client = ScanServiceClient.CreateProxy(Program.SCAN_IP);
                var keys = Security.CreateInstance().GenerateSecurityKeys();

                for (; ; )
                {
                    using (var file = finfo.Open(FileMode.OpenOrCreate))
                    {
                        file.Seek(file.Length, SeekOrigin.Begin);

                        var buffer = client.SelectFileById(fileId, isVoucher, (int)file.Length, keys.Item1, keys.Item2);

                        if (buffer.Length == 0)
                            return finfo.FullName;

                        file.Write(buffer, 0, buffer.Length);
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                return null;
            }
            finally
            {
                ((IDisposable)client).DisposeSf();
            }
        }

        #endregion

        #region VERSION UPDATE

        public UpdateFileInfo[] GetVersionInfo(string currentVersion)
        {
            var client = ScanServiceClient.CreateProxy(Program.SCAN_IP);
            try
            {
                var keys = Security.CreateInstance().GenerateSecurityKeys();
                var results = client.GetVersionInfo(currentVersion, keys.Item1, keys.Item2);
                return results;
            }
            finally
            {
                ((IDisposable)client).Dispose();
            }
        }

        public byte[] ReadVersionFile(string fileName, long from)
        {
            var client = ScanServiceClient.CreateProxy(Program.SCAN_IP);
            try
            {
                var keys = Security.CreateInstance().GenerateSecurityKeys();
                var buffer = client.ReadVersionFile(fileName, from, keys.Item1, keys.Item2);
                return buffer;
            }
            finally
            {
                ((IDisposable)client).Dispose();
            }
        }

        #endregion

        #region TRANSFER FILE

        public TransferFileInfo[] GetTransferFile(int countryId, int beginNumber, int endNumber, string siteCode)
        {
            IScanService client = null;
            try
            {
                client = ScanServiceClient.CreateProxy(Program.SCAN_IP);
                var keys = Security.CreateInstance().GenerateSecurityKeys();
                return client.GetTransferFile(countryId, beginNumber, endNumber, siteCode, keys.Item1, keys.Item2);
            }
            finally
            {
                ((IDisposable)client).DisposeSf();
            }
        }

        #endregion

        #region LOG

        public void LogOperation(OperationHistory operation, Guid session, int countryId, int branchId, int voucherId,
            int voucher2Id = 0, int count = 0, string message = null)
        {
#if SCANNING
            IScanService client = null;
            try
            {
                client = ScanServiceClient.CreateProxy(Program.SCAN_IP);
                var keys = Security.CreateInstance().GenerateSecurityKeys();


                client.SaveHistory(Program.currentUser.CountryID, Program.currentUser.UserID,
                    operation, session, countryId, branchId, voucherId, voucher2Id, count, message, keys.Item1, keys.Item2);

            }
            finally
            {
                ((IDisposable)client).DisposeSf();
            }
#endif
        }

        #endregion
    }
}
