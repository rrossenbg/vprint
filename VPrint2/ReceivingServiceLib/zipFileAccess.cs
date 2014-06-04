/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.IO;
using System.Xml.Linq;
using Ionic.Zip;

namespace ReceivingServiceLib
{
    public class zipFileAccess
    {
        public static zipFileAccess Instance { get { return new zipFileAccess(); } }

        public void CreateZip(string zipFilePath, string fromDirName, string message)
        {
            using (ZipFile zip = new ZipFile())
            {
                zip.UseUnicodeAsNecessary = true;  // utf-8
                zip.AddDirectory(fromDirName);
                zip.Comment = message;
                zip.Save(zipFilePath);
            }
        }

        public void RestoreZip(string zipFilePath, string toDirName)
        {
            using (ZipFile zip = new ZipFile(zipFilePath))
                zip.ExtractAll(toDirName, ExtractExistingFileAction.OverwriteSilently);
        }

        //C:\VOUCHERS\[CountryID]\[RetailerId]\[VoucherId]
        public DirectoryInfo CreateDirectoryHerarchy(string vouchersFolder, int countryId, int retailerId, int voucherId)
        {
            var voucherRoot = new DirectoryInfo(vouchersFolder);
            voucherRoot.EnsureDirectory();
            var countryDirectory = voucherRoot.Combine(countryId.ToString());
            countryDirectory.EnsureDirectory();
            var retailerDirectory = countryDirectory.Combine(retailerId.ToString());
            retailerDirectory.EnsureDirectory();
            var voucherDirectory = retailerDirectory.Combine(voucherId.ToString());
            voucherDirectory.EnsureDirectory();
            return voucherDirectory;
        }

        public DirectoryInfo CreateDirectoryHerarchy(string rootName, VoucherInfo2 info)
        {
            var dir = new DirectoryInfo(rootName);
            dir.CreateIfNotExist();
            var iso = dir.Combine(info.IsoId.ToString()).CreateIfNotExist();
            var retailerId = iso.Combine(info.RetailerId.ToString()).CreateIfNotExist();
            var voucherId = retailerId.Combine(info.VoucherId.ToString()).CreateIfNotExist();
            return voucherId;
        }

        public void ReadCoversheetXml(string xmlPath, out int? folderId,
            out int userId, out int locationId, out string sessionId)
        {
            var xml = XDocument.Load(xmlPath);
            userId = xml.Root.ElementThrow("OperatorID").Value.ConvertTo<string, int>("OperatorID");
            locationId = xml.Root.ElementThrow("LocationID").Value.ConvertTo<string, int>("LocationID");
            sessionId = xml.Root.ElementThrow("SessionID").Value;
            folderId = xml.Root.ElementThrow("FolderID").Value.IsNullOrWhiteSpace() ? (int?)null :
                        xml.Root.ElementThrow("FolderID").Value.ConvertTo<string, int>("FolderID");
        }

        public void ReadVoucherXml(string xmlPath, out int jobId, out int countryId, out int retailerId, out int voucherId, out int? folderId,
            out string siteCode, out string barCode, out int userId, out int locationId, out string sessionId)
        {
            var xml = XDocument.Load(xmlPath);
            jobId = xml.Root.ElementThrow("JobID").Value.ConvertTo<string, int>("JobID");
            countryId = xml.Root.ElementThrow("CountryID").Value.ConvertTo<string, int>("CountryID");
            retailerId = xml.Root.ElementThrow("RetailerID").Value.ConvertTo<string, int>("RetailerID");
            voucherId = xml.Root.ElementThrow("VoucherID").Value.ConvertTo<string, int>("VoucherID");
            siteCode = xml.Root.ElementThrow("SiteCode").Value;
            barCode = xml.Root.ElementThrow("BarCode").Value;
            userId = xml.Root.ElementThrow("OperatorID").Value.ConvertTo<string, int>("OperatorID");
            locationId = xml.Root.ElementThrow("LocationID").Value.ConvertTo<string, int>("LocationID");
            sessionId = xml.Root.ElementThrow("SessionID").Value;
            folderId = xml.Root.ElementThrow("FolderID").Value.IsNullOrWhiteSpace() ? (int?)null :
                        xml.Root.ElementThrow("FolderID").Value.ConvertTo<string, int>("FolderID");
        }

        public void SaveCoversheetXml(FileInfo xmlName, int countryId, int? folderId,
            int userId, int locationId, string sessionId)
        {
            XElement xml =
                new XElement("Voucher",
                    new XElement("CountryID", countryId),
                    new XElement("OperatorID", userId),
                    new XElement("LocationID", locationId),
                    new XElement("SessionID", sessionId),
                    new XElement("FolderID", folderId),
                    new XElement("CreateAt", DateTime.Now));

            xml.Save(xmlName.FullName);
        }

        public void SaveVoucherXml(FileInfo xmlName, int jobId, int countryId, int retailerId, int voucherId, int? folderId,
             string siteCode, string barCode, int userId, int locationId, string sessionId)
        {
            XElement xml =
                new XElement("Voucher",
                    new XElement("JobID", jobId),
                    new XElement("CountryID", countryId),
                    new XElement("RetailerID", retailerId),
                    new XElement("VoucherID", voucherId),
                    new XElement("SiteCode", siteCode),
                    new XElement("BarCode", barCode),
                    new XElement("OperatorID", userId),
                    new XElement("LocationID", locationId),
                    new XElement("SessionID", sessionId),
                    new XElement("FolderID", folderId),
                    new XElement("CreateAt", DateTime.Now));

            xml.Save(xmlName.FullName);
        }
    }
}
