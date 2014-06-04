/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using ReceivingServiceLib.Data;

namespace ReceivingServiceLib.FileWorkers
{
    public class ImportFileWorker : FileWorkerBase
    {
        protected static ImportFileWorker ms_instance;
        public static ImportFileWorker Default
        {
            get
            {
                if (ms_instance == null)
                    ms_instance = new ImportFileWorker();
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
                    var errorRoot = new DirectoryInfo(Global.Strings.UPLOADERRORS);
                    errorRoot.CreateIfNotExist();

                    var uploadRoot = new DirectoryInfo(Global.Strings.UPLOADROOT);
                    uploadRoot.CreateIfNotExist();

                    //TODO: When it starts if there are zip insert them to the database

                    //var zips = uploadRoot.GetFiles(".zip");
                    //foreach (var zip in zips)
                    //{
                    //}

                    var dirs = uploadRoot.GetDirectories();                    

                    if (dirs.Length != 0)
                    {
                        int jobId = 0, countryId = 0, retailerId = 0, voucherId = 0, userId, locationId;
                        int? folderId;
                        string siteCode = null, barCode = null, sessionId;
                        string message = null;

                        foreach (var fromDir in dirs)
                        {
                            try
                            {
                                Trace.WriteLine("Trying ".concat(fromDir.Name), Strings.APPNAME);

                                if (!fromDir.Exists)
                                    continue;

                                if (ScanService.FileLocks.Contains(fromDir.Name, StringComparer.InvariantCultureIgnoreCase))
                                    continue;

                                bool isVoucher = false;

                                var fac = new zipFileAccess();

                                string xmlPath = Path.Combine(uploadRoot.FullName, fromDir.Name, "data.xml");
                                if (File.Exists(xmlPath))
                                {
                                    isVoucher = true;

                                    fac.ReadVoucherXml(xmlPath, out jobId, out countryId, out retailerId, out voucherId, out folderId,
                                        out siteCode, out barCode, out userId, out locationId, out sessionId);

                                    message = string.Concat("Job ID: ", jobId, Environment.NewLine, "Country ID: ", countryId, Environment.NewLine, "Retailer ID: ", retailerId, Environment.NewLine,
                                       "Voucher ID: ", voucherId, Environment.NewLine, "Folder ID: ", folderId, Environment.NewLine, "Site code: ", siteCode, Environment.NewLine,
                                       "User ID: ", userId, Environment.NewLine, "Location ID: ", locationId, Environment.NewLine, "Session ID: ", sessionId, Environment.NewLine,
                                       string.Format("Archive created: {0:G}", DateTime.Now));
                                }
                                else
                                {
                                    isVoucher = false;

                                    xmlPath = Path.Combine(uploadRoot.FullName, fromDir.Name, "cover.xml");

                                    if (File.Exists(xmlPath))
                                    {
                                        fac.ReadCoversheetXml(xmlPath, out folderId, out userId, out locationId, out sessionId);

                                        message = string.Concat("User ID: ", userId, Environment.NewLine, "Location ID: ", locationId, Environment.NewLine, "Session ID: ", sessionId, Environment.NewLine,
                                            string.Format("Archive created: {0:G}", DateTime.Now));
                                    }
                                    else
                                    {
                                        throw new Exception("Cannot find: " + xmlPath);
                                    }
                                }

                                Trace.WriteLine("Create zip ".concat(fromDir.Name), Strings.APPNAME);

                                string zipPath = Path.Combine(uploadRoot.FullName, string.Concat(fromDir.Name, ".zip"));
                                var zipFile = new FileInfo(zipPath);
                                zipFile.DeleteSafe();
                                
                                fac.CreateZip(zipPath, fromDir.FullName, message);

                                Trace.WriteLine("Read xml ".concat(fromDir.Name), Strings.APPNAME);
                                Trace.WriteLine("Sql Insert ".concat(fromDir.Name), Strings.APPNAME);

                                using (var file = zipFile.Open(FileMode.Open))
                                using (var reader = new BinaryReader(file))
                                {
                                    var length = reader.Read(m_Buffer50MB, 0, (int)file.Length);

                                    if (isVoucher)
                                    {
                                        DataAccess.Instance.AddVoucher(jobId, countryId, retailerId, voucherId, folderId,
                                            siteCode, barCode, locationId, userId, m_Buffer50MB, length, sessionId);
                                    }
                                    else
                                    {
                                        DataAccess.Instance.AddCoversheet(folderId,
                                            locationId, userId, m_Buffer50MB, length, sessionId);
                                    }
                                }

                                Trace.WriteLine("Save to history ".concat(fromDir.Name), Strings.APPNAME);
                                
                                var voucherDirectory = fac.CreateDirectoryHerarchy(Global.Strings.VOCUHERSFOLDER, countryId, retailerId, voucherId);
                                fromDir.CopyFiles(voucherDirectory, true);

                                Trace.WriteLine("Clean up ".concat(fromDir.Name), Strings.APPNAME);

                                fromDir.DeleteSafe(true);
                                zipFile.DeleteSafe();

                                Array.Clear(m_Buffer50MB, 0, m_Buffer50MB.Length);
                            }
                            catch (Exception ex)
                            {
                                FireError(ex);

                                try
                                {
                                    var errDir = errorRoot.Combine(fromDir.Name);
                                    errDir.CreateIfNotExist();
                                    fromDir.MoveTo(errDir);
                                }
                                catch(Exception ex2)
                                {
                                    FireError(ex2);
                                }
                            }
                            finally
                            {
                                Trace.WriteLine("===================================", Strings.APPNAME);
                                Thread.Yield();
                            }
                        }
                    }
                    else
                    {
                        Thread.Sleep(EMPTYTIMEOUT);
                    }
                }
                catch (Exception ex)
                {
                    FireError(ex);
                }
                finally
                {
                    Thread.Sleep(TIMEOUT);
                }
            }
        }
    }
}