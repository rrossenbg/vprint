﻿
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Xml.Linq;

namespace ReceivingServiceLib.FileWorkers
{
    public class ExportFileWorker : FileWorkerBase
    {
        protected static ExportFileWorker ms_instance;

        public static ExportFileWorker Default
        {
            get
            {
                if (ms_instance == null)
                    ms_instance = new ExportFileWorker();
                return ms_instance;
            }
        }

        protected override void WorkerThreadFunction()
        {
            Debug.Assert(Global.Strings.IsValid());

            while (true)
            {
                try
                {
                    var extractRoot = new DirectoryInfo(Global.Strings.VOCUHERSEXPORTFOLDER);
                    var zips = extractRoot.GetFiles("*.zip");

                    if (zips.Length != 0)
                    {
                        foreach (var zipfile in zips)
                        {
                            var zip = new fileAccess();
                            DirectoryInfo unzipDir = null;
                            FileInfo xmlPath = null;
                            try
                            {
                                unzipDir = extractRoot.CreateSubdirectory(zipfile.GetFileName());
                                zip.RestoreZip(zipfile.FullName, unzipDir.FullName);

                                xmlPath = unzipDir.CombineFileName("data.xml");
                                if (!xmlPath.Exists)
                                    continue;

                                XDocument xml = XDocument.Load(xmlPath.FullName);
                                int jobId = xml.Root.ElementThrow("JobID").Value.ConvertTo<string, int>("JobID");
                                int countryId = xml.Root.ElementThrow("CountryID").Value.ConvertTo<string, int>("CountryID");
                                int retailerId = xml.Root.ElementThrow("RetailerID").Value.ConvertTo<string, int>("RetailerID");
                                int voucherId = xml.Root.ElementThrow("VoucherID").Value.ConvertTo<string, int>("VoucherID");
                                string sessionId = xml.Root.ElementValueOrDefault("SessionID", zipfile.GetFileNameWithoutExtension());

                                var fac2 = new fileAccess();
                                var voucherDirectory = fac2.CreateDirectoryHerarchy(Global.Strings.VOCUHERSFOLDER, 
                                    countryId, retailerId, voucherId);
                                unzipDir.CopyFiles(voucherDirectory, true);
                            }
                            catch (Exception ex)
                            {
                                FireError(ex);
                            }
                            finally
                            {
                                if (zipfile != null)
                                    zipfile.DeleteSafe();

                                if (unzipDir != null)
                                    unzipDir.DeleteSafe(true);
                            }
                        }
                    }
                    else
                    {
                        Thread.Sleep(EMPTYTIMEOUT);
                    }
                }
                catch (Exception ex2)
                {
                    FireError(ex2);
                }
                finally
                {
                    Thread.Sleep(TIMEOUT);
                }
            }
        }
    }
}
