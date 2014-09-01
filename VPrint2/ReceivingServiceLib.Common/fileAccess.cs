/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Ionic.Zip;

namespace ReceivingServiceLib
{
    public class fileAccess
    {
        public static fileAccess Instance { get { return new fileAccess(); } }

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
    }
}
