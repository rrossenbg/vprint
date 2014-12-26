/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml.Linq;
using Ionic.Zip;

namespace VPrinting
{
    public class ZipFileAccess
    {
        public static ZipFileAccess Instance { get { return new ZipFileAccess(); } }

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

        public IEnumerable<FileInfo> ExtractFileZip(string zipFilePath, string toDirName)
        {
            Debug.Assert(zipFilePath != null);
            Debug.Assert(toDirName != null);

            using (ZipFile zip = new ZipFile(zipFilePath))
            {
                var en = zip.GetEnumerator();
                while (en.MoveNext())
                {
                    en.Current.Extract(toDirName, ExtractExistingFileAction.OverwriteSilently);
                    yield return new FileInfo(Path.Combine(toDirName, en.Current.FileName));
                }
            }
        }

        public void ExtractFileZip(string zipFilePath, string fileToExtract, string toDirName)
        {
            Debug.Assert(zipFilePath != null);
            Debug.Assert(fileToExtract != null);
            Debug.Assert(toDirName != null);

            using (ZipFile zip = new ZipFile(zipFilePath))
            {
                var en = zip.GetEnumerator();
                while (en.MoveNext())
                {
                    if (string.Equals(en.Current.FileName, fileToExtract, StringComparison.InvariantCultureIgnoreCase))
                    {
                        en.Current.Extract(toDirName, ExtractExistingFileAction.OverwriteSilently);
                        break;
                    }
                }
            }
        }

        //C:\VOUCHERS\[CountryID]\[RetailerId]\[VoucherId]
        public DirectoryInfo CreateDirectoryHerarchy(string vouchersFolder, int countryId, int retailerId, int voucherId)
        {
            var voucherRoot = new DirectoryInfo(vouchersFolder);
            voucherRoot.EnsureDirectory();

            var countryDir = voucherRoot.Combine(countryId.ToString());
            countryDir.EnsureDirectory();

            var retailerDir = countryDir.Combine(retailerId.ToString());
            retailerDir.EnsureDirectory();

            var voucherDir = retailerDir.Combine(voucherId.ToString());
            voucherDir.EnsureDirectory();

            return voucherDir;
        }

        //public DirectoryInfo CreateDirectoryHerarchy(string rootName, VoucherInfo2 info)
        //{
        //    var dir = new DirectoryInfo(rootName);
        //    dir.CreateIfNotExist();
        //    var iso = dir.Combine(info.IsoId.ToString()).CreateIfNotExist();
        //    var retailerId = iso.Combine(info.RetailerId.ToString()).CreateIfNotExist();
        //    var voucherId = retailerId.Combine(info.VoucherId.ToString()).CreateIfNotExist();
        //    return voucherId;
        //}

        public void ReadCoversheetXml(string xmlPath, out int? folderId,
    out int userId, out int locationId, out string sessionId)
        {
            var xml = XDocument.Load(xmlPath);
            userId = xml.Root.ElementThrow("OperatorID").Value.ConvertToThrow<string, int>("OperatorID");
            locationId = xml.Root.ElementThrow("LocationID").Value.ConvertToThrow<string, int>("LocationID");
            sessionId = xml.Root.ElementThrow("SessionID").Value;
            folderId = xml.Root.ElementThrow("FolderID").Value.IsNullOrWhiteSpace() ? (int?)null :
                        xml.Root.ElementThrow("FolderID").Value.ConvertToThrow<string, int>("FolderID");
        }

        public void ReadVoucherXml(string xmlPath, out int jobId, out int countryId, out int retailerId, out int voucherId, out int? folderId,
            out string siteCode, out string barCode, out int userId, out int locationId, out string sessionId, out int? v_type)
        {
            var xml = XDocument.Load(xmlPath);

            jobId = xml.Root.ElementThrow("JobID").Value.ConvertToThrow<string, int>("JobID");

            countryId = xml.Root.ElementThrow("CountryID").Value.ConvertToThrow<string, int>("CountryID");

            retailerId = xml.Root.ElementThrow("RetailerID").Value.ConvertToThrow<string, int>("RetailerID");

            voucherId = xml.Root.ElementThrow("VoucherID").Value.ConvertToThrow<string, int>("VoucherID");

            siteCode = xml.Root.ElementThrow("SiteCode").Value;

            barCode = xml.Root.ElementThrow("BarCode").Value;

            userId = xml.Root.ElementThrow("OperatorID").Value.ConvertToThrow<string, int>("OperatorID");

            locationId = xml.Root.ElementThrow("LocationID").Value.ConvertToThrow<string, int>("LocationID");

            sessionId = xml.Root.ElementThrow("SessionID").Value;

            folderId = xml.Root.ElementThrow("FolderID").Value.IsNullOrWhiteSpace() ? (int?)null :
                        xml.Root.ElementThrow("FolderID").Value.ConvertToThrow<string, int>("FolderID");

            v_type = xml.Root.ElementValueOrDefault("TypeID", null).IsNullOrWhiteSpace() ? (int?)null :
                        xml.Root.ElementThrow("TypeID").Value.ConvertToThrow<string, int>("TypeID");
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
             string siteCode, string barCode, int userId, int locationId, string sessionId, int typeId)
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
                    new XElement("TypeID", typeId),
                    new XElement("CreateAt", DateTime.Now));

            xml.Save(xmlName.FullName);
        }
    }
}
